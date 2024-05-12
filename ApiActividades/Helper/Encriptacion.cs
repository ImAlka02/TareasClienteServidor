using System.Security.Cryptography;
using System.Text;

namespace ApiActividades.Helper
{
    public class Encriptacion
    {
        public static string StringToSha512(string password)
        {
            using (var sha512 = SHA512.Create())
            {
                var arreglo = Encoding.UTF8.GetBytes(password);
                var hash = sha512.ComputeHash(arreglo);
                return Convert.ToHexString(hash).ToUpper();
            }
        }
    }
}
