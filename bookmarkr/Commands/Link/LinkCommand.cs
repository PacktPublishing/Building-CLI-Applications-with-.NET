
using System.CommandLine;
using bookmarkr.Services;


namespace bookmarkr.Commands;

public class LinkCommand : Command
{

    #region Properties

    private readonly IBookmarkService _service;

    #endregion

    #region Constructor

    public LinkCommand(IBookmarkService service, string name, string? description = null)
        : base(name, description)
    {
        _service = service;
        AddCommand(new LinkAddCommand(_service, "add", "Add a new bookmark link"));
    }

    #endregion


    #region Options    
    #endregion 

    #region Handler method
    #endregion
}