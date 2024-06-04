using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;


namespace bookmarkr;


/*
 * The different possible executions:
 * dotnet run => "You haven't passed any argument..."
 * dotnet run hello => "Unknown Command"
 * dotnet run link hello => "Unsufficient number of parameters..."
 * dotnet run link add Packt https://www.packtpub.com => "Bookmark successfully added!"
  * dotnet run link add Packt https://www.packtpub.com => "Bookmark successfully added!"
    * Yeah, I know you were expecting to be told that the bookmark already exists. The reason why it doesn't happen is that, for now, there is no backing store for the list of bookmarks.
    * Hence, with every execution, the list is reinitialized to an empty one.
*/


class Program
{    

    private static BookmarkService service = new BookmarkService();

    static async Task<int> Main(string[] args)
    {

        /***** THE ROOT COMMAND *******************/
        var rootCommand = new RootCommand("Bookmarkr is a bookmark manager provided as a CLI application.")
        {
        };

        rootCommand.SetHandler(OnHandleRootCommand);


        /***** THE LINK COMMAND *******************/
        var linkCommand = new Command("link", "Manage bookmarks links")
        {
        };

        rootCommand.AddCommand(linkCommand);

        /***** THE ADD COMMAND *******************/
        var nameOption = new Option<string>(
            ["--name", "-n"], // equivalent to new string[] { "--name", "-n" }
            "The name of the bookmark"
        );

        var urlOption = new Option<string>(
            ["--url", "-u"],
            "The URL of the bookmark"
        );

        var addLinkCommand = new Command("add", "Add a new bookmark link")
        {
            nameOption,
            urlOption
        };

        linkCommand.AddCommand(addLinkCommand);

        addLinkCommand.SetHandler(OnHandleAddLinkCommand, nameOption, urlOption);

        /***** THE BUILDER PATTERN *******************/
        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .Build();

        return await parser.InvokeAsync(args);


        /***** HANDLER METHODS *******************/
        static void OnHandleRootCommand()
        {
            Console.WriteLine("Hello from the root command!");
        }

        static void OnHandleAddLinkCommand(string name, string url)
        {
            service.AddLink(name, url);
        }
    }    
}
