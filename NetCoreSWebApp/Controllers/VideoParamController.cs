using Microsoft.AspNetCore.Mvc;
using NetCoreSWebApp.Models;

namespace NetCoreSWebApp.Controllers
{
    public class VideoParamController : Controller
    {
        private readonly IVideoParamRepository _repository;

        public VideoParamController(IVideoParamRepository paramRepository)
        {
            _repository = paramRepository;
        }

        public IActionResult Details(int id)
        {
            VideoParam? videoParam = _repository.GetById(id);
            if(videoParam == null)
                return NotFound();
            return View(videoParam);
        }
    }
}
