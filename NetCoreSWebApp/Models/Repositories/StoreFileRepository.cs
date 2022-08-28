namespace NetCoreSWebApp.Models
{
    public class StoreFileRepository : IStoreFileRepository
    {
        private LocalSQLContext _dbContext;

        public StoreFileRepository(LocalSQLContext dbContext)
        {
            _dbContext = dbContext;
        }

        public StoreFile? GetById(int fileId)
        {
            return _dbContext.StoreFiles.FirstOrDefault(p => p.ID == fileId);
        }

        public List<StoreFile> GetFilesByFileName(string fileNameMask)
        {
            List<StoreFile> storeFiles = _dbContext.StoreFiles.Where(af => af.Name.Contains(fileNameMask)).ToList();
            return storeFiles;
        }

        public List<StoreFile> GetFilesByFileNameOrPath(string fileMask)
        {
            List<StoreFile> storeFiles = _dbContext.StoreFiles.Where(af => af.Name.Contains(fileMask) || af.Path.Contains(fileMask)).ToList();
            return storeFiles;
        }

        List<StoreFile> IStoreFileRepository.GetFilesByFileNameAndPath(string fileNameMask, string pathMask)
        {
            List<StoreFile> storeFiles = _dbContext.StoreFiles.Where(af => af.Name.Contains(fileNameMask) && af.Path.Contains(pathMask)).ToList();
            return storeFiles;
        }
    }
}
