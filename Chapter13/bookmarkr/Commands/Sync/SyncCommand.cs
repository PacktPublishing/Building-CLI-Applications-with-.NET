using System.CommandLine;
using bookmarkr.Services;
using bookmarkr.ServiceAgents;
using Serilog;
using bookmarkr.Exceptions;
using Spectre.Console;


namespace bookmarkr.Commands;

public class SyncCommand : Command
{

    #region Properties

    private readonly IBookmarkService _service;
    private readonly IBookmarkrSyncrServiceAgent _serviceAgent;

    #endregion

    #region Constructor

    public SyncCommand(IBookmarkrSyncrServiceAgent serviceAgent, IBookmarkService service, string name, string? description = null)
        : base(name, description)
    {
        AddOption(patOption);
        _service = service;
        _serviceAgent = serviceAgent;
        this.SetHandler(OnSyncCommand, patOption);    
    }

    #endregion


    #region Options

    private Option<string> patOption = new Option<string>(
        ["--pat", "-p"],
        "The personal access token used to authenticate to BookmarkrSyncr"
    );

    #endregion


    #region Handler method

    private async Task OnSyncCommand(string patValue)
    {
        var retrievedBookmarks = _service.GetAll();
        try
        {
            var mergedBookmarks = await _serviceAgent.Sync(patValue, retrievedBookmarks);
            _service.ClearAll();
            _service.Import(mergedBookmarks!);

            Log.Information("Successfully synced bookmarks");
        }
        catch(PatNotFoundException ex)
        {
            Helper.ShowErrorMessage([$"The provided PAT value ({ex.Pat}) was not found."]);
        }
        catch(PatInvalidException ex)
        {
            Helper.ShowErrorMessage([$"The provided PAT value ({ex.Pat}) is invalid."]);
        }
        catch(PatExpiredException ex)
        {
            Helper.ShowErrorMessage([$"The provided PAT value ({ex.Pat}) is expired."]);
        }
        catch(HttpRequestException ex)
        {
            Log.Error(ex.Message);
        }                       
    }

    #endregion
}