using Spectre.Console;

namespace bookmarkr;


static class Helper
{

    #region Wit Spectre.Console

    public static void ShowErrorMessage(string[] errorMessages)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        AnsiConsole.MarkupLine(Emoji.Known.CrossMark + " [bold red]ERROR[/] :cross_mark:");
        foreach(var message in errorMessages)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{message}[/]");
        }
    }


    public static void ShowWarningMessage(string[] errorMessages) 
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var m = new Markup(Emoji.Known.Warning + " [bold yellow]Warning[/] :warning:");
        m.Centered();
        AnsiConsole.Write(m);
        AnsiConsole.WriteLine();
        foreach(var message in errorMessages)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]{message}[/]");
        }
    }


    public static void ShowSuccessMessage(string[] errorMessages) 
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        AnsiConsole.MarkupLine(Emoji.Known.BeatingHeart + " [bold green]SUCCESS[/] :beating_heart:");
        foreach(var message in errorMessages)
        {
            AnsiConsole.MarkupLineInterpolated($"[green]{message}[/]");
        }
    }

    #endregion

    #region Without Spectre.Console

    public static void ShowErrorMessage2(string[] errorMessages)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        foreach(var message in errorMessages)
        {
            Console.WriteLine(message);
        }
        Console.ForegroundColor = color; 
    }

    public static void ShowWarningMessage2(string[] errorMessages)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        foreach(var message in errorMessages)
        {
            Console.WriteLine(message);
        }
        Console.ForegroundColor = color; 
    }

    public static void ShowSuccessMessage2(string[] errorMessages)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        foreach(var message in errorMessages)
        {
            Console.WriteLine(message);
        }
        Console.ForegroundColor = color; 
    }
    
    #endregion
}