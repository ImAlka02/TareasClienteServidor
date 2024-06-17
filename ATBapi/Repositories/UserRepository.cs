using ApiActividades.Repositories;
using ATBapi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATBapi.Repositories
{
    public class UserRepository : Repository<Users>
    {
        public UserRepository(atbContext context) : base(context)
        {
            
        }

        public IEnumerable<Users>? GetAllUsers()
        {
            var users = context.Users.Include(x => x.IdRoleNavigation);
            if (users != null)
            {
                return users;
            }
            return null;
        }

        public Users? GetByEmail(string email)
        {
            var user = context.Users.Include(x=>x.IdRoleNavigation).Include(x=>x.IdCajaNavigation).FirstOrDefault(x => x.Correo == email);
            if(user != null)
            {
                return user;
            }
            return null;
        }

        public Users? GetById(int id) 
        {            
            var user = context.Users.Include(x => x.IdRoleNavigation).Include(x=>x.IdCajaNavigation).FirstOrDefault(x=>x.Id == id);
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public async Task<Users>? GetByIdAsync(int id)
        {
            var user = await context.Users.Include(x => x.IdRoleNavigation).FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                return user;
            }
            return null;
        }

    }
}
