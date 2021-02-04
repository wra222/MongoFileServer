using Microsoft.AspNetCore.Mvc;
namespace MonGo.Controllers
{

    [Produces("application/json")]
        [Route("api/Health")]

        public class HealthController : Controller
        {
            [HttpGet]
            public IActionResult Get() => Ok("ok");
          }
    
}
