

namespace bookmarkr.ServiceAgents;

public interface IBookmarkrSyncrServiceAgent
{
    Task<List<Bookmark>> Sync(string pat, List<Bookmark> localBookmarks);
}