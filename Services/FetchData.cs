using System.Net.Http;
using System.Net;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace BattachApp.Services
{
    public class FetchData : IFetchData
    {
        public readonly IHttpClientFactory _client;
        public FetchData(IHttpClientFactory client)
        {
            _client = client;
        }
        async Task<string> IFetchData.fetch(Uri url)
        {
            JsonObject obj = new JsonObject();
            obj.Add("title", "Abdelkabir");
            obj.Add("body", "Battach from Morocco, Agadir");
            obj.Add("userId", "2");
            HttpClient client = _client.CreateClient();
            var requestMsg = new HttpRequestMessage(HttpMethod.Post, url);
            requestMsg.Content = new StringContent(obj.ToString(), null, "application/json");
            var request = await client.SendAsync(requestMsg);
            request.EnsureSuccessStatusCode();
            string resp = await request.Content.ReadAsStringAsync();
            //JsonObject jsonData = JsonConvert.DeserializeObject<JsonObject>(resp);
            return resp;
        }
    }
    public interface IFetchData
    {
        public Task<string> fetch(Uri url);
    }
}
