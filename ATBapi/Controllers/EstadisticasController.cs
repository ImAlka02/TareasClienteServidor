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
            var estadisticas = repoTurno.GetAllT().Select(x=> new {
                
                HoraInicialPromedio = TimeOnly.FromDateTime(x.HoraInicial),
                HoraFinalPromedio = TimeOnly.FromDateTime(x.HoraFinal.Value),
                HoraInicioAtencionPromedio = x.TiempoInicio
            }).ToList();

            double tiempoPromedioDeAtencion = estadisticas.Average(x =>
            (x.HoraInicialPromedio - x.HoraFinalPromedio).TotalHours);

            var tiempoPromedioEspera = estadisticas.Average(x => (x.HoraInicioAtencionPromedio - x.HoraFinalPromedio).TotalHours);

            EstadisticasDTO estadisticasDTO = new EstadisticasDTO()
            {
                TiempoPromedioDeAtencion = tiempoPromedioDeAtencion.ToString("F2"), //Tiempo promedio en horas
                TiempoPromedioDeEspera = tiempoPromedioEspera.ToString("F2"), //Tiempo promedio en horas
                
            };

            

            return Ok();
        }
    }
}
