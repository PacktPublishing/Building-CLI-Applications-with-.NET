

namespace bookmarkr.ServiceAgents;

public interface IBookmarkrSyncrServiceAgent
{
    Task<List<Bookmark>> Sync(List<Bookmark> localBookmarks);
}