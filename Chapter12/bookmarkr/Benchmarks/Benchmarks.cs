using bookmarkr.Services;
using bookmarkr.ServiceAgents;
using BenchmarkDotNet.Attributes;
using bookmarkr.Commands;
using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;


namespace bookmarkr;

[MemoryDiagnoser]
public class Benchmarks
{

    #region Properties

    private IBookmarkService? _service;


    #endregion

    #region GlobalSetup

    [GlobalSetup]
    public void BenchmarksGlobalSetup()
    {
        _service = new BookmarkService();            
    }

    #endregion


    #region Benchmark cases

    [Benchmark(Baseline = true)]
    public async Task ExportBookmarks()
    {
        var exportCmd = new ExportCommand(_service!, "export", "Exports all bookmarks to a file");
        var exportArgs = new string[] { "--file", "bookmarksbench.json" };
        await exportCmd.InvokeAsync(exportArgs);        
    }

    [Benchmark]
    public async Task ExportBookmarksOptimized()
    {
        var exportCmd = new ExportCommandOptimized(_service!, "export", "Exports all bookmarks to a file");
        var exportArgs = new string[] { "--file", "bookmarksbench.json" };
        await exportCmd.InvokeAsync(exportArgs);        
    }
    #endregion
}