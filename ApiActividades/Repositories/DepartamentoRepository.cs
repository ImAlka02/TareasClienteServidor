using ApiActividades.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiActividades.Repositories
{
    public class DepartamentoRepository : Repository<Departamentos>
    {
        public DepartamentoRepository(ItesrcneActividadesContext context) : base(context)
        {
        }

        public Departamentos? GetById(int id)
        {
            return context.Departamentos
                .Include(x => x.IdSuperiorNavigation)
                .Include(x => x.InverseIdSuperiorNavigation)
                .Include(x=> x.Actividades)
                .FirstOrDefault(x=> x.Id == id);
        }
        public Departamentos? GetByEmail(string email)
        {
            return context.Departamentos.FirstOrDefault(x => x.Username == email);
        }
    }
}
