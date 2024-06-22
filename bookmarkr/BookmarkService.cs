namespace bookmarkr;

public class BookmarkService
{
    private readonly List<Bookmark> _bookmarks = new();
    
    // Comment line 5 and uncomment this block if you want to test the "export" command. 
    // This will pre-populate the list of bookmarks so you don't have to ;)
    // private readonly List<Bookmark> _bookmarks = new List<Bookmark> {
    //     new Bookmark { Name = "Packt Publishing", Url = "https://packtpub.com/", Category = "Tech books" },
    //     new Bookmark { Name = "Audi cars", Url = "https://audi.ca", Category = "Read later" }
    // };

    public void AddLink(string name, string url, string category)
    {
        /**********************************************************************************/
        // NOTE: We don't need these validations anymore because the name and url options are required and enforced by System.CommandLine.
        // We can however keep these validations if the BookmarkService is meant to be used by another caller. This is not the case here so that's why I'm commenting them out.
        /**********************************************************************************/
        // if(string.IsNullOrWhiteSpace(name))
        // {
        //     Helper.ShowErrorMessage(["the 'name' for the link is not provided. The expected syntax is:", "bookmarkr link add <name> <url>"]);
        //     return;
        // }

        // if(string.IsNullOrWhiteSpace(url))
        // {
        //     Helper.ShowErrorMessage(["the 'url' for the link is not provided. The expected syntax is:", "bookmarkr link add <name> <url>"]);
        //     return;
        // }

        if(_bookmarks.Any(b => b.Name.ToLower().Equals(name.ToLower())))
        {
            Helper.ShowWarningMessage([$"A link with the name '{name}' already exists. It will thus not be added",
                                       $"To update the existing link, use the command: bookmarkr link update '{name}' '{url}'"]);
            return;
        }

        _bookmarks.Add(new Bookmark { Name = name, Url = url, Category = category});
        Helper.ShowSuccessMessage(["Bookmark successfully added!"]);
        Console.WriteLine(_bookmarks.Count);
    }


    public void AddLinks(string[] names, string[] urls, string[] categories)
    {
        for(int i = 0; i < names.Length; i++)
        {
            if(!_bookmarks.Any(b => b.Name.ToLower().Equals(names[i].ToLower())))
            {
                _bookmarks.Add(new Bookmark { Name = names[i], Url = urls[i], Category = categories[i] });
                Helper.ShowSuccessMessage(["Bookmark successfully added!"]);
                Console.WriteLine(_bookmarks.Count);
            }
        }
    }


    public void ListAll()
    {
        foreach(var bookmark in _bookmarks)
        {
            Console.WriteLine($"Name: '{bookmark.Name}' | URL: '{bookmark.Url}' | Category: '{bookmark.Category}'");            
        }
    }

    public List<Bookmark> GetAll()
    {
        return _bookmarks.ToList();
    }

    public void Import(List<Bookmark> bookmarks)
    {
        int count = 0;
        foreach(var bookmark in bookmarks)
        {
            _bookmarks.Add(bookmark);
            count++;
        }
        
        Helper.ShowSuccessMessage([$"Successfully imported {count} bookmarks!"]);
    }
}
