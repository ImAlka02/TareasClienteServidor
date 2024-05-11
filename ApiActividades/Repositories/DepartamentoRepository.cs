using ApiActividades.Models.Entities;

namespace ApiActividades.Repositories
{
    public class DepartamentoRepository : Repository<Departamentos>
    {
        public DepartamentoRepository(ItesrcneActividadesContext context) : base(context)
        {
        }

        public Departamentos? GetByName(string name)
        {
            return context.Departamentos.FirstOrDefault(x => x.Nombre == name);
        }
    }
}
