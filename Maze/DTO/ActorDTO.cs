using System;
using System.Runtime.Serialization;

namespace Maze.DTO
{
    [DataContract]
    public class ActorDTO
    {
        public ActorDTO(int id, string name, string dob)
        {
            Id = id;
            Name = name;
            DoB = dob;
        }

        [DataMember(Name = "id")]
        public int Id { get; }

        [DataMember(Name = "name")]
        public string Name { get; }

        [DataMember(Name = "birthday")]
        public string DoB { get; }
    }
}
