
using System.CommandLine;
using System.Text.Json;
using bookmarkr.Services;
using Spectre.Console;


namespace bookmarkr.Commands;

public class InteractiveCommand : Command
{

    #region Properties

    private readonly IBookmarkService _service;

    #endregion

    #region Constructor

    public InteractiveCommand(IBookmarkService service, string name, string? description = null)
        : base(name, description)
    {
        _service = service;
        this.SetHandler(OnInteractiveCommand);    
    }

    #endregion


    #region Options
    #endregion 

    #region Handler method

    private void OnInteractiveCommand()
    {
        bool isRunning = true;
        while(isRunning)
        {
            AnsiConsole.Write(new FigletText("Bookmarkr").Centered().Color(Color.SteelBlue));

            var selectedOperation = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]What do you wanna do?[/]")
                    .AddChoices([
                        "Export bookmarks to file",
                        "View Bookmarks",
                        "Exit Program"
                    ])
            );

            switch(selectedOperation)
            {
                case "Export bookmarks to file": 
                    ExportBookmarks();
                    break;
                case "View Bookmarks":
                    ViewBookmarks(); 
                    break;
                default: 
                    isRunning = false;
                    break;
            }
        }            
    } 

    private void ExportBookmarks()
    {  
        // ask for the outputfilePath
        var outputfilePath = AnsiConsole.Prompt(
                new TextPrompt<string>("Please provide the output file name (default: 'bookmarks.json')")
                .DefaultValue("bookmarks.json"));

        // export the bookmarks to the specified file, while showing progress.
        AnsiConsole.Progress()
            .AutoRefresh(true) // Turns on auto refresh
            .AutoClear(false)   // Avoids removing the task list when completed
            .HideCompleted(false)   // Avoids hiding tasks as they are completed
            .Columns(
            [
                new TaskDescriptionColumn(),    // Shows the task description
                new ProgressBarColumn(),        // Shows the progress bar
                new PercentageColumn(),         // Shows the current percentage
                new RemainingTimeColumn(),      // Shows the remaining time
                new SpinnerColumn(),            // Shows the spinner, indicating that the operation is ongoing
            ])
            .Start(ctx =>
            {
                // Get the list of all bookmarks
                var bookmarks = _service.GetAll();

                // export the bookmarks to the file
                // 1. Create the task
                var task = ctx.AddTask("[yellow]exporting all bookmarks to file...[/]");

                // 2. Set the total steps for the progress bar
                task.MaxValue = bookmarks.Count;
                
                // 3. Open the file for writing
                using (StreamWriter writer = new StreamWriter(outputfilePath))
                {
                    while (!ctx.IsFinished)
                    {
                        foreach (var bookmark in bookmarks)
                        {
                            // 3.1. Serialize the current bookmark as JSON and write it to the file asynchronously
                            writer.WriteLine(JsonSerializer.Serialize(bookmark));

                            // 3.2. Increment the progress bar
                            task.Increment(1);

                            // 3.3. Slow down the process so we can see the progress (since this operation is not that much time-consuming)
                            Thread.Sleep(1500);
                        }
                    }
                }
            });
        AnsiConsole.MarkupLine("[green]All bookmarks have been successfully exported![/]");        
    }

    private void ViewBookmarks()
    {
        // Create the tree
        var root = new Tree("Bookmarks");

        // Add some nodes
        var techBooksCategory = root.AddNode("[yellow]Tech Books[/]");
        var carsCategory = root.AddNode("[yellow]Cars[/]");
        var socialMediaCategory = root.AddNode("[yellow]Social Media[/]");
        var cookingCategory = root.AddNode("[yellow]Cooking[/]");

        // add bookmarks for the Tech Book category
        var techBooks = _service.GetBookmarksByCategory("Tech Books");
        foreach(var techbook in techBooks)
        {
            techBooksCategory.AddNode($"{techbook.Name} | {techbook.Url}");
        }

        // add bookmarks for the Cars category
        var cars = _service.GetBookmarksByCategory("Cars");
        foreach(var car in cars)
        {
            carsCategory.AddNode($"{car.Name} | {car.Url}");
        }

        // add bookmarks for the Social Media category
        var socialMedias = _service.GetBookmarksByCategory("Social Media");
        foreach(var socialMedia in socialMedias)
        {
            socialMediaCategory.AddNode($"{socialMedia.Name} | {socialMedia.Url}");
        }

        // add bookmarks for the Cooking category
        var cookings = _service.GetBookmarksByCategory("Cooking");
        foreach(var cooking in cookings)
        {
            cookingCategory.AddNode($"{cooking.Name} | {cooking.Url}");
        }

        // Render the tree
        AnsiConsole.Write(root);        
    }

    #endregion
}