using ApiActividades.Repositories;
using ATBapi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATBapi.Repositories
{
    public class ColaEsperaRepository : Repository<Colaespera>
    {
        public ColaEsperaRepository(atbContext context) : base(context)
        {
            
        }

        public IEnumerable<Colaespera>? GetAllTurnos()
        {
            var Turnos = context.Colaespera;

            if(Turnos == null) { return null; }

            return Turnos;
        }

        public async Task<List<Colaespera>> GetAllTurnosAsync()
        {
            return await context.Colaespera.ToListAsync();
        }

        public async Task InsertAsync(Colaespera colaEspera)
        {
            context.Colaespera.Add(colaEspera);
            await context.SaveChangesAsync();
        }

    }
}
