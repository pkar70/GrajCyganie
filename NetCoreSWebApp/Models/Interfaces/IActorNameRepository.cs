namespace NetCoreSWebApp.Models
{
    public interface IActorNameRepository
    {
        List<ActorName> GetActorsByName(string nameMask);
        ActorName? GetActorById(string id);
    }
}
