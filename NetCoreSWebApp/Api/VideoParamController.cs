using Microsoft.AspNetCore.Mvc;
using NetCoreSWebApp.Models;

// jako Empty controller


namespace NetCoreSWebApp.Api
{
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[Controller]")]
    [ApiController]
    public class VideoParamController : ControllerBase
    {
        IVideoParamRepository _paramRepository;

        public VideoParamController(IVideoParamRepository paramRepository)
        {
            _paramRepository = paramRepository;
        }
        

        private IActionResult GetById(int id)
        {
            VideoParam? videoParam = _paramRepository.GetById(id);
            if (videoParam == null)
                return NotFound();

            return Ok(videoParam);
        }

        // [ApiVersionNeutral]
        // /api/videoparam/1
        [HttpGet("{id:int}")]
        [ApiVersion("1.0")]
        public IActionResult GetUri(int id)
        {
            return GetById(id);
        }

        // /api/videoparam/?id=1
        [ApiVersion("1.0")]
        [HttpGet]
        public IActionResult GetParam(int id = 0)
        {
            return GetById(id);
        }


    }
}
