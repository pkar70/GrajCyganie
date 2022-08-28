using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreSWebApp.Models;

namespace NetCoreSWebApp.Controllers
{
    public class StoreFileController : Controller
    {
        private IStoreFileRepository _repository;
        public StoreFileController(IStoreFileRepository storeFileRepository)
        {
            _repository = storeFileRepository;
        }

        // GET: HomeController
        public ActionResult Index()
        {
            // strona z szukaniem powinna być
            return View();
        }

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            StoreFile? storeFile = _repository.GetById(id);
            if (storeFile == null)
                return NotFound();
            return View(storeFile);

        }

    }
}
