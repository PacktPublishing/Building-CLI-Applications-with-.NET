using System.CommandLine;
using bookmarkr.Services;
using bookmarkr.ServiceAgents;
using Serilog;


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
        _service = service;
        _serviceAgent = serviceAgent;
        this.SetHandler(OnSyncCommand);    
    }

    #endregion


    #region Options
    #endregion


    #region Handler method

    private async Task OnSyncCommand()
    {
        var retrievedBookmarks = _service.GetAll();
        try
        {
            var mergedBookmarks = await _serviceAgent.Sync(retrievedBookmarks);
            _service.ClearAll();
            _service.Import(mergedBookmarks!);

            Log.Information("Successfully synced bookmarks");
        }
        catch(HttpRequestException ex)
        {
            Log.Error(ex.Message);
        }                       
    }

    #endregion
}