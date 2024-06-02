using ATBapi.Helper;
using ATBapi.Models.DTOs;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                return Ok(jwtToken.GetToken(user.IdRole, user.Id, user.Nombre));
            }

            return Unauthorized();
        }
    }
}
