using Maze.Models;
using Newtonsoft.Json.Linq;
using System;

namespace Maze.Extensions
{
    public static class Mappers
    {
        public static Func<JToken, TVShow> JsonToTVShowMapper()
        {
            return (token) => new TVShow
            {
                Id = token.Value<int>("id"),
                Name = token.Value<string>("name")
            };
        }

        public static Func<JToken, Actor> JsonToActorMapper()
        {
            return (token) =>
            new Actor
            {
                Id = token["person"].Value<int>("id"),
                Name = token["person"].Value<string>("name"),
                DoB = token["person"].Value<DateTime?>("birthday") ?? DateTime.MinValue
            };
        }
    }
}
