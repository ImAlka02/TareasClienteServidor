using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiActividades.Helper
{
    public class JwtTokenGenerator
    {
        public string GetToken(int? idSuperior, int id, string name)
        {
            List<Claim> claims = new();

            if(idSuperior == null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            claims.Add(new Claim("idSuperior", idSuperior.ToString() ?? "0"));
            claims.Add(new Claim("Id", id.ToString()));
			claims.Add(new Claim(ClaimTypes.Name, name));
			claims.Add(new Claim(JwtRegisteredClaimNames.Iss, "ApiActividades"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "ActividadesApp"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(5).ToString()));

            JwtSecurityTokenHandler handler = new();

            var token = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = "ApiActividades",
                Audience = "ActividadesApp",
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(5),
                NotBefore = DateTime.Now.AddMinutes(-1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes("PROGRAMACIONCLIENTESERVIDOR_2024OPORDIOS")),
                    SecurityAlgorithms.HmacSha256)
            };

            return handler.CreateEncodedJwt(token);
        }
    }
}
