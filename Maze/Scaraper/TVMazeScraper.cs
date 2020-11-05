using Maze.Extensions;
using Maze.Models;
using Maze.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maze.Scaraper
{
    public class TVMazeScraper : ITVMazeScraper
    {
        private readonly IHttpClient<TVShow> _tvShowHttpClient;
        private readonly IHttpClient<Actor> _actorHttpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TVMazeScraper> _logger;

        public TVMazeScraper(IHttpClient<TVShow> tvShowHttpClient, IHttpClient<Actor> actorHttpClient, IUnitOfWork unitOfWork, ILogger<TVMazeScraper> logger)
        {
            _tvShowHttpClient = tvShowHttpClient;
            _actorHttpClient = actorHttpClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ScrapeAsync()
        {
            _logger.LogInformation("Scrapper started ...");

            var tvShows = await Retry(_tvShowHttpClient, "shows");

            var tvShowAdded = new List<TVShow>();

            HashSet<int> trackedActorIDs = new HashSet<int>();

            // iterate over the shows and get all the casts
            foreach (var tvShow in await AddRangeIfNotExist(tvShows.Distinct(new SimpleIDComparer<TVShow>(x => x.Id))))
            {
                var actors = await Retry(_actorHttpClient, $"shows/{tvShow.Id}/cast");

                var actorsAdded = await AddCastToTVShow(tvShow, actors.Distinct(new SimpleIDComparer<Actor>(x => x.Id)), trackedActorIDs);

                tvShowAdded.Add(tvShow);

                await _unitOfWork.Actors.AddRangeAsync(actorsAdded);

                trackedActorIDs.AddRange(actorsAdded.Select(a => a.Id));

                await _unitOfWork.TVShows.AddRangeAsync(new[] { tvShow });

                await _unitOfWork.CommitAsync();
            }

            _logger.LogInformation("Scrapper finished ...");
        }

        /// <summary>
        /// Add the <see cref="TVShow"/> which has not been already added to the database
        /// </summary>
        /// <param name="tryToAdd">The <see cref="TVShow"/> comming from the tvmaze.com</param>
        /// <returns>The <see cref="IEnumerable{TVShow}"/> which will be actually added</returns>
        private async Task<IEnumerable<TVShow>> AddRangeIfNotExist(IEnumerable<TVShow> tryToAdd)
        {
            var existingTVShows = await _unitOfWork
                .TVShows
                .GetFilteredAsync(tryToAdd.Select(x => x.Id).ToList());

            HashSet<int> ids = new HashSet<int>(existingTVShows.Select(x => x.Id));

            var toBeAdded = tryToAdd.Where(x => !ids.Contains(x.Id));

            return toBeAdded;
        }

        private async Task<IEnumerable<Actor>> AddRangeIfNotExist(IEnumerable<Actor> tryToAdd, HashSet<int> trackedActorIDs)
        {
            var existingActors = await _unitOfWork
                .Actors
                .GetFilteredAsync(tryToAdd.Select(x => x.Id).ToList());

            HashSet<int> ids = new HashSet<int>(existingActors.Select(x => x.Id));

            var toBeAdded = tryToAdd
                .Where(x => !ids.Contains(x.Id) && !trackedActorIDs.Contains(x.Id));

            return toBeAdded;
        }

        private async Task<IEnumerable<Actor>> AddCastToTVShow(TVShow tvShow, IEnumerable<Actor> actors, HashSet<int> trackedActorIDs)
        {
            foreach (var actor in actors)
            {
                tvShow.TVShowActors.Add(new TVShowActor
                {
                    TVShow = tvShow,
                    Actor = actor
                });
            }

            return await AddRangeIfNotExist(actors, trackedActorIDs);
        }

        private async Task<IEnumerable<T>> Retry<T>(IHttpClient<T> httpClient, string path)
        {
            int retryCount = 0;

            try
            {
                do
                {
                    try
                    {
                        return await httpClient.GetAsync(path);
                    }
                    catch (HttpRetriableException)
                    {
                        // known error, try again
                        _logger.LogInformation($"Retry experienaced api.tvmaze.com rete limitation ...");
                    }

                    retryCount++;
                }
                while (retryCount <= 20); // assumption, can be tweaked, with 20 retry there will be enough time to avoid api.tvmaze.com rate limit
            }
            catch (HttpNonRetriableException ex)
            {
                // known error, go to the next tvshow
                _logger.LogInformation($"Retry thrown an exception: {ex} ...");
            }

            return Enumerable.Empty<T>();
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }

        private class SimpleIDComparer<T> : IEqualityComparer<T>
            where T : class
        {
            private readonly Func<T, int> _idSelector;

            public SimpleIDComparer(Func<T, int> idSelector)
            {
                _idSelector = idSelector;
            }

            public bool Equals(T x, T y)
            {
                if(x == null && y != null)
                {
                    return false;
                }

                if(x != null && y == null)
                {
                    return false;
                }

                return _idSelector(x) == _idSelector(y);
            }

            public int GetHashCode(T obj)
            {
                return _idSelector(obj);
            }
        }
    }
}

