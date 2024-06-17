using ApiActividades.Repositories;
using ATBapi.Models.DTOs;
using ATBapi.Models.Entities;
using ATBapi.Models.Validators;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATBapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CajaController : ControllerBase
    {
        private readonly CajaRepository repoCaja;

        public CajaController(CajaRepository repoCaja)
        {
            this.repoCaja = repoCaja;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CajaDTO>> GetAllCajas()
        {
            var cajas = repoCaja.GetAll().Select(x => new CajaDTO()
            {
                Id = x.Id,
                Nombre = x.Nombre
            });

            if(cajas == null ) { BadRequest("No hay cajas que mostrar."); }

            return Ok(cajas);
        }

        [HttpPost]
        public ActionResult AddCaja(CajaDTO caja)
        {
            CajaValidator validator = new(repoCaja.context);
            var resultados = validator.Validate(caja);

            if (!resultados.IsValid)
            {
                return BadRequest(resultados.Errors.Select(x => x.ErrorMessage));
            }

            Caja c = new()
            {
                Nombre = caja.Nombre
            };

            repoCaja.Insert(c);
            return Ok("Se agrego la caja correctamente.");

        }

        [HttpPut]
        public ActionResult EditCaja(CajaDTO caja)
        {
            CajaValidator validator = new(repoCaja.context);
            var resultados = validator.Validate(caja);

            if (!resultados.IsValid)
            {
                return BadRequest(resultados.Errors.Select(x => x.ErrorMessage));
            }

            var cajaBD = repoCaja.GetById(caja.Id);
            if(cajaBD == null) { return NotFound("No existe la caja a editar."); }

            cajaBD.Nombre = caja.Nombre;
            repoCaja.Update(cajaBD);
            return Ok("Se actualizo correctamente la caja.");
        }
    }
}
