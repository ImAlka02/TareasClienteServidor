﻿using ApiActividades.Helper;
using ApiActividades.Models.DTOs;
using ApiActividades.Models.Entities;
using ApiActividades.Models.Validators;
using ApiActividades.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiActividades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	[Authorize(Roles = "Admin")]
	public class DepartamentoController : ControllerBase
    {
        private readonly DepartamentoRepository repoDepa;
        private readonly ActividadesRepository repoActividad;

        public DepartamentoController(DepartamentoRepository repoDepa, ActividadesRepository repoActividad)
        {
            this.repoDepa = repoDepa;
            this.repoActividad = repoActividad;
        }
        [HttpGet]
        public ActionResult<IEnumerable<DepartamentoDTO>> GetDepartamentos()
        {
            var departamentos = repoDepa.GetAll().Where(x=> x.Username.Contains("@samsung.com")).Select(x=> new DepartamentoDTO()
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Correo = x.Username,
                IdSuperior =  x.IdSuperior ?? 0
            });

            return Ok(departamentos);
        }
        [HttpPost]
        public ActionResult CrearDepartamento(DepartamentoDTO depa)
        {
            DepartamentoValidator validator = new(repoDepa.context);
            var resultados = validator.Validate(depa);

            if (!resultados.IsValid)
            {
                return BadRequest(resultados.Errors.Select(x => x.ErrorMessage));
            }

            Departamentos d = new()
            {
                Nombre = depa.Nombre,
                Username = depa.Correo,
                Password = Encriptacion.StringToSha512(depa.Contraseña),
                IdSuperior = depa.IdSuperior ?? null
            };

            repoDepa.Insert(d);
            return Ok("Se creo correctamente el departamento.");
        }

        [HttpPut("Editar")]
        public ActionResult EditarDepartamento(DepartamentoDTO depa)
        {
            DepartamentoEditValidator validator = new(repoDepa.context);
            var resultados = validator.Validate(depa);

            if (!resultados.IsValid)
            {
                return BadRequest(resultados.Errors.Select(x => x.ErrorMessage));
            }
            
            var depaBD = repoDepa.Get(depa.Id ?? 0);
            if(depaBD == null) { return NotFound(); }

            depaBD.Nombre = depa.Nombre;
            depaBD.Username = depa.Correo;
            depaBD.Password = Encriptacion.StringToSha512(depa.Contraseña);
            depaBD.IdSuperior = depa.IdSuperior;

            repoDepa.Update(depaBD);
            return Ok("Se actualizo correctamete el departamento");

        }

        [HttpDelete("Eliminar/{id}")]
        public ActionResult Delete(int id)
        {
            var depa = repoDepa.GetById(id);
            if (depa == null) { return NotFound(); }
            if(int.Parse(User.FindFirstValue("id") ?? "0") == id) { return BadRequest("No te puedes eliminar a ti mismo"); }


            if (depa.InverseIdSuperiorNavigation != null)
            {
                foreach (var departamento in depa.InverseIdSuperiorNavigation)
                {
                    departamento.IdSuperior = depa.IdSuperior;
                    repoDepa.Update(departamento);
                }
            }

            var actDelDepa = repoActividad.GetAll().Where(x => x.IdDepartamento == depa.Id).ToList();

            foreach (var act in actDelDepa)
            {
                repoActividad.Delete(act);      
            }

            
            //Se eliminaran si eliminamos al creador de la actividad??
            repoDepa.Delete(depa);
            return Ok("Se elimino correctamente");
        }
        
    }
}
