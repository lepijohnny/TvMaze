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
    public class TVShowsController : ControllerBase
    {        
        private readonly ILogger<TVShowsController> _logger;

        public TVShowsController(ILogger<TVShowsController> logger)
        {
            _logger = logger;
        }

        // GET api/tvshows
        [HttpGet]
        public async Task<ActionResult<string>> Get([FromServices]IUnitOfWork unitOfWork, int page)
        {
            using (unitOfWork)
            {
                string apiMethod = $"api/tvshows?page={page}";

                _logger.LogInformation($"{apiMethod} started ...");

                var cmd = new GetPaginatedTVShowsCommand(unitOfWork, Math.Max(0, page));

                try
                {
                    var result = await cmd.Execute();

                    var (payload, last, totalCount) = result.Value;

                    Response.Headers.Add("X-Pagination-Last", last.ToString());
                    Response.Headers.Add("X-Pagination-Total", totalCount.ToString());

                    _logger.LogInformation($"{apiMethod} finised ...");

                    return Ok(payload);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{apiMethod} thrown an exception: {ex} ...");
                    return StatusCode(500);
                }
            }
        }
    }
}
