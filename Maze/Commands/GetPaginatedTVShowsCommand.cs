using Maze.DTO;
using Maze.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maze.Commands
{
    public class GetCastCommandQuery : IApiCommandQuery<(List<TVShowDTO>, int, int)>
    {
        public GetCastCommandQuery((List<TVShowDTO>, int, int) value)
        {
            Value = value;
        }

        public (List<TVShowDTO>, int, int) Value { get; }
    }

    public class GetPaginatedTVShowsCommand : IApiCommand<(List<TVShowDTO>, int, int)>
    {
        // This can be send through the query string but for now keep it simple
        private const int PageSize = 20;

        private readonly IUnitOfWork _unitOfWork;
        private readonly int _page;
        

        public GetPaginatedTVShowsCommand(IUnitOfWork unitOfWork, int page)
        {
            _unitOfWork = unitOfWork;
            _page = page;
        }

        public async Task<IApiCommandQuery<(List<TVShowDTO>, int, int)>> Execute()
        {
            var tvShows = await _unitOfWork.TVShows.GetAllAsync(_page, PageSize);

            List<TVShowDTO> tvShowDTOs = new List<TVShowDTO>();

            foreach(var tvShow in tvShows)
            {
                var actors = tvShow.TVShowActors
                    .OrderBy(x => x.Actor.DoB)
                    .Select(x => new ActorDTO(x.Actor.Id, x.Actor.Name, x.Actor.DoB.ToString("yyyy-MM-dd")))
                    .ToList();

                tvShowDTOs.Add(new TVShowDTO(tvShow.Id, tvShow.Name, actors));
            }

            var totalCount = _unitOfWork.TVShows.GetTotalCount();

            return new GetCastCommandQuery((tvShowDTOs, _page*PageSize + PageSize, totalCount));
        }
    }
}
