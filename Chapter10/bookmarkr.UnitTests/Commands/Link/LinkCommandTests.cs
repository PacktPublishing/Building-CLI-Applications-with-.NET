using bookmarkr.Commands;
using bookmarkr.Services;
using NSubstitute;


namespace bookmarkr.Tests;

[TestClass]
public class LinkCommandTests
{
    [TestMethod]
    public void LinkCommand_CallingClassConstuctor_EnsuresThatLinkAddCommandIsTheOnlySubCommandOfLinkCommand()
    {
        // Arrange
        IBookmarkService service = Substitute.For<IBookmarkService>();
        var expectedSubCommand = new LinkAddCommand(service, "add", "Add a new bookmark link");
        
        // Act
        var actualCommand = new LinkCommand(service, "link", "Manage bookmarks links");
        var actualSubCommand = actualCommand.Subcommands[0];

        // Assert
        Assert.AreEqual(1, actualCommand.Subcommands.Count);
        CollectionAssert.AreEqual(actualSubCommand.Aliases.ToList(), expectedSubCommand.Aliases.ToList());
        Assert.AreEqual(actualSubCommand.Description, expectedSubCommand.Description);        
    }
}