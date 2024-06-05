using ATBapi.Models.DTOs;
using ATBapi.Models.Entities;
using ATBapi.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ATBapi.Hubs
{
	[Authorize(Roles = "Cajero")]
	public class TicketsHub:Hub
    {
        private readonly ColaEsperaRepository repoColaEspera;
        private readonly TurnoRepository repoTurno;
		private readonly UserRepository repoUser;

		public TicketsHub(ColaEsperaRepository repoColaEspera, TurnoRepository repoTurno, UserRepository repoUser)
        {
            this.repoColaEspera = repoColaEspera;
            this.repoTurno = repoTurno;
			this.repoUser = repoUser;
		}

        public async Task<string> GenerarTicket()
        {
            var NumeroTurno = "ATB-0001";
            var TurnosExistentes = await repoColaEspera.GetAllTurnosAsync();
            if (TurnosExistentes.Any(x=>x.NumeroTurno == NumeroTurno))
            {
                int n = int.Parse(TurnosExistentes.Last().NumeroTurno.Substring(4, 4)) + 1;
                NumeroTurno = "ATB-" + n.ToString();
                Colaespera colaEspera = new()
                {
                    NumeroTurno = NumeroTurno,
                    DateTurnoCreado = DateTime.Now
                };
                await repoColaEspera.InsertAsync(colaEspera);
                return NumeroTurno;
            }

            Colaespera colaEspera1 = new()
            {
                NumeroTurno = NumeroTurno,
                DateTurnoCreado = DateTime.Now
            };

            await repoColaEspera.InsertAsync(colaEspera1);
            return NumeroTurno;
        }

        //Al mandar llamar actualizar tabla actualizara el dashboar de la pantalla
        //de espera, y creara un objeto Turno, el cual recibira el cajero
        //public async Task<TablaEsperaDTO> ActualizarTabla(TablaEsperaDTO dto)
        //{
        //    if (dto != null)
        //    {
        //        var turnoEspera = repoColaEspera.GetTurnoAsync();
        //        var nombreCajaDB = repoUser.Get(dto.IdUser)?.IdCajaNavigation?.Nombre;

        //        Turno t = new()
        //        {
        //            IdUsuario = dto.IdUser,
        //            HoraInicial = DateTime.UtcNow,
        //            HoraFinal = null,
        //            NumeroTurno = turnoEspera.Result.NumeroTurno,
        //            TiempoInicio = TimeOnly.Parse(turnoEspera.Result.DateTurnoCreado.ToLongTimeString()),
        //        };

        //        await repoTurno.InsertAsync(t);
        //        TablaEsperaDTO tablaEspera = new()
        //        {
        //            IdUser = dto.IdUser,
        //            NumeroCaja = +,
        //            NumeroTurno = turnoEspera.Result.NumeroTurno
        //        }
        //        return Ok(t);
        //    }
        //}

        //EL hub puede tener seguridad?
        //Puede usar cliet?
    }
}
