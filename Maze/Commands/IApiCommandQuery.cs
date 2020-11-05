namespace Maze.Commands
{
    public interface IApiCommandQuery<T>
    {
        T Value { get; }
    }
}
