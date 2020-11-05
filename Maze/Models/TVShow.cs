using System.Collections.Generic;

namespace Maze.Models
{
    public class TVShow
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TVShowActor> TVShowActors { get; set; } = new List<TVShowActor>();
    }
}
