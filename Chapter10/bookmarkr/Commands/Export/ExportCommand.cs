
using System.CommandLine;
using System.Text.Json;
using bookmarkr.Services;


namespace bookmarkr.Commands;

public class ExportCommand : Command
{

    #region Properties

    private readonly IBookmarkService _service;

    #endregion

    #region Constructor

    public ExportCommand(IBookmarkService service, string name, string? description = null)
        : base(name, description)
    {
        _service = service;

        AddOption(outputfileOption);  
        this.SetHandler(async (context) =>
        {
            FileInfo? outputfileOptionValue = context.ParseResult.GetValueForOption(outputfileOption);
            var token = context.GetCancellationToken();
            await OnExportCommand(outputfileOptionValue!, token);
        });      
    }

    #endregion


    #region Options

    private Option<FileInfo> outputfileOption = new Option<FileInfo>(
        ["--file", "-f"],
        "The output file that will store the bookmarks"
    )
    {
        IsRequired = true
    }.LegalFileNamesOnly(); 

    #endregion 

    #region Handler method

    private async Task OnExportCommand(FileInfo outputfile, CancellationToken token)
    {
        try
        {
            Console.WriteLine("Starting export operation...");
            var bookmarks = _service.GetAll();
            string json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(outputfile.FullName, json, token);   
        }
        catch(OperationCanceledException ex)
        {
            var requested = ex.CancellationToken.IsCancellationRequested ? "Cancellation was requested by you." : "Cancellation was NOT requested by you.";
            Helper.ShowWarningMessage(["Operation was cancelled.", requested, $"Cancellation reason: {ex.Message}"]);
        }
        catch(JsonException ex)
        {
            Helper.ShowErrorMessage([$"Failed to serialize bookmarks to JSON.", 
                                        $"Error message {ex.Message}"]);   
        }
        catch (UnauthorizedAccessException ex)
        {
            Helper.ShowErrorMessage([$"Insufficient permissions to access the file {outputfile.FullName}", 
                                        $"Error message {ex.Message}"]);
        }
        catch (DirectoryNotFoundException ex)
        {
            Helper.ShowErrorMessage([$"The file {outputfile.FullName} cannot be found due to an invalid path", 
                                        $"Error message {ex.Message}"]);
        }
        catch (PathTooLongException ex)
        {
            Helper.ShowErrorMessage([$"The provided path is exceeding the maximum length.", 
                                        $"Error message {ex.Message}"]);
        }
        catch(Exception ex)
        {
            Helper.ShowErrorMessage([$"An unknown exception occurred.", 
                                        $"Error message {ex.Message}"]);
        }            
    }

    #endregion
}