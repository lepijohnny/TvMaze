using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Maze.DTO
{
    [DataContract]
    public class TVShowDTO
    {
        public TVShowDTO(int id, string name, List<ActorDTO> actorDTOs)
        {
            Id = id;
            Name = name;
            ActorDTOs = actorDTOs;
        }

        [DataMember(Name = "id")]
        public int Id { get; }

        [DataMember(Name = "name")]
        public string Name { get; }

        [DataMember(Name = "cast")]
        public List<ActorDTO> ActorDTOs { get; }       
    }
}
