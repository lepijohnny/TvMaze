using System;

namespace Maze.Scaraper
{
    public class HttpNonRetriableException : Exception
    {
        public HttpNonRetriableException(string message)
            : base(message)
        {
        }
    }
}
