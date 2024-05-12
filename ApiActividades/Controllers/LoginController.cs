using ApiActividades.Helper;
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
        public IActionResult Post(string username, string password)
        {
            var user = repoDepartamento.GetByName(username);

            if (user == null) { return BadRequest("Falta usuario."); }

            if (user.Password == Encriptacion.StringToSha512(password))
            {
                JwtTokenGenerator jwtToken = new();
                return Ok(jwtToken.GetToken(user.IdSuperior, user.Id));
            }

            return Unauthorized();
        }
    }
}
