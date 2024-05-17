using ApiActividades.Models.DTOs;
using ApiActividades.Models.Entities;
using ApiActividades.Models.Validators;
using ApiActividades.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace ApiActividades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin,User")]
    public class ActividadController : ControllerBase
    {
        private readonly ActividadesRepository repoActividad;
		private readonly DepartamentoRepository repoDepa;

		public ActividadController(ActividadesRepository repoActividad, DepartamentoRepository repoDepa)
        {
            this.repoActividad = repoActividad;
			this.repoDepa = repoDepa;
		}
        [HttpGet]
        public ActionResult<IEnumerable<ActividadDTO>> Get()
        {
           
            if (User.FindFirstValue("idSuperior") != "") 
            {
                //var actividades = repoActividad.GetAll()
                //    .Where(x => x.IdDepartamento == int.Parse(User.FindFirstValue("Id") ?? "0") || (x.IdDepartamentoNavigation.IdSuperior == int.Parse(User.FindFirstValue("Id") ?? "0") && x.Estado == 1))
                //    .Select(x => new ActividadDTO()
                //    {
                //        Id = x.Id,
                //        Titulo = x.Titulo,
                //        Descripcion = x.Descripcion ?? "",
                //        Estado = x.Estado,
                //        FechaRealizacion = x.FechaRealizacion,
                //        NombreDepartamento = x.IdDepartamentoNavigation.Nombre
                //    }).ToList();

				List<ActividadDTO> actividades2 = new List<ActividadDTO>();

				// Obtener el departamento del usuario actual
				int idUsuario = int.Parse(User.FindFirstValue("Id") ?? "0");
				var departamentoUsuario = repoDepa.GetById(idUsuario);  

                if(departamentoUsuario == null)
                {
                    return BadRequest();
                }
                GetActividadesDepartamentoYHijos(departamentoUsuario, actividades2);
                return Ok(actividades2);
			}

            var actividades1 = repoActividad.GetAll().Where(x=> x.Estado == 1).Select(x=> new ActividadDTO()
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
		private void GetActividadesDepartamentoYHijos(Departamentos departamento, List<ActividadDTO> actividades)
		{
          

            if(departamento.Id == int.Parse(User.FindFirstValue("Id") ?? "0"))
            {
                actividades.AddRange(departamento.Actividades.Select(x => new ActividadDTO()
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Descripcion = x.Descripcion ?? "",
                    Estado = x.Estado,
                    FechaRealizacion = x.FechaRealizacion,
                    NombreDepartamento = departamento.Nombre
                }));
            }
            else
            {
                actividades.AddRange(repoDepa.GetById(departamento.Id).Actividades.Where(x => x.Estado == 1).Select(x => new ActividadDTO()
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Descripcion = x.Descripcion ?? "",
                    Estado = x.Estado,
                    FechaRealizacion = x.FechaRealizacion,
                    NombreDepartamento = departamento.Nombre
                }));
            }

            
			foreach (var hijo in departamento.InverseIdSuperiorNavigation)
			{
				GetActividadesDepartamentoYHijos(hijo, actividades);
			}
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

            if(actividad.Imagen != null)
            {
				SubirImg(actividad.Imagen, act.Id);
			}

			return Ok("Se creo correctamente la activiad");
        }

		private void SubirImg(string img, int idAct)
		{
			if (string.IsNullOrEmpty(img)) { }
				

			try
			{
				// Convertir el string base64 a un arreglo de bytes
				var imageBytes = Convert.FromBase64String(img);

				// Crear la ruta completa para guardar la imagen
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", User.Identity.Name);

				// Asegurarse de que el directorio existe
				if (!Directory.Exists(filePath))
				{
					Directory.CreateDirectory(filePath);
				}

				// Nombre del archivo
				var fileName = $"{idAct}.jpg"; // o el formato que prefieras

				// Ruta completa del archivo
				var fullFilePath = Path.Combine(filePath, fileName);

				// Guardar la imagen en el disco
				System.IO.File.WriteAllBytes(fullFilePath, imageBytes);

				//return Ok(new { filePath = fullFilePath });
			}
			catch (Exception ex)
			{
				//return StatusCode(500, $"Internal server error: {ex.Message}");
			}
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
