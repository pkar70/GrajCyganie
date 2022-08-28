namespace NetCoreSWebApp.Models
{
    public interface IStoreFileRepository
    {
        List<StoreFile> GetFilesByFileName(string fileNameMask);
        List<StoreFile> GetFilesByFileNameAndPath(string fileNameMask, string pathMask);
        List<StoreFile> GetFilesByFileNameOrPath(string fileMask);

        StoreFile? GetById(int id);
    }
}
