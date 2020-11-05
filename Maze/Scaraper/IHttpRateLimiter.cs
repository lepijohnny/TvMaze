using System;
using System.Threading;
using System.Threading.Tasks;

namespace Maze.Scaraper
{
    public interface IHttpRateLimiter
    {
        Task Limit();

        Task BrakeAsync();

        Task UnbrakeAsync();

        int TimeoutMillis { get; }
    }

    public class HttpRateLimiter : IHttpRateLimiter, IDisposable
    {
        private const int MaxTimeoutMillis = 10000;
        private const int TimeoutStepMillis = 1000;
        private readonly SemaphoreSlim _semaphoreSlim;

        public int TimeoutMillis { get; private set; } = 0;

        public HttpRateLimiter()
        {
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task BrakeAsync()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                TimeoutMillis = Math.Min(TimeoutMillis + TimeoutStepMillis, MaxTimeoutMillis);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task UnbrakeAsync()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                TimeoutMillis = 0;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<bool> ShouldBreak()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                return TimeoutMillis != 0;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task Limit()
        {
            if(await ShouldBreak())
            {
                await Task.Delay(TimeoutMillis);
            }
        }

        public void Dispose()
        {
            _semaphoreSlim?.Dispose();
        }
    }
}
