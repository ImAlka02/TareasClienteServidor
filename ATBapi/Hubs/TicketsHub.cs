using ATBapi.Models.DTOs;
using ATBapi.Models.Entities;
using ATBapi.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace ATBapi.Hubs
{
    public class TicketsHub:Hub
    {
        private readonly ColaEsperaRepository repoColaEspera;
        private readonly TurnoRepository repoTurno;

        public TicketsHub(ColaEsperaRepository repoColaEspera, TurnoRepository repoTurno)
        {
            this.repoColaEspera = repoColaEspera;
            this.repoTurno = repoTurno;
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
                
        //    }
        //}

        //EL hub puede tener seguridad?
        //Puede usar cliet?
    }
}
