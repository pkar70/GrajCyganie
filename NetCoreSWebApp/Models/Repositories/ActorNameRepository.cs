namespace NetCoreSWebApp.Models
{
    public class ActorNameRepository : IActorNameRepository
    {
        private readonly LocalSQLContext _dbContext;

        public ActorNameRepository(LocalSQLContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ActorName> GetActorsByName(string nameMask)
        {
            // może być jakoś do regexp
            List<ActorName> lista = _dbContext.ActorNames.Where(an => an.Name.Contains(nameMask)).ToList();
            return lista;
        }

        public ActorName? GetActorById(string id)
        {
            List<ActorName> temp = _dbContext.ActorNames.Where(an => an.Id.Contains(id)).ToList();
            if(temp == null)
                return null;
            return temp.First();
        }

    }
}
