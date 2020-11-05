using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maze.DTO
{
    public class JsonHttpDeserializer<T> : IHttpDeserializer<T>
    {
        private readonly Func<JToken, T> _mapper;

        public JsonHttpDeserializer(Func<JToken, T> mapper)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<T>> DeserializeAsync(HttpResponseMessage httpResponseMessage)
        {
            string json = await httpResponseMessage.Content.ReadAsStringAsync();

            return await Task.Run(() => JArray.Parse(json).Children().Select(t => _mapper(t)));
        }
    }
}
