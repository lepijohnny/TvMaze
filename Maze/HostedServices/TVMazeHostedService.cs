using Maze.Scaraper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Maze.HostedServices
{
    public class TVMazeHostedService : IHostedService
    {
        private const int PolingIntervalMillis = 600000;

        private readonly IServiceProvider _services;

        private CancellationTokenSource _myCancellationTokenSource;

        public TVMazeHostedService(IServiceProvider services)
        {
            _services = services;
            _myCancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var joinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _myCancellationTokenSource.Token);

            while (!joinedCancellationTokenSource.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                using (var scraper = scope.ServiceProvider.GetRequiredService<ITVMazeScraper>())
                {                
                    try
                    {
                        await scraper.ScrapeAsync();
                    }
                    catch (Exception)
                    {
                        // do poling every X[ms], ignore this error deliberately
                    }

                    await Task.Delay(PolingIntervalMillis, joinedCancellationTokenSource.Token);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _myCancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
