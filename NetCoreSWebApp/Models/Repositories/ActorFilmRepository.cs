namespace NetCoreSWebApp.Models
{
    public class ActorFilmRepository : IActorFilmRepository
    {
        private readonly LocalSQLContext _dbContext;

        public ActorFilmRepository(LocalSQLContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ActorFilm> GetFilmyAktora(string aktorId)
        {
            List<ActorFilm> actorFilmList = _dbContext.ActorFilms.Where(af => af.ActorId.Contains(aktorId)).ToList();
            return actorFilmList;
        }
    }
}
