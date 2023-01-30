using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NovitSoftware.Academia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Comercial")]
    public class ReservaController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Lista de Reservas");
        }
    }
}
