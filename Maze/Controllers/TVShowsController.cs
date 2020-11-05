using System;
using System.Threading.Tasks;
using Maze.Commands;
using Maze.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Maze.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TVShowsController : ControllerBase, IDisposable
    {
        private IUnitOfWork _unitOfWork;
        private readonly ILogger<TVShowsController> _logger;

        private bool _disposed = false;

        public TVShowsController(IUnitOfWork unitOfWork, ILogger<TVShowsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET api/tvshows
        [HttpGet]
        public async Task<ActionResult<string>> Get(int page)
        {
            string apiMethod = $"api/tvshows?page={page}";

            _logger.LogInformation($"{apiMethod} started ...");

            var cmd = new GetPaginatedTVShowsCommand(_unitOfWork, Math.Max(0, page));

            try
            {
                var result = await cmd.Execute();
   
                var (payload, last, totalCount) = result.Value;

                Response.Headers.Add("X-Pagination-Last", last.ToString());
                Response.Headers.Add("X-Pagination-Total", totalCount.ToString());

                _logger.LogInformation($"{apiMethod} finised ...");

                return Ok(payload);
            }
            catch(Exception ex)
            {
                _logger.LogError($"{apiMethod} thrown an exception: {ex} ...");
                return StatusCode(500);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _unitOfWork?.Dispose();
                _unitOfWork = null;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
