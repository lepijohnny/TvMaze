using Microsoft.EntityFrameworkCore;

namespace Maze.Models
{
    public class TVShowDbContext : DbContext
    {
        public DbSet<TVShow> TVShows { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<TVShowActor> TVShowActors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Filename=./tvshows.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TVShowActor>()
                .HasKey(x => new { x.TVShowId, x.ActorId });

            modelBuilder.Entity<TVShowActor>()
                .HasOne(x => x.TVShow)
                .WithMany(x => x.TVShowActors)
                .HasForeignKey(x => x.TVShowId);

            modelBuilder.Entity<TVShowActor>()
                .HasOne(x => x.Actor)
                .WithMany(x => x.TVShowActors)
                .HasForeignKey(x => x.ActorId);
        }
    }
}
