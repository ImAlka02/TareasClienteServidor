using ApiActividades.Models.DTOs;
using ApiActividades.Models.Entities;
using ApiActividades.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiActividades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var actividades = repoActividad.GetAll().Select(x=> new ActividadDTO()
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Descripcion = x.Titulo,
                Estado = x.Estado,
                FechaRealizacion = x.FechaRealizacion,
                NombreDepartamento = x.IdDepartamentoNavigation.Nombre
            });

            return Ok(actividades);
        }

        [HttpGet("{id}")]
        public ActionResult<ActividadDTO> Get(int id)
        {
            var actividad = repoActividad.GetById(id);

            if(actividad == null) { return NotFound(); }

            ActividadDTO actividaDto = new()
            {
                Id = actividad.Id,
                Titulo = actividad.Titulo,
                Descripcion = actividad.Titulo,
                Estado = actividad.Estado,
                FechaRealizacion = actividad.FechaRealizacion,
                NombreDepartamento = actividad.IdDepartamentoNavigation.Nombre
            };

            return Ok(actividaDto);
        }

        [HttpPost]
        public ActionResult CrearActividad(ActividadDTO actividad)
        {
            if(actividad == null) { return BadRequest("La actividad esta vacia.");}

            //VALIDAR
            //VALIDAR
            //VALIDAR
            //VALIDAR

            Actividades act = new()
            {
                Titulo = actividad.Titulo,
                Descripcion=actividad.Descripcion,
                FechaRealizacion = actividad.FechaRealizacion,
                IdDepartamento = int.Parse(actividad.NombreDepartamento),
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow,
                Estado = actividad.Estado ?? 0
            };

            repoActividad.Insert(act);
            return Ok("Se creo correctamente la activiad");
        }

        [HttpPut("Editar")]
        public ActionResult EditarActividad(ActividadDTO actividad)
        {
            if (actividad == null) { return BadRequest("La actividad esta vacia."); }

            //VALIDAR
            //VALIDAR
            //VALIDAR
            //VALIDAR

            var actividadDB = repoActividad.GetById(actividad.Id ?? 0);
            if(actividadDB == null) { return NotFound(); }

            actividadDB.Titulo = actividad.Titulo;
            actividadDB.Descripcion = actividad.Descripcion;
            actividadDB.FechaRealizacion = actividad.FechaRealizacion;
            actividadDB.FechaActualizacion = DateTime.UtcNow;

            repoActividad.Update(actividadDB);
            return Ok("Se edito correctamente la activiad");
        }

        [HttpDelete("Eliminar/{id}")]
        public ActionResult EliminarActividad(int id)
        {
            var actividad = repoActividad.GetById(id);

            if (actividad == null) { return NotFound(); }

            repoActividad.Delete(actividad);
            return Ok("Se elimino correctamente");
        }
    }
}
