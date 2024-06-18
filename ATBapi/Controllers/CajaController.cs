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
        private readonly UserRepository repoUser;

        public CajaController(CajaRepository repoCaja, UserRepository repoUser)
        {
            this.repoCaja = repoCaja;
            this.repoUser = repoUser;
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

        [HttpDelete]
        public ActionResult EliminarCaja(CajaDTO cajaDto)
        {
            var caja = repoCaja.GetById(cajaDto.Id);
            if(caja == null) { return NotFound("La caja a eliminar no se encontro. "); }
            if(caja.Users.Count != 0) 
            {
                foreach (var user in caja.Users)
                {
                    if(user.Estado == "Conectado") 
                    {
                        return BadRequest("No se puede eliminar una caja en uso. "); 
                    }
                    else
                    {
                        user.IdCaja = null;
                        repoUser.Update(user);
                        repoCaja.Delete(caja);
                        return Ok("Se elimino correctamente. ");
                    }
                }
                repoCaja.Delete(caja);
                return Ok("Se elimino correctamente. ");
            }
            repoCaja.Delete(caja);
            return Ok("Se elimino correctamente. ");
        }
    }
}
