using ApiActividades.Repositories;
using ATBapi.Models.Entities;

namespace ATBapi.Repositories
{
    public class TurnoRepository : Repository<Turno>
    {
        public TurnoRepository(atbContext context) : base(context)
		{
            
        }

		public async Task InsertAsync(Turno t)
		{
			context.Turno.Add(t);
			await context.SaveChangesAsync();
		}
	}
}
