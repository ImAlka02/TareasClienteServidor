using ATBapi.Helper;
using ATBapi.Models.DTOs;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace ATBapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly  UserRepository repoUser;

        public LoginController(UserRepository repoUser)
        {
            this.repoUser = repoUser;
        }

        [HttpPost]
        public IActionResult Post(UserDTO userDTO)
        {
            var user = repoUser.GetByEmail(userDTO.Email);

            if (user == null) { return NotFound(); }

            if (user.Contraseña == Encriptacion.StringToSha512(userDTO.Password))
            {
                JwtTokenGenerator jwtToken = new();

                user.Estado = "Conectado";
                repoUser.Update(user);
                if (user.IdCajaNavigation == null) 
                { 
                    return Ok(jwtToken.GetToken(user.Id, user.Nombre, user.IdRoleNavigation.Nombre, null)); 
                }
                else
                {
                    return Ok(jwtToken.GetToken( user.Id, user.Nombre, user.IdRoleNavigation.Nombre, user.IdCajaNavigation.Nombre ?? null));

                }
            }

            return Unauthorized();
        }

        [HttpPost("/cerrar-sesion")]
        [Authorize(Roles = "Admin,Cajero")]
        public IActionResult CerrarSesion()
        {
            var user = repoUser.GetById(int.Parse(User.FindFirstValue("Id")));

            if (user == null) { return NotFound(); }


            user.Estado = "Desconectado";
            repoUser.Update(user);
            return Ok("Se desconecto.");
        }
    }
}
