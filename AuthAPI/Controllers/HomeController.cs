using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet("run")]
        public async Task<IActionResult> Run()
        {
            return Ok("API running");
        }
    }
}
