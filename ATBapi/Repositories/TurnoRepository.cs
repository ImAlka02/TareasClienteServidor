using ApiActividades.Repositories;
using ATBapi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATBapi.Repositories
{
    public class TurnoRepository : Repository<Turno>
    {
        public TurnoRepository(atbContext context) : base(context)
		{
            
        }

		public async Task<Turno>? GetTurnoByUserAsync(int IdUser)
		{
			var turno = await context.Turno.FirstOrDefaultAsync(x => x.IdUsuario == IdUser && x.Estado == "Atendiendo");
			if(turno == null) { return null; }
			return turno;
		}
		public async Task InsertAsync(Turno t)
		{
			context.Turno.Add(t);
			await context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Turno t)
		{
			context.Turno.Update(t);
			await context.SaveChangesAsync();
		}
	}
}
