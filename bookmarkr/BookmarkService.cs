namespace bookmarkr;

public class BookmarkService
{
    //private readonly List<Bookmark> _bookmarks = new();
    
    // Comment line 5 and uncomment this block if you want to test the "export" command. 
    // This will pre-populate the list of bookmarks so you don't have to ;)
    private readonly List<Bookmark> _bookmarks = new List<Bookmark> {
        new Bookmark { Name = "Packt Publishing", Url = "https://packtpub.com/", Category = "Tech Books" },
        new Bookmark { Name = "Audi cars", Url = "https://audi.ca", Category = "Cars" },
        new Bookmark { Name = "O'Reilly Media", Url = "https://www.oreilly.com/", Category = "Tech Books" },
        new Bookmark { Name = "Tesla", Url = "https://www.tesla.com/", Category = "Cars" },
        new Bookmark { Name = "Allrecipes", Url = "https://www.allrecipes.com/", Category = "Cooking" },
        new Bookmark { Name = "Twitter", Url = "https://twitter.com/", Category = "Social Media" },
        new Bookmark { Name = "Manning Publications", Url = "https://www.manning.com/", Category = "Tech Books" },
        new Bookmark { Name = "BMW", Url = "https://www.bmw.com/", Category = "Cars" },
        new Bookmark { Name = "Food Network", Url = "https://www.foodnetwork.com/", Category = "Cooking" },
        new Bookmark { Name = "Facebook", Url = "https://www.facebook.com/", Category = "Social Media" },
        new Bookmark { Name = "APress", Url = "https://apress.com/", Category = "Tech Books" },
        new Bookmark { Name = "LinkedIn", Url = "https://www.linkedin.com/", Category = "Social Media" },
        new Bookmark { Name = "Mercedes-Benz", Url = "https://www.mercedes-benz.com/", Category = "Cars" }
};

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

        if(_bookmarks.Any(b => b.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
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
        // you can uncomment this Thread.Sleep instruction to give you enough time to terminate the program before the export operation completes. 
        // Thread.Sleep(2000);
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


    public BookmarkConflictModel? Import(Bookmark bookmark)
    {
        var conflict = _bookmarks.FirstOrDefault(b => b.Url == bookmark.Url && b.Name != bookmark.Name);
        if(conflict is not null)
        {
            var conflictModel = new BookmarkConflictModel { OldName = conflict.Name, NewName = bookmark.Name, Url = bookmark.Url };
            conflict.Name = bookmark.Name; // this updates the name of the bookmark.                               
            return conflictModel;
        }
        else
        {
            _bookmarks.Add(bookmark);
            return null;
        }        
    }


    public List<Bookmark> GetBookmarksByCategory(string category)
    {
        return _bookmarks.Where(b => b.Category.ToLower().Equals(category.ToLower())).ToList();    
    }
}
