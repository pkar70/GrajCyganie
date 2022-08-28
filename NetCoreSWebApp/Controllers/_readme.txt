
Cel: przy korzystaniu z MVC, dla app pokazującej strony

- szkiele Controllera

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoreCodeCamp.Controllers
{
  [Route("api/[controller]")] - czyli bedzie dla /api/Values
  public class ValuesController : ControllerBase
  {

    public string[] Get() - czyli dla GET
    {
      return new[] { "Hello", "From","Pluralsight" };
    }

    public object Get() - bedzie JSON
    {
      return new { Moniker="ATL2018", Name= "Hello" };
    }

    [HttGet] - wtedy nazwa moze byc dowolna
    public IActionResult Get() - bedzie JSON
    {
      return Ok(new { Moniker="ATL2018", Name= "Hello" }); -- z roznymi HttpStatus
    }

    ctor(IcosRepository)
    {
    try
    {
     - wtedy async Task<> Get()
     return Ok(await _reposit.GetAll())
     }
     catch
     {
     return this.StatusCode(StatusCode.dowolny,"message")
     }
    }

    [HttpGet("{moniker}")] -- bedzie dla GET ale z monikerem(parametrem)
    get(string moniker)
    {
    }

    [HttpGet("{moniker:int}")] -- bedzie dla GET ale z monikerem(parametrem), ktory ma byc int

    [HttGet]
    public IActionResult Get(bool paramName = false) - moze byc wtedy albo normalne GET, albo GET ...?paramName=true

    [HttGet("search")]
    public IActionResult Get(DateTime thedate) - GET ...search?paramName=true
    - moga byc rozne search?thedate, roznia sie parametrami Get() a wiec i automatycznie trafia tam gdzie chcemy
    
    public Task<ActionResult<CampModel>>Post([FromBody]CampModel model)
    - przyjmuje POST, oraz zjada "model" z Body requesta


    [Route("api/camps/{moniker}/talks")] -- i wtedy robi bardziej skomplikowana sciezke

    przy podaniu HttpGet(COS):
    [MapToApiVersion("1.0")]
    
    przy klasie:
    [Route...]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]  - bo obie sa w srodku
    [ApiController]

  }
}