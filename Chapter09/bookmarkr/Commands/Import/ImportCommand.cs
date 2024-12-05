
using System.CommandLine;
using System.Text.Json;
using bookmarkr.Services;
using Serilog;


namespace bookmarkr.Commands;

public class ImportCommand : Command
{

    #region Properties

    private readonly IBookmarkService _service;

    #endregion

    #region Constructor

    public ImportCommand(IBookmarkService service, string name, string? description = null)
        : base(name, description)
    {
        _service = service;

        AddOption(inputfileOption);  
        this.SetHandler(OnImportCommand, inputfileOption);    
    }

    #endregion


    #region Options

    private Option<FileInfo> inputfileOption = new Option<FileInfo>(
        ["--file", "-f"],
        "The input file that contains the bookmarks to be imported"
    )
    {
        IsRequired = true
    }
    .LegalFileNamesOnly()
    .ExistingOnly();

    #endregion 

    #region Handler method

    private void OnImportCommand(FileInfo inputfile)
    {
        string json = File.ReadAllText(inputfile.FullName);
        List<Bookmark> bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(json) ?? new List<Bookmark>();
        
        foreach(var bookmark in bookmarks)
        {
            var conflict = _service.Import(bookmark);
            if (conflict is not null)
            {
                Log.Information($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | Bookmark updated | name changed from '{conflict.OldName}' to '{conflict.NewName}' for URL '{conflict.Url}'");
            }
        }
    }

    #endregion
}