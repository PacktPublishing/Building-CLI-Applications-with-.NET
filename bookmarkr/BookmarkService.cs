namespace bookmarkr;

public class BookmarkService
{
    private readonly List<Bookmark> _bookmarks = new();


    public void AddLink(string name, string url)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            Helper.ShowErrorMessage(["the 'name' for the link is not provided. The expected syntax is:", "bookmarkr link add <name> <url>"]); 
            return;
        }
        
        if(string.IsNullOrWhiteSpace(url))
        {
            Helper.ShowErrorMessage(["the 'url' for the link is not provided. The expected syntax is:", "bookmarkr link add <name> <url>"]); 
            return;
        }

        if(_bookmarks.Any(b => b.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            Helper.ShowWarningMessage([$"A link with the name '{name}' already exists. It will thus not be added", 
                                       $"To update the existing link, use the command: bookmarkr link update '{name}' '{url}'"]);
            return;
        }

        _bookmarks.Add(new Bookmark { Name = name, Url = url});
        Helper.ShowSuccessMessage(["Bookmark successfully added!"]);
        Console.WriteLine(_bookmarks.Count);
    }
}
