namespace bookmarkr.Services;

public interface IBookmarkService
{
    void AddLink(string name, string url, string category);

    void AddLinks(string[] names, string[] urls, string[] categories);
    
    void ListAll();

    List<Bookmark> GetAll();
    
    void Import(List<Bookmark> bookmarks);

    BookmarkConflictModel? Import(Bookmark bookmark);

    List<Bookmark> GetBookmarksByCategory(string category);
}
