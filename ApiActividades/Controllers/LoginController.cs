using ApiActividades.Helper;
using ApiActividades.Models.DTOs;
using ApiActividades.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiActividades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DepartamentoRepository repoDepartamento;

        public LoginController(DepartamentoRepository repoDepartamento)
        {
            this.repoDepartamento = repoDepartamento;
        }

        [HttpPost]
        public IActionResult Post(UserDTO userDTO)
        {
            var user = repoDepartamento.GetByEmail(userDTO.Email);

            if (user == null) { return NotFound(); }

            if (user.Password == Encriptacion.StringToSha512(userDTO.Password))
            {
                JwtTokenGenerator jwtToken = new();
                return Ok(jwtToken.GetToken(user.IdSuperior, user.Id, user.Nombre));
            }

            return Unauthorized();
        }
    }
}
