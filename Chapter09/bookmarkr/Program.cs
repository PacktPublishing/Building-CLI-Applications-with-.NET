﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.CommandLine.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Configuration;
using bookmarkr.Commands;
using bookmarkr.Services;
using Microsoft.Extensions.DependencyInjection;
using bookmarkr.ServiceAgents;


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
 * dotnet run -- interactive => runs the interactive version of the application.
 * dotnet run sync => calls the web service to sync the local bookmarks with the ones stored in the remote location. 
*/


class Program
{    
    static async Task<int> Main(string[] args)
    {
        FreeSerilogLoggerOnShutdown();

        /***** DECLARING VARIABLES *******************/
        IBookmarkService _service;
        IHttpClientFactory _clientFactory;
        IBookmarkrSyncrServiceAgent _serviceAgent;

        /***** INSTANTIATE THE ROOT COMMAND *******************/
        var rootCommand = new RootCommand("Bookmarkr is a bookmark manager provided as a CLI application.")
        {
        };

        rootCommand.SetHandler(OnHandleRootCommand);


        /***** CONFIGURE DEPENDENCY INJECTIONS *******************/
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Register your services here
                services.AddSingleton<IBookmarkService, BookmarkService>();
                services.AddScoped<IBookmarkrSyncrServiceAgent, BookmarkrSyncrServiceAgent>();

                services.AddHttpClient("bookmarkrSyncr", client =>
                {
                    client.BaseAddress = new Uri("https://bookmarkrsyncr-api.azurewebsites.net");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "Bookmarkr");
                });
            })
            .Build();

        _service = host.Services.GetRequiredService<IBookmarkService>();
        _serviceAgent = host.Services.GetRequiredService<IBookmarkrSyncrServiceAgent>();
        _clientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
        

        /***** REGISTER SUBCOMMANDS OF THE ROOT COMMAND *******************/
        rootCommand.AddCommand(new ExportCommand(_service, "export", "Exports all bookmarks to a file"));
        rootCommand.AddCommand(new ImportCommand(_service, "import", "Imports all bookmarks from a file"));
        rootCommand.AddCommand(new InteractiveCommand(_service, "interactive", "Manage bookmarks interactively"));
        rootCommand.AddCommand(new LinkCommand(_service, "link", "Manage bookmarks links"));
        rootCommand.AddCommand(new SyncCommand(_serviceAgent, _service, "sync", "sync local and remote bookmark stores"));
        
       
        /***** THE BUILDER PATTERN *******************/
        var parser = new CommandLineBuilder(rootCommand)
            .UseHost(_ => Host.CreateDefaultBuilder(), 
            host => 
            {
                host.ConfigureServices(services =>
                {
                    // ** configuration moved to appsettings.json
                    services.AddSerilog((config) =>
                    {
                        var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build();
                        config.ReadFrom.Configuration(configuration);
                    });
                });
            })
            .UseDefaults()
            .Build();    

        return await parser.InvokeAsync(args);
    }

    /***** HANDLER OF THE ROOT COMMAND *******************/
    static void OnHandleRootCommand()
    {
        Console.WriteLine("Hello from the root command!");
    }

    
    static void FreeSerilogLoggerOnShutdown()
    {
        // This event is raised when the process is about to exit, allowing you to perform cleanup tasks or save data.
        AppDomain.CurrentDomain.ProcessExit += (s, e) => ExecuteShutdownTasks();
        // This event is triggered when the user presses Ctrl+C or Ctrl+Break. While it doesn't cover all shutdown scenarios, it's useful for handling user-initiated terminations.
        Console.CancelKeyPress += (s, e) => ExecuteShutdownTasks();
    }


    // Code to execute before shutdown
    static void ExecuteShutdownTasks()
    {
        Console.WriteLine("Performing shutdown tasks...");
        // Perform cleanup tasks, save data, etc.
        Log.CloseAndFlush();
    }  
}