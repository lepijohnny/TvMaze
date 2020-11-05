using Maze.Models;
using System;
using System.Threading.Tasks;

namespace Maze.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private TVShowDbContext _context;

        private bool _disposed = false;

        public UnitOfWork(TVShowDbContext context)
        {
            _context = context;

            TVShows = new TVShowRepository(_context);
            Actors = new ActorRepository(_context);
        }

        public ITVShowRepository TVShows { get; }

        public IActorRepository Actors { get; }

        public Task<int> CommitAsync()
        {
            return _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _context?.Dispose();
                _context = null;             
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
