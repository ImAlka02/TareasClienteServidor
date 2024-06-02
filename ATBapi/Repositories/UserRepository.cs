using ApiActividades.Repositories;
using ATBapi.Models.Entities;

namespace ATBapi.Repositories
{
    public class UserRepository : Repository<Users>
    {
        public UserRepository(atbContext context) : base(context)
        {
            
        }

        public Users GetByEmail(string email)
        {
            try
            {
                var user = context.Users.FirstOrDefault(x => x.Correo == email);
                return user ?? throw new Exception();

            }
            catch (Exception)
            {

                throw new Exception();
            }
            
        }

    }
}
