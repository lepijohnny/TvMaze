using Maze.Repositories;
using System.Threading.Tasks;

namespace Maze.Commands
{
    public interface IApiCommand<T>
    {
        Task<IApiCommandQuery<T>> Execute();
    }
}
