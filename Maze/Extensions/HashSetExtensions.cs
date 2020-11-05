using System.Collections.Generic;

namespace Maze.Extensions
{
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> value, IEnumerable<T> items)
        {
            foreach(T i in items)
            {
                value.Add(i);
            }
        }
    }
}
