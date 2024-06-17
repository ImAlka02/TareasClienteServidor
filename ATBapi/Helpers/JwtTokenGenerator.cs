using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ATBapi.Helper
{
    public class JwtTokenGenerator
    {
        public string GetToken(int id, string name, string rol, string? nombreCaja)
        {
            List<Claim> claims = new();


            claims.Add(new Claim(ClaimTypes.Role, rol));
            claims.Add(new Claim("Id", id.ToString()));
			claims.Add(new Claim(ClaimTypes.Name, name));
            if(nombreCaja != null)
            {
                claims.Add(new Claim("NombreCaja", nombreCaja));
            }
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, "ATBapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "ATBapp"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(20).ToString()));

            JwtSecurityTokenHandler handler = new();

            var token = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = "ATBapi",
                Audience = "ATBapp",
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(20),
                NotBefore = DateTime.Now.AddMinutes(-1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes("PROGRAMACIONCLIENTESERVIDOR_2024OPORDIOS")),
                    SecurityAlgorithms.HmacSha256)
            };

            return handler.CreateEncodedJwt(token);
        }
    }
}
