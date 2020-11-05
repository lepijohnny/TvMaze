using System;
using System.Collections.Generic;

namespace Maze.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DoB { get; set; }

        public ICollection<TVShowActor> TVShowActors { get; set; } = new List<TVShowActor>();
    }
}
