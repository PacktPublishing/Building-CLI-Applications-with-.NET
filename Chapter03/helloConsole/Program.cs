namespace helloConsole;
using System.Threading.Tasks;

/*
    HOW TO USE THIS PROGRAM
    ---

    * Comment / uncomment the methods called in the "Main" method to execute them.
    * Above each method, you have an example of how to call the program to execute this method. 
*/

class Program
{
    static void Main(string[] args)
    {
        /* dotnet run */
        HelloWorld();
        
        /* dotnet run Packt */
        // PassingOneParameter(args);

        /* dotnet run Packt Publishing */
        // PassingMoreThanOneParameter(args);

        /* dotnet run 42 */
        // ParsingInputParameters(args);

        /* dotnet run Packt Publishing */
        // SwitchingInputParameters(args);

        /* dotnet run */
        // MissingInputParameter(args);
        
        /* dotnet run */
        // ConsoleProperties();

        /* dotnet run */
        // ReadLine();

        /* dotnet run */
        // ReadKeyTrue();

        /* dotnet run */
        // ReadKeyFalse();

        /* dotnet run */
        // Clear();

        /* dotnet run */
        // CancelKeyPress();
   }


    #region helper methods

    private static void HelloWorld()
    {
        Console.WriteLine("Hello, World!");
    }

    private static void PassingOneParameter(string[] args)
    {
        Console.WriteLine($"Hello, {args[0]}!");
    }

    private static void PassingMoreThanOneParameter(string[] args)
    {
        Console.WriteLine($"Hello, {args[0]} {args[1]}!");
    }

    private static void ParsingInputParameters(string[] args)
    {
        var value = args[0];
        Console.WriteLine($"The input value {value} is of type {value.GetType()}");
        var parsedValue = int.Parse(value);
        Console.WriteLine($"The parsed value {parsedValue} is of type {parsedValue.GetType()}");
    }

    private static void SwitchingInputParameters(string[] args)
    {
        Console.WriteLine($"Before switching => {args[0]} {args[1]}");

        Console.WriteLine($"After switching => {args[1]} {args[0]}");
    }

    private static void MissingInputParameter(string[] args)
    {
        Console.WriteLine($"Hello, {args[0]}!");
    }

    private static void ConsoleProperties()
    {
        // performing a backup of the background and foreground colors
            var originalBackroungColor = Console.BackgroundColor;
            var originalForegroundColor = Console.ForegroundColor;

            // changing the background and foreground colors
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;

            // setting the title of the terminal while the application is running 
            Console.Title = "Packt Publishing Console App";

            // displaying a message
            Console.WriteLine($"Hello from Packt Publishing!");

            // restoring the background and foreground colors to their original values
            Console.BackgroundColor = originalBackroungColor;
            Console.ForegroundColor = originalForegroundColor;

            Console.WriteLine("Press any key to exit...");
            // waiting for the user to press a key to end the program.
            // this is useful to see the altering of the terminal's title
            Console.ReadKey(true);
    } 

    private static void ReadLine()
    {
        Console.WriteLine("enter some text then hit ENTER, or simply hit ENTER to end the program");
        
        string? line;
        while((line = Console.ReadLine()) != "")
        {
            Console.WriteLine(line);
        }

        // if we reach this point, it means that the user has hit ENTER without entering any text.
        Console.WriteLine("bye!");
    }

    private static void ReadKeyTrue()
    {
        Console.WriteLine("Press any key or ESC to exit...");

        var keyPressed = Console.ReadKey(true).Key;

        while(keyPressed != ConsoleKey.Escape)
        {
            Console.WriteLine($"you pressed {keyPressed}");
            keyPressed = Console.ReadKey(true).Key;
        }
    }

    private static void ReadKeyFalse()
    {
        Console.WriteLine("Press any key or ESC to exit...");

        var keyPressed = Console.ReadKey(false).Key;

        while(keyPressed != ConsoleKey.Escape)
        {
            Console.WriteLine($"you pressed {keyPressed}");
            keyPressed = Console.ReadKey(false).Key;
        }
    }

    private static void Clear()
    {
         var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        Console.WriteLine(lorem);
        Console.WriteLine();

        Console.WriteLine("press C to clear the screen before exiting the method, or any other key to exit without clearing the screen...");
        if(Console.ReadKey(true).Key == ConsoleKey.C)
        {
            Console.Clear();
        }   
    }

    private static void CancelKeyPress()
    {
         Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // This will prevent the program from terminating immediately
            Console.WriteLine("CancelKeyPress event raised!\nPerforming cleanup...");
            // Performing cleanup operations (logging out of services, saving progress state, closing database connections,...)
            Environment.Exit(0); // This will terminate the program when cleanup is done
        };

        int counter = 1;
        while(true)
        {
            Console.WriteLine($"Printing line number {counter}");
            counter++;
            Task delayTask = Task.Run(async () => await Task.Delay(1000));
            delayTask.Wait();
        }
    }

    #endregion
}