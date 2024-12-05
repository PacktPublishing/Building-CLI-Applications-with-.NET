


using System.Net;
using System.Text;
using System.Text.Json;

namespace bookmarkr.ServiceAgents;

public class BookmarkrSyncrServiceAgent : IBookmarkrSyncrServiceAgent
{
    private readonly IHttpClientFactory _clientFactory;

    public BookmarkrSyncrServiceAgent(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<List<Bookmark>> Sync(List<Bookmark> localBookmarks)
    {
        var serializedRetrievedBookmarks = JsonSerializer.Serialize(localBookmarks);
        var content = new StringContent(serializedRetrievedBookmarks, Encoding.UTF8, "application/json");

        var client = _clientFactory.CreateClient("bookmarkrSyncr");
        var response = await client.PostAsync("sync", content);

        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var mergedBookmarks = await JsonSerializer.DeserializeAsync<List<Bookmark>>(
                await response.Content.ReadAsStreamAsync(),
                options
            );

            return mergedBookmarks!;
        }
        else
        {
            switch(response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new HttpRequestException($"Resource not found: {response.StatusCode}");
                case HttpStatusCode.Unauthorized:
                    throw new HttpRequestException($"Unauthorized access: {response.StatusCode}");
                default:
                    var error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to sync bookmarks: {response.StatusCode} | {error}");
            }
        }
    }
}