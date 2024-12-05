using System.Diagnostics.CodeAnalysis;

namespace bookmarkr;

[ExcludeFromCodeCoverage(Justification ="model class. No processing is performed by this class.")]

public class Bookmark
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required string Category { get; set; }
}
