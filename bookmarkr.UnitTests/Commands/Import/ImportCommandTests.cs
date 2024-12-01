using bookmarkr.Commands;
using bookmarkr.Services;
using NSubstitute;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace bookmarkr.Tests;

[TestClass]
public class ImportCommandTests
{
    public required IBookmarkService _bookmarkService;
    public required MockFileSystem _mockFileSystem;

    [TestInitialize]
    public void TestInitialize()
    {
        string bookmarksAsJson = @"[
            {
                ""Name"": ""Packt Publishing"",
                ""Url"": ""https://packtpub.com/"",
                ""Category"": ""Tech Books""
            },
            {
                ""Name"": ""Audi cars"",
                ""Url"": ""https://audi.ca"",
                ""Category"": ""See later""
            },
            {
                ""Name"": ""LinkedIn"",
                ""Url"": ""https://www.linkedin.com/"",
                ""Category"": ""Social Media""
            }
        ]"; 

        _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> 
        {
            {@"bookmarks.json", new MockFileData(bookmarksAsJson)}
        });
    }

    [TestMethod]
    public void OnImportCommand_PassingAValidAndExistingFile_CallsImportMethodOnBookmarkService()
    {
        // Arrange
        _bookmarkService = Substitute.For<IBookmarkService>();
       
        var command = new ImportCommand(_bookmarkService, _mockFileSystem, "import", "Imports all bookmarks from a file");

        // Act
        command.OnImportCommand(_mockFileSystem.FileInfo.New("bookmarks.json"));

        // Assert
        _bookmarkService.Received(3).Import(Arg.Any<Bookmark>());
        _bookmarkService.Received(1).Import(Arg.Is<Bookmark>(b => b.Name == "Packt Publishing" && b.Url == "https://packtpub.com/" && b.Category == "Tech Books"));
        _bookmarkService.Received(1).Import(Arg.Is<Bookmark>(b => b.Name == "Audi cars" && b.Url == "https://audi.ca" && b.Category == "See later"));
        _bookmarkService.Received(1).Import(Arg.Is<Bookmark>(b => b.Name == "LinkedIn" && b.Url == "https://www.linkedin.com/" && b.Category == "Social Media"));
    }

    [TestMethod]
    public void ImportCommand_Conflict_TheNameOfTheConflictingBookmarkIsUpdated()
    {
        // Arrange
        _bookmarkService = new BookmarkService();
        _bookmarkService.ClearAll();
        _bookmarkService.AddLink("Audi Canada", "https://audi.ca", "See later");
        
        var command = new ImportCommand(_bookmarkService, _mockFileSystem, "import", "Imports all bookmarks from a file");

        // Act
        command.OnImportCommand(_mockFileSystem.FileInfo.New("bookmarks.json"));
        var currentBookmarks = _bookmarkService.GetAll();

        // Assert
        Assert.AreEqual(3, currentBookmarks.Count);
        Assert.IsTrue(currentBookmarks.Exists(b => b.Name == "Packt Publishing" && b.Url == "https://packtpub.com/" && b.Category == "Tech Books"));
        Assert.IsTrue(currentBookmarks.Exists(b => b.Name == "Audi cars" && b.Url == "https://audi.ca" && b.Category == "See later"));
        Assert.IsTrue(currentBookmarks.Exists(b => b.Name == "LinkedIn" && b.Url == "https://www.linkedin.com/" && b.Category == "Social Media"));   
        Assert.IsFalse(currentBookmarks.Exists(b => b.Name == "Audi Canada" && b.Url == "https://audi.ca" && b.Category == "See later"));   
    }
}