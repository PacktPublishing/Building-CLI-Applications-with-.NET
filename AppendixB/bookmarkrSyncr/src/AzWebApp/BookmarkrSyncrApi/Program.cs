using System.Text.Json;
using System.Net.Http;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITokenValidator, TokenValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/hello", () => {
    return Results.Ok("Hello from BookmarkrSyncr API !");
});

app.MapPost("/sync", async ([FromHeader(Name = "X-PAT")] string pat, List<Bookmark> bookmarks, ITokenValidator tokenValidator, HttpContext context) =>
{
    // Ensure the Personal Access Token (PAT) is valid
    if (!tokenValidator.IsValid(pat))
    {
        context!.Response.Headers["X-Invalid-PAT"] = pat;
        return Results.Unauthorized();     
    }

    // Ensure the Personal Access Token (PAT) is not expired
    if (tokenValidator.IsExpired(pat))
    {
        context!.Response.Headers["X-Expired-PAT"] = pat;
        return Results.Unauthorized();     
    }

    // Read the incoming bookmarks from the request body
    var incomingBookmarks = bookmarks;

    // Read existing bookmarks from the blob
    List<Bookmark> existingBookmarks = new List<Bookmark>();
    var client = new HttpClient();
    var json = await client.GetStringAsync("https://bookmarkrdatastore.blob.core.windows.net/bookmarks/bookmarks.json");
    
    existingBookmarks = JsonSerializer.Deserialize<List<Bookmark>>(json)!;

    // Merge the lists
    var mergedBookmarks = existingBookmarks
        .GroupJoin(incomingBookmarks!,
            existing => existing.Url,
            incoming => incoming.Url,
            (existing, incoming) => incoming.FirstOrDefault() ?? existing)
        .Union(incomingBookmarks!.Where(incoming => !existingBookmarks.Any(existing => existing.Url == incoming.Url)))
        .ToList();

    // Write the merged list back to the blob
    await WriteToBlob(JsonSerializer.Serialize(mergedBookmarks));

    // Return the merged list as the response
    return Results.Ok(mergedBookmarks);        
})
.WithName("Sync")
.WithOpenApi();

app.Run();

async Task WriteToBlob(string json)
{
    string connectionString = "<Your_Azure_Storage_Connection_String>";
    string containerName = "bookmarks";
    string blobName = "bookmarks.json";

    var blobServiceClient = new BlobServiceClient(connectionString);
    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

    // Ensure the container exists
    await containerClient.CreateIfNotExistsAsync();

    var blobClient = containerClient.GetBlobClient(blobName);

    using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
    {
        await blobClient.UploadAsync(stream, overwrite: true);
    }
}