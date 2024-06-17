using ApiActividades.Repositories;
using ATBapi.Models.Entities;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATBapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ColaEsperaRepository repoColaEspera;
        private readonly Repository<Configuracion> repoConfi;

        public ConfiguracionController(ColaEsperaRepository repoColaEspera, Repository<Configuracion> repoConfi)
        {
            this.repoColaEspera = repoColaEspera;
            this.repoConfi = repoConfi;
        }
        //Este metodo abrira el banco, dando oportunidad a los usuarios de hacer cualquier operacion
        [HttpPut("/Abrir")]
        public ActionResult AbrirBanco()
        {
            var config = repoConfi.Get(1);
            if(config == null) { return NotFound("No se encontro la configuracion del banco. "); }
            if (config.Estado == "Cerrado")
            {
                config.Estado = "Abierto";
                repoConfi.Update(config);
                return Ok("Se ha abierto el banco. ");
            }
            else
            {
                return BadRequest("El banco esta abierto actualmente. ");
            }
        }

        //Este metodo hara que el Admin al cerrar el banco se limpie
        //La tabla ColaEspera y el banco este en "Cerrado"
        [HttpPut("/Cerrar")]
        public ActionResult CerrarBanco()
        {
            var config = repoConfi.Get(1);
            if (config == null) { return NotFound("No se encontro la configuracion del banco. "); }
            if (config.Estado == "Abierto")
            {
                config.Estado = "Cerrado";
                
                var ColaEsperaList = repoColaEspera.GetAll().ToList();
                if (ColaEsperaList.Count() > 0)
                {
                    foreach (var turno in ColaEsperaList)
                    {
                        repoColaEspera.Delete(turno);
                    }
                    repoConfi.Update(config);
                    return Ok("Se ha cerrado el banco. ");
                }
                else
                {
                    repoConfi.Update(config);
                    return Ok("Se cerro el banco, no habia clientes esperando");
                }
            }
            else
            {
                return BadRequest("El banco esta cerrado actualmente. ");
            }

           
        }
    }
}
