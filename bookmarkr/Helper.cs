namespace bookmarkr;


static class Helper
{
    public static void ShowErrorMessage(string[] errorMessages)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        foreach(var message in errorMessages)
        {
            Console.WriteLine(message);
        }
        Console.ForegroundColor = color; 
    }

    public static void ShowWarningMessage(string[] errorMessages)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        foreach(var message in errorMessages)
        {
            Console.WriteLine(message);
        }
        Console.ForegroundColor = color; 
    }

    public static void ShowSuccessMessage(string[] errorMessages)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        foreach(var message in errorMessages)
        {
            Console.WriteLine(message);
        }
        Console.ForegroundColor = color; 
    }
}