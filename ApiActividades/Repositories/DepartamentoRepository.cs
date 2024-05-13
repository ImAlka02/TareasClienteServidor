using ApiActividades.Models.Entities;

namespace ApiActividades.Repositories
{
    public class DepartamentoRepository : Repository<Departamentos>
    {
        public DepartamentoRepository(ItesrcneActividadesContext context) : base(context)
        {
        }

        public Departamentos? GetByEmail(string email)
        {
            return context.Departamentos.FirstOrDefault(x => x.Username == email);
        }
    }
}
