using System.Net.Http;
using System.Threading.Tasks;

namespace Opserver.Helpers
{
    public class SmsSender : ISmsSender
    {
        public async Task Instance(string baseUri, string requestUri, string body, string httpCreateClient, IHttpClientFactory httpClient)
        {
            var client = httpClient.CreateClient(httpCreateClient);

            client.BaseAddress = new System.Uri(baseUri);

            var req = await client.GetAsync($"{requestUri}={body}");

            if (req.IsSuccessStatusCode)
            {
                using var contentStream = await req.Content.ReadAsStreamAsync();
            }
        }
    }
}
