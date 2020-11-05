using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maze.DTO
{
    public interface IHttpDeserializer<T>
    {
        Task<IEnumerable<T>> DeserializeAsync(HttpResponseMessage httpResponseMessage);
    }
}
