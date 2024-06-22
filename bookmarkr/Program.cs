using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Completions;
using System.CommandLine.Parsing;
using System.Text.Json;


namespace bookmarkr;


/*
 * The different possible executions:
 * dotnet run => "Hello from the root command!"
 * dotnet run hello => "Unrecognized command or argument 'hello'."
 * dotnet run -- --version => 2.0.0
 * dotnet run -- --help | dotnet run -- -h | dotnet run -- -? => help menu for the CLI application.
 * dotnet run -- link --help | dotnet run -- link -h | dotnet run -- link -? => help menu for the link command.
  * dotnet run -- link add --help | dotnet run -- link add -h | dotnet run -- link add -? => help menu for the link command.
 * dotnet run link add --name 'Packt Publishing' --url 'https://packtpub.com/' --category 'Tech books' => "Bookmark successfully added!"
   * dotnet run link add -n 'Packt Publishing' -u 'https://www.packtpub.com' -c 'Tech books' => "Bookmark successfully added!"
 * dotnet run link add --name 'Packt Publishing' --url 'https://packtpub.com/' --name 'A great tech book publisher' => the second name will override the first name.
 * dotnet run link add --name 'Packt Publishing' --url 'https://packtpub.com/' --category 'Tech books' --name 'Audi cars' --url 'https://audi.ca' --category 'Read later' => adding two bookmarks with a single CLI request.
   * dotnet run link add --name 'Packt Publishing' 'Audi cars' --url 'https://packtpub.com/' 'https://audi.ca' --category 'Tech books' 'Read later' => an equivalent syntax.
 * dotnet run export --file 'bookmarks.json' => exports all the bookmarks held by the application into the specified output JSON file.
 * dotnet run import --file 'bookmarks.json' => imports all the bookmarks found in the input JSON file into the application.
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
        var nameOption = new Option<string[]>(
            ["--name", "-n"], // equivalent to new string[] { "--name", "-n" }
            "The name of the bookmark"
        );
        nameOption.IsRequired = true;
        nameOption.Arity = ArgumentArity.OneOrMore;
        nameOption.AllowMultipleArgumentsPerToken = true;


        var urlOption = new Option<string[]>(
            ["--url", "-u"],
            "The URL of the bookmark"
        );
        urlOption.IsRequired = true;
        urlOption.Arity = ArgumentArity.OneOrMore;
        urlOption.AllowMultipleArgumentsPerToken = true;

        // this is valid only if your option accepts a single value (e.g., string), not a list (e.g., string[]).
        // however, the uncommented version works in both cases, so I'm leaving this version commented out for reference only ;)
        // urlOption.AddValidator(result =>
        // {
        //     if (result.Tokens.Count == 0)
        //     {
        //         result.ErrorMessage = "The URL is required";
        //     }
        //     else if (!Uri.TryCreate(result.Tokens[0].Value, UriKind.Absolute, out _))
        //     {
        //         result.ErrorMessage = "The URL is invalid";
        //     }
        // });

        urlOption.AddValidator(result =>
        {
            foreach (var token in result.Tokens)
            {
                if (string.IsNullOrWhiteSpace(token.Value))
                {
                    result.ErrorMessage = "URL cannot be empty";
                    break;
                }
                else if (!Uri.TryCreate(token.Value, UriKind.Absolute, out _))
                {
                    result.ErrorMessage = $"Invalid URL: {token.Value}";
                    break;
                }
            }
        });


        var categoryOption = new Option<string[]>(
            ["--category", "-c"],
            "The category to which the bookmark is associated"
        );

        categoryOption.Arity = ArgumentArity.OneOrMore;
        categoryOption.AllowMultipleArgumentsPerToken = true;

        categoryOption.SetDefaultValue("Read later");
        categoryOption.FromAmong("Read later", "Tech books", "Cooking", "Social media");
        categoryOption.AddCompletions("Read later", "Tech books", "Cooking", "Social media");
        
        // Note: this is another approach to defining completions. It is useful for dynamic values (example, a range of dates for the next two weeks, starting today)
        // categoryOption.AddCompletions((ctx) =>
        // {
        //     var list = new List<CompletionItem>
        //     {
        //         new CompletionItem("Read later"),
        //         new CompletionItem("Tech books"),
        //         new CompletionItem("Cooking"),
        //         new CompletionItem("Social media")
        //     };
        //     return list;
        // });
        

        var addLinkCommand = new Command("add", "Add a new bookmark link")
        {
            nameOption,
            urlOption,
            categoryOption
        };

        linkCommand.AddCommand(addLinkCommand);

        addLinkCommand.SetHandler(OnHandleAddLinkCommand, nameOption, urlOption, categoryOption);


        /****** the export command *****/
        var outputfileOption = new Option<FileInfo>(
            ["--file", "-f"],
            "The output file that will store the bookmarks"
        )
        {
            IsRequired = true
        };

        outputfileOption.LegalFileNamesOnly();

        var exportCommand = new Command("export", "Exports all bookmarks to a file")
        {
            outputfileOption
        };

        rootCommand.AddCommand(exportCommand);

        exportCommand.SetHandler(OnExportCommand, outputfileOption);


        /****** the import command *****/
        var inputfileOption = new Option<FileInfo>(
            ["--file", "-f"],
            "The input file that contains the bookmarks to be imported"
        )
        {
            IsRequired = true
        };

        inputfileOption.LegalFileNamesOnly();
        inputfileOption.ExistingOnly();

        var importCommand = new Command("import", "Imports all bookmarks from a file")
        {
            inputfileOption
        };

        rootCommand.AddCommand(importCommand);

        importCommand.SetHandler(OnImportCommand, inputfileOption);

        
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

        //static void OnHandleAddLinkCommand(string[] name, string[] url, string category)
        static void OnHandleAddLinkCommand(string[] names, string[] urls, string[] categories)
        {
            //service.AddLink(name, url, category.ToString());
            service.AddLinks(names, urls, categories);
            service.ListAll();
        }

        static void OnExportCommand(FileInfo outputfile)
        {
            var bookmarks = service.GetAll();
            string json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputfile.FullName, json);
        }

        static void OnImportCommand(FileInfo inputfile)
        {
            string json = File.ReadAllText(inputfile.FullName);
            List<Bookmark> bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(json) ?? new List<Bookmark>();
            service.Import(bookmarks);
        }
    }    
}