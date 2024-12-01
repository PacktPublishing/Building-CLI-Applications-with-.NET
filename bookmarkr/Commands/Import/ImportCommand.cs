
using System.CommandLine;
using System.Text.Json;
using bookmarkr.Services;
using Serilog;
using System.IO.Abstractions;


namespace bookmarkr.Commands;

public class ImportCommand : Command
{

    #region Properties

    private readonly IBookmarkService _service;
    private readonly IFileSystem _fileSystem;

    #endregion

    #region Constructor

    public ImportCommand(IBookmarkService service, string name, string? description = null)
        : base(name, description)
    {
        _service = service;
        _fileSystem = new FileSystem();

        AddOption(inputfileOption);  
        this.SetHandler(OnImportCommand, inputfileOption);    
    }

    internal ImportCommand(IBookmarkService service, IFileSystem fileSystem, string name, string? description = null)
        : base(name, description)
    {
        _service = service;
        _fileSystem = fileSystem;

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

    public void OnImportCommand(FileInfo inputfile)
    {
        string json = _fileSystem.File.ReadAllText(inputfile.FullName);
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

    internal void OnImportCommand(IFileInfo inputfile)
    {
        OnImportCommand(new FileInfo(inputfile.FullName));
    }

    #endregion
}