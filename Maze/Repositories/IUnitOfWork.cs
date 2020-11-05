using System;
using System.Threading.Tasks;

namespace Maze.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITVShowRepository TVShows { get; }

        IActorRepository Actors { get; }

        Task<int> CommitAsync();
    }
}
