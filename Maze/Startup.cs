using Maze.DTO;
using Maze.Extensions;
using Maze.HostedServices;
using Maze.Models;
using Maze.Repositories;
using Maze.Scaraper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Maze
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //DB
            services.AddDbContext<TVShowDbContext>(ServiceLifetime.Transient);
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //HostedServices
            services.AddHostedService<TVMazeHostedService>();
            services.AddTransient<ITVMazeScraper, TVMazeScraper>();
            services.AddTransient<IHttpClient<TVShow>, TVShowsScraperHttpClient>();
            services.AddTransient<IHttpClient<Actor>, ActorsScraperHttpClient>();
            services.AddSingleton<IHttpDeserializer<TVShow>>(new JsonHttpDeserializer<TVShow>(Mappers.JsonToTVShowMapper()));
            services.AddSingleton<IHttpDeserializer<Actor>>(new JsonHttpDeserializer<Actor>(Mappers.JsonToActorMapper()));
            services.AddSingleton<IHttpRateLimiter, HttpRateLimiter>();

            //Http
            services.AddHttpClient("tvmaze", c =>
            {
                c.BaseAddress = new System.Uri("http://api.tvmaze.com/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");                
            });            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile($"Logging.log");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
