


using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Schema;
using bookmarkr.Exceptions;

namespace bookmarkr.ServiceAgents;

public class BookmarkrSyncrServiceAgent : IBookmarkrSyncrServiceAgent
{
    private readonly IHttpClientFactory _clientFactory;

    public BookmarkrSyncrServiceAgent(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<List<Bookmark>> Sync(string pat, List<Bookmark> localBookmarks)
    {
        // ensure that the pat is present
        if(string.IsNullOrWhiteSpace(pat))
        {
            string? value = Environment.GetEnvironmentVariable("BOOKMARKR_PAT");
            if(value == null) throw new PatNotFoundException(pat);
            pat = value;           
        }

        var serializedRetrievedBookmarks = JsonSerializer.Serialize(localBookmarks);
        var content = new StringContent(serializedRetrievedBookmarks, Encoding.UTF8, "application/json");

        var client = _clientFactory.CreateClient("bookmarkrSyncr");
        // Add the PAT to the request header
        client.DefaultRequestHeaders.Add("X-PAT", pat);
        var response = await client.PostAsync("sync", content);

        if (response.IsSuccessStatusCode)
        {
            // saving the PAT to the environment variable, if not already
            string? value = Environment.GetEnvironmentVariable("BOOKMARKR_PAT");
            if(value == null || !value.Equals(pat)) Environment.SetEnvironmentVariable("BOOKMARKR_PAT", pat); 

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
                    if (response.Headers.TryGetValues("X-Invalid-PAT", out var headerValues))
                        throw new PatInvalidException(pat);
                    if (response.Headers.TryGetValues("X-Expired-PAT", out var headerValues2))
                        throw new PatExpiredException(pat);
                    throw new HttpRequestException($"Unauthorized access: {response.StatusCode}");
                default:
                    var error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to sync bookmarks: {response.StatusCode} | {error}");
            }
        }
    }
}