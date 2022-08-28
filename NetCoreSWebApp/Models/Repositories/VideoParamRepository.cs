namespace NetCoreSWebApp.Models
{
    public class VideoParamRepository : IVideoParamRepository
    {
        private readonly LocalSQLContext _dbContext;

        public int GetTotalCount()
        {
            return _dbContext.VideoParams.Count();
        }

        public int GetTotalDuration()
        {
            return _dbContext.VideoParams.Sum(x => x.Duration);
        }

        public VideoParam? GetById(int videoParamId)
        {
            return _dbContext.VideoParams.FirstOrDefault(p => p.Id == videoParamId);
        }

        public VideoParamRepository(LocalSQLContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
