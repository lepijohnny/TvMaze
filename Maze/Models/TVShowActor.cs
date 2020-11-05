namespace Maze.Models
{
    public class TVShowActor
    {
        public int TVShowId { get; set; }

        public virtual TVShow TVShow { get; set; }

        public int ActorId { get; set; }

        public virtual Actor Actor { get; set; }
    }
}
