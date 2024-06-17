using ATBapi.Models.DTOs;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATBapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly TurnoRepository repoTurno;

        public DashboardController(TurnoRepository repoTurno)
        {
            this.repoTurno = repoTurno;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DashboardDTO>> GetTurnosByCaja()
        {
            var turnos = repoTurno.GetAllTurnos().Select(x=> new DashboardDTO()
            {
                 Caja = x.IdUsuarioNavigation.IdCajaNavigation.Nombre,
                 NumeroTurno = x.NumeroTurno
            });

            if (turnos != null)
            {
                return Ok(turnos);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
