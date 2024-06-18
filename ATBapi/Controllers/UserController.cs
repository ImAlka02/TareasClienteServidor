using ATBapi.Helper;
using ATBapi.Models.DTOs;
using ATBapi.Models.Entities;
using ATBapi.Models.Validators;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ATBapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository repoUser;

        public UserController(UserRepository repoUser)
        {
            this.repoUser = repoUser;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserCompleteDTO>> GetAllUsers()
        {
            var users = repoUser.GetAllUsers()?.Where(x=> x.Id != int.Parse(User.FindFirstValue("Id")) && x.Eliminado < 1).Select(x=> new UserCompleteDTO()
            {
                Correo = x.Correo,
                Id = x.Id,
                IdRol = x.IdRole,
                Nombre = x.Nombre
            });

            if(users != null)
            {
                return Ok(users);
            }

            return BadRequest("No hay usuarios registrados.");
        }

        [HttpGet("{id}")]
        public ActionResult<UserCompleteDTO> GetById(int id)
        {
            var user = repoUser.GetById(id);

            if(user != null)
            {
                UserCompleteDTO dto = new()
                {
                    Correo = user.Correo,
                    Id = user.Id,
                    IdRol = user.IdRole,
                    Nombre = user.Nombre,
                    IdCaja = user.IdCaja ?? 0
                };

                return Ok(dto);
            }

            return NotFound("No se encontro el usuario.");
        }

        [HttpPost]
        public ActionResult AddUser(UserCompleteDTO user) 
        {
            try
            {
                UserValidator validator = new(repoUser.context);
                var resultados = validator.Validate(user);

                if (!resultados.IsValid)
                {
                    return BadRequest(resultados.Errors.Select(x => x.ErrorMessage));
                }

                Users u = new()
                {
                    Nombre = user.Nombre,
                    Correo = user.Correo,
                    Contraseña = Encriptacion.StringToSha512(user.Contraseña),
                    IdRole = user.IdRol
                };

                repoUser.Insert(u);
                return Ok("Se creo correctamente el departamento.");
            }
            catch
            {
                return BadRequest();
            }
            
        }

        [HttpPut]
        public ActionResult EditUser(UserCompleteDTO user)
        {
            UserValidator validator = new(repoUser.context);
            var resultados = validator.Validate(user);

            if (!resultados.IsValid)
            {
                return BadRequest(resultados.Errors.Select(x => x.ErrorMessage));
            }

            var userBD = repoUser.GetById(user.Id);
            if (userBD == null) { return NotFound("No existe este usuario."); }
            if(userBD.Estado == "Conectado") { return BadRequest("El usuario esta conectado, hasta que se desconecte se podra modificar. "); }
            userBD.Nombre = user.Nombre;
            userBD.Correo = user.Correo;
            userBD.Contraseña = Encriptacion.StringToSha512(user.Contraseña);
            userBD.IdRole = user.IdRol;
            userBD.IdCaja = user.IdCaja;
        
            //Que pasa si esta conectado

            repoUser.Update(userBD);
            return Ok("Se actualizo correctamente.");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id) 
        {
            var user = repoUser.GetById(id);
            if (user == null) { return NotFound("No se encontro el usuario."); }

            if (int.Parse(User.FindFirstValue("Id")) == user.Id) { return BadRequest("No te puedes eliminar a ti mismo"); }
            //Si esta conectado no lo puede eliminar
            if(user.Estado == "Conectado") { return BadRequest("No se puede eliminar a alguien conectado. "); }

            user.Eliminado = 1;
            repoUser.Update(user);
            return Ok("Se elimino correctamente.");


        }
    }
}
