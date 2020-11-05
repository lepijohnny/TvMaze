using System;
using System.Threading.Tasks;

namespace Maze.Scaraper
{
    public interface ITVMazeScraper : IDisposable
    {
        Task ScrapeAsync();
    }
}
