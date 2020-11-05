using Maze.DTO;
using Maze.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maze.Scaraper
{
    public interface IHttpClient<T>
    {
        Task<IEnumerable<T>> GetAsync(string path);
    }

    public abstract class ThrottableScraperHttpClient<T>
    {
        private readonly IHttpDeserializer<T> _httpDeserializer;
        private readonly IHttpRateLimiter _httpRateLimiter;

        protected HttpClient Client { get; }

        protected ThrottableScraperHttpClient(IHttpClientFactory httpClientFactory, IHttpDeserializer<T> httpDeserializer, IHttpRateLimiter httpRateLimiter)
        {
            Client = httpClientFactory.CreateClient("tvmaze");
            _httpDeserializer = httpDeserializer;
            _httpRateLimiter = httpRateLimiter;
        }

        /// <summary>
        /// Try to retrieve data from the server using <see cref="HttpClient"/>
        /// </summary>
        /// <param name="path">The path to the resource</param>
        /// <returns>The deserialized<see cref="IEnumerable{T}"/></returns>
        /// <exception cref="HttpRetriableException">In the case rate limiter, we should definetelly try again</exception>
        /// <exception cref="HttpNonRetriableException">In the case of the other error, we skip the call.</exception>
        public async Task<IEnumerable<T>> GetAsync(string path)
        {
            await _httpRateLimiter.Limit();
            {
                var httpResponse = await Client.GetAsync(path);

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    await _httpRateLimiter.BrakeAsync();
                    throw new HttpRetriableException("The rate limiter, try again...");
                }
                else if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpNonRetriableException($"Unexpected error, http status code={httpResponse.StatusCode}");
                }
                else
                {
                    await _httpRateLimiter.UnbrakeAsync();
                }

                return await _httpDeserializer.DeserializeAsync(httpResponse);
            }
        }
    }

    public class TVShowsScraperHttpClient : ThrottableScraperHttpClient<TVShow>, IHttpClient<TVShow>
    {
        public TVShowsScraperHttpClient(IHttpClientFactory httpClientFactory, IHttpDeserializer<TVShow> httpDeserializer, IHttpRateLimiter httpRateLimiter)
            :base(httpClientFactory, httpDeserializer, httpRateLimiter)
        {
        }
    }

    public class ActorsScraperHttpClient : ThrottableScraperHttpClient<Actor>, IHttpClient<Actor>
    {
        public ActorsScraperHttpClient(IHttpClientFactory httpClientFactory, IHttpDeserializer<Actor> httpDeserializer, IHttpRateLimiter httpRateLimiter)
            : base(httpClientFactory, httpDeserializer, httpRateLimiter)
        {
        }
    }
}
