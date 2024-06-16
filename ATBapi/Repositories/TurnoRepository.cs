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

        public Turno? GetByTurno(string turno)
        {
            return context.Turno.FirstOrDefault(x => x.NumeroTurno == turno && x.Estado == "Atendiendo" && x.HoraInicial.Date == DateTime.Now.Date);
        }
        public Turno? GetByUser(int idUser)
		{
			return context.Turno.FirstOrDefault(x => x.IdUsuario == idUser && x.Estado == "Atendiendo") ;
		}

		public async Task<IEnumerable<Turno>> GetAllTurnosAsync()
		{
			return  await context.Turno.Where(x=> x.HoraInicial.Date == DateTime.Now.Date).ToListAsync();
		}

		public async Task<Turno>? GetTurnoByUserAsync(int IdUser)
		{
			var turno = await context.Turno.FirstOrDefaultAsync(x => x.IdUsuario == IdUser && x.Estado == "Atendiendo");
			if(turno == null) { return null; }
			return turno;
		}

        public async Task<Turno>? GetTurnoByIdAsync(int Id)
        {
            var turno = await context.Turno.FirstOrDefaultAsync(x => x.Id == Id && x.Estado == "Atendiendo");
            if (turno == null) { return null; }
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
