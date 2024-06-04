using ATBapi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATBapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnoController : ControllerBase
    {
        private readonly ColaEsperaRepository repoColaEspera;

        public TurnoController(ColaEsperaRepository repoColaEspera)
        {
            this.repoColaEspera = repoColaEspera;
        }

        //[HttpPost]
        //public ActionResult AddTurno()
        //{
        //    var clienteDB = repoColaEspera.GetAll().First();

        //    if(clienteDB == null) { return NotFound("No hay clientes que atender."); }
            

            
        //}
    }
}
