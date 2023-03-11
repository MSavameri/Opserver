using System.Net.Http;
using System.Threading.Tasks;

namespace Opserver.Helpers
{
    public interface ISmsSender
    {
        Task Instance(string baseUri, string requestUri, string body, string httpCreateClient, IHttpClientFactory httpClient);
    }
}