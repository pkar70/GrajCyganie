namespace NetCoreSWebApp.Models
{
    public interface IActorFilmRepository
    {
        List<ActorFilm> GetFilmyAktora(string aktorId);
    }
}
