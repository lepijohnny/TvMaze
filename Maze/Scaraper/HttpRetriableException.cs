using System;

namespace Maze.Scaraper
{
    public class HttpRetriableException : Exception
    {
        public HttpRetriableException(string message)
            : base(message)
        {
        }
    }
}
