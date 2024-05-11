using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiActividades.Helper
{
    public class JwtTokenGenerator
    {
        public string GetToken(int? idSuperior, string name)
        {
            List<Claim> claims = new();

            if(idSuperior != null)
            {
                claims.Add(new Claim("idSuperior", idSuperior.ToString()));
            }

            claims.Add(new Claim(ClaimTypes.Name, name));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, "https://localhost:7221/"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "https://localhost:7221/"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(5).ToString()));

            JwtSecurityTokenHandler handler = new();

            var token = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = "https://localhost:7221/",
                Audience = "https://localhost:7221/",
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
