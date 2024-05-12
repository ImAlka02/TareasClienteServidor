using ApiActividades.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiActividades.Repositories
{
    public class ActividadesRepository : Repository<Actividades>
    {
        public ActividadesRepository(ItesrcneActividadesContext context) : base(context)
        {
        }

        public virtual IEnumerable<Actividades> GetAll()
        {
            return context.Actividades
                .Include(x=>x.IdDepartamentoNavigation);
        }

        public Actividades? GetById(int id)
        {
            return context.Actividades.Include(x=>x.IdDepartamentoNavigation).FirstOrDefault(x=>x.Id == id);
        }

    }
}
