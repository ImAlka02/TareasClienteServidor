using ApiActividades.Models.DTOs;
using ApiActividades.Models.Entities;
using ApiActividades.Models.Validators;
using ApiActividades.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiActividades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin,User")]
    public class ActividadController : ControllerBase
    {
        private readonly ActividadesRepository repoActividad;

        public ActividadController(ActividadesRepository repoActividad)
        {
            this.repoActividad = repoActividad;
        }
        [HttpGet]
        public ActionResult<IEnumerable<ActividadDTO>> Get()
        {
           
            if (User.FindFirstValue("idSuperior") != "") 
            {
                var actividades = repoActividad.GetAll() 
                    .Where(x=> x.IdDepartamento == int.Parse(User.FindFirstValue("Id")??"0") || x.IdDepartamentoNavigation.IdSuperior == int.Parse(User.FindFirstValue("Id") ?? "0"))
                    .Select(x => new ActividadDTO()
                    {
                        Id = x.Id,
                        Titulo = x.Titulo,
                        Descripcion = x.Descripcion ??"",
                        Estado = x.Estado,
                        FechaRealizacion = x.FechaRealizacion,
                        NombreDepartamento = x.IdDepartamentoNavigation.Nombre
                    });

                return Ok(actividades);
            }

            var actividades1 = repoActividad.GetAll().Select(x=> new ActividadDTO()
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Descripcion = x.Descripcion ?? "",
                Estado = x.Estado,
                FechaRealizacion = x.FechaRealizacion,
                NombreDepartamento = x.IdDepartamentoNavigation.Nombre
            });

            return Ok(actividades1);
        }

        [HttpGet("{id}")]
        public ActionResult<ActividadDTO> Get(int id)
        {
            var actividad = repoActividad.GetById(id);

            if(actividad == null) { return NotFound(); }

            if(User.FindFirstValue("idSuperior") == "" || actividad.IdDepartamentoNavigation.IdSuperior >= int.Parse(User.FindFirstValue("idSuperior") ?? "0"))
            {
                ActividadDTO actividaDto = new()
                {
                    Id = actividad.Id,
                    Titulo = actividad.Titulo,
                    Descripcion = actividad.Descripcion ?? "",
                    Estado = actividad.Estado,
                    FechaRealizacion = actividad.FechaRealizacion,
                    NombreDepartamento = actividad.IdDepartamentoNavigation.Nombre
                };

                return Ok(actividaDto);
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult CrearActividad(ActividadDTO actividad)
        {
            ActividadValidator validator = new();
            var resultados = validator.Validate(actividad);
            if (!resultados.IsValid) { return BadRequest(resultados.Errors.Select(x => x.ErrorMessage)); }

            Actividades act = new()
            {
                Titulo = actividad.Titulo,
                Descripcion=actividad.Descripcion,
                FechaRealizacion = actividad.FechaRealizacion,
                IdDepartamento = int.Parse(User.FindFirstValue("Id") ?? "0"),
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow,
                Estado = 0
            };

            repoActividad.Insert(act);
            return Ok("Se creo correctamente la activiad");
        }

        [HttpPut("Editar")]
        public ActionResult EditarActividad(ActividadDTO actividad)
        {
            ActividadValidator validator = new();
            var resultados = validator.Validate(actividad);
            if (!resultados.IsValid) { return BadRequest(resultados.Errors.Select(x => x.ErrorMessage)); }

            var actividadDB = repoActividad.GetById(actividad.Id ?? 0);
            
            if(actividadDB == null) { return NotFound(); }
            if (actividadDB.IdDepartamento == int.Parse(User.FindFirstValue("Id") ?? "0")) 
            {
                actividadDB.Titulo = actividad.Titulo;
                actividadDB.Descripcion = actividad.Descripcion;
                actividadDB.FechaRealizacion = actividad.FechaRealizacion;
                actividadDB.FechaActualizacion = DateTime.UtcNow;

                repoActividad.Update(actividadDB);
                return Ok("Se edito correctamente la activiad");
            }
            return BadRequest("No puedes editar una actividad que no te pertenece");
        }

        [HttpPut("Publicar/{id}")]
        public ActionResult PublicarActividad(int id)
        {
            var actividad = repoActividad.GetById(id);
            if (actividad == null) { return NotFound(); }
            if (actividad.IdDepartamento == int.Parse(User.FindFirstValue("Id") ?? "0"))
            {
                actividad.Estado = 1;
                repoActividad.Update(actividad);
                return Ok("Se publico correctamente");
            }
            return BadRequest("No puedes publicar una actividad que no te pertenece");
        }

        [HttpDelete("Eliminar/{id}")]
        public ActionResult EliminarActividad(int id)
        {
            var actividad = repoActividad.GetById(id);
            if (actividad == null) { return NotFound(); }
            if (actividad.IdDepartamento == int.Parse(User.FindFirstValue("Id") ?? "0"))
            {
                actividad.Estado = 2;
                repoActividad.Update(actividad);
                return Ok("Se elimino correctamente");
            }
            return BadRequest("No puedes eliminar una actividad que no te pertenece");

        }
    }
}
