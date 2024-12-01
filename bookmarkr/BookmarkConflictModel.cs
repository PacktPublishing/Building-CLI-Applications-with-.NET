
using System.Diagnostics.CodeAnalysis;

namespace bookmarkr;


[ExcludeFromCodeCoverage(Justification ="model class. No processing is performed by this class.")]
public class BookmarkConflictModel
{
    public string? OldName { get; set; }
    public string? NewName { get; set; }
    public string? Url { get; set; }
}