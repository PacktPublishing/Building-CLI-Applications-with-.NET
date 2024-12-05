using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Storage.Blob;

public static class BookmarkMerger
{
    public class Bookmark
    {
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required string Category { get; set; }
    }

    [Function("BookmarkMerger")]
    [BlobOutput("bookmarks/bookmarks.json")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        //[Blob("bookmarks/bookmarks.json", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlockBlob blob,
        [BlobInput("bookmarks/bookmarks.json")] CloudBlockBlob blob,
        ILogger log)
    {
        log.LogInformation("BookmarkrSyncr | processing request...");

        // Read the incoming bookmarks from the request body
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var incomingBookmarks = JsonConvert.DeserializeObject<List<Bookmark>>(requestBody);

        // Read existing bookmarks from the blob
        List<Bookmark> existingBookmarks = new List<Bookmark>();
        if (await blob.ExistsAsync())
        {
            string blobContent = await blob.DownloadTextAsync();
            existingBookmarks = JsonConvert.DeserializeObject<List<Bookmark>>(blobContent)!;
        }

        // Merge the lists
        var mergedBookmarks = existingBookmarks
            .GroupJoin(incomingBookmarks!,
                existing => existing.Url,
                incoming => incoming.Url,
                (existing, incoming) => incoming.FirstOrDefault() ?? existing)
            .Union(incomingBookmarks!.Where(incoming => !existingBookmarks.Any(existing => existing.Url == incoming.Url)))
            .ToList();

        // Write the merged list back to the blob
        await blob.UploadTextAsync(JsonConvert.SerializeObject(mergedBookmarks));

        // Return the merged list as the response
        return new OkObjectResult(mergedBookmarks);
    }
}