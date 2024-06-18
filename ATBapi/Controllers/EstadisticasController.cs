using ATBapi.Models.DTOs;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATBapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EstadisticasController : ControllerBase
    {
        private readonly TurnoRepository repoTurno;

        public EstadisticasController(TurnoRepository repoTurno)
        {
            this.repoTurno = repoTurno;
        }

        [HttpGet]
        public ActionResult<EstadisticasDTO> GetEstadisticas()
        {
            var turnos = repoTurno.GetAllT().Where(x=>x.HoraFinal != null && x.Estado != "Cancelado" && x.Estado != "Atendiendo").ToList();

            if (turnos == null || !turnos.Any())
            {
                return NotFound("No se encontraron turnos.");
            }

            var tiempoEsperaTotal = TimeSpan.Zero;
            var tiempoAtencionTotal = TimeSpan.Zero;
            var totalTurnosConFinal = 0;

            var personasAtendidasPorCaja = new Dictionary<string, int>();

            foreach (var turno in turnos)
            {
                if (turno.HoraFinal.HasValue)
                {
                    var tiempoInicioDateTime = turno.HoraInicial.Date + turno.TiempoInicio.ToTimeSpan();
                    var tiempoEspera = turno.HoraInicial - tiempoInicioDateTime;

                    var tiempoAtencion = turno.HoraFinal.Value - turno.HoraInicial;

                    tiempoEsperaTotal += tiempoEspera;
                    tiempoAtencionTotal += tiempoAtencion;
                    totalTurnosConFinal++;

                    if (personasAtendidasPorCaja.ContainsKey(turno.IdUsuarioNavigation.IdCajaNavigation.Nombre))
                    {
                        personasAtendidasPorCaja[turno.IdUsuarioNavigation.IdCajaNavigation.Nombre]++;
                    }
                    else
                    {
                        personasAtendidasPorCaja[turno.IdUsuarioNavigation.IdCajaNavigation.Nombre] = 1;
                    }
                }
            }

            var estadisticasDTO = new EstadisticasDTO
            {
                TiempoPromedioDeEspera = (tiempoEsperaTotal / totalTurnosConFinal).ToString(@"hh\:mm\:ss"),
                TiempoPromedioDeAtencion = (tiempoAtencionTotal / totalTurnosConFinal).ToString(@"hh\:mm\:ss"),
                PersonasAtendidasPorCaja = personasAtendidasPorCaja
            };

            return Ok(estadisticasDTO);
        }
    }
}
