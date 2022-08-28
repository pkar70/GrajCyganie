using Microsoft.AspNetCore.Mvc;
using NetCoreSWebApp.Models;

namespace NetCoreSWebApp.Controllers
{
    public class AktorzyController : Controller
    {

        // aktorzy?name=Bullock
        // -> lista aktorów (name) (linki do actorId=nm oraz do https://www.imdb.com/name/nm6196153/)

        // aktorzy?actorId=nm0120147
        // -> HDR: aktor; BODY: lista filmów (tu mozna odfiltrowac TXT, badz pliki ktore nie mają VideoParam) jako link do filmy.asp?name=tt.... oraz <small>(as POSTAC)</small>

        // aktorzy?postac=cos [do dodania względem ASP)
        // -> HDR: 

        private readonly IActorNameRepository _actorNameRepository;
        private readonly IActorFilmRepository _actorFilmRepository;
        private readonly IStoreFileRepository _storeFileRepository;

        public AktorzyController(IActorNameRepository actorNameRepository, IActorFilmRepository actorFilmRepository,IStoreFileRepository storeFileRepository)
        {
            _actorNameRepository = actorNameRepository;
            _actorFilmRepository = actorFilmRepository;
            _storeFileRepository = storeFileRepository;
        }


        // GET: /aktorzy
        //[HttpGet]
        public ActionResult Index(string actorId = "", string name="", string filmId = "")
        {
            if (actorId != "")
            {
                //                ' filmy aktora

                ActorName? actorName = _actorNameRepository.GetActorById(actorId);
                if (actorName == null)
                    return NotFound("Niestety, nie mogę znaleźć tego aktora?");

                List<ActorFilm> actorFilmList = _actorFilmRepository.GetFilmyAktora(actorId);
                if (actorFilmList == null)
                    return NotFound("Niestety, nie mogę znaleźć filmów tego aktora?");

                // mamy listę filmów (tt...), to teraz lista plików
                List<StoreFile> storeFiles = new List<StoreFile>();
                foreach (ActorFilm actorFilm in actorFilmList)
                {
                    List<StoreFile> plikiFilmu = _storeFileRepository.GetFilesByFileName("[" + actorFilm.FilmId.Trim() + "]");

                    // podmiana Path na graną postać 
                    foreach (StoreFile storeFile in plikiFilmu)
                    {
                        storeFile.Path = actorFilm.Postac;
                    }
                    storeFiles = storeFiles.Concat(plikiFilmu).ToList();
                }

                ViewBag.ActorName = actorName.Name;
                ViewBag.Lista = storeFiles;
                return View("actorListaFilmow");

            }

            if (name != "")
            {
                //    ' czyli wyszukujemy aktora...
                List<ActorName> actors = _actorNameRepository.GetActorsByName(name).
                    OrderBy(an => an.Name).ToList();
                if (actors == null)
                    return NotFound($"Nie mogę znaleźć tego aktora ({name})");

                ViewBag.Lista = actors;
                return View("actorListaAktorow");
            }

            if (filmId != "")
            {
                // aktorzy do filmu
    //            sQry = "SELECT * FROM StoreFiles WHERE name LIKE '%" & Request("filmId") & "%' OR path LIKE '%" & Request("filmId") & "%'"

    //Response.Write "<!-- " & sQry & " -->"

    //Set oRs = objConn.Execute(sQry)

    //Response.Write "<!-- po Qyery -->"


    //If oRs.Eof Then

    //    Response.Write  "<h4>Niestety, nie mogę znaleźć tego filmu?</h4>" & vbCrLf & "<ul>" & vbCrLf

    //Else

    //    Response.Write "<h4>" & oRs("path") & "\" & oRs("name") & " </ h4 > " & vbCrLf & " < ul > " & vbCrLf

    //End If


    //sQry = "SELECT * FROM actorNames INNER JOIN actorFilm ON actorNames.id = actorFilm.actorId WHERE filmId LIKE '" & Request("filmId") & "%'"

    //Response.Write "<!-- " & sQry & " -->"

    //Set oRs = objConn.Execute(sQry)

    //Do While Not oRs.Eof

    //    Response.Write "<li><a href='aktorzy.asp?actorid=" & oRs("actorId") & "'>" & oRs("name") & "</a> <small>(as " & oRs("postac") & ")</small>"

    //    sLink = ActorName2AlbumName(oRs("name"))

    //    If sLink<> "" Then

    //        Response.Write sLink

    //    End If


    //    Response.Write vbCrLf

    //    oRs.MoveNext
    //Loop

    //Response.Write "</ul>"
            }

            return View();
        }

    }
}
