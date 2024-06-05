using ATBapi.Models.DTOs;
using ATBapi.Models.Entities;
using ATBapi.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

namespace ATBapi.Hubs
{
	
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


        //Este metodo genera un ticket el cual aparecera en la pantalla
        //luego lo manda a la cola de espera en la base de datos esperando
        //A que un cajero lo reciba
        public async Task<string> GenerarTicket()
        {
            var NumeroTurno = "ATB-0001";
            var colaEsperaList = await repoColaEspera.GetAllTurnosAsync();
            var turnosDB = await repoTurno.GetAllTurnosAsync();
            if (turnosDB == null)
            {
                if (colaEsperaList.Count() > 0)
                {
                    if (colaEsperaList.Any(x => x.NumeroTurno == NumeroTurno))
                    {
                        int n = int.Parse(colaEsperaList.Last().NumeroTurno.Substring(4, 4)) + 1;
                        NumeroTurno = "ATB-" + n.ToString();
                        Colaespera colaEspera = new()
                        {
                            NumeroTurno = NumeroTurno,
                            DateTurnoCreado = DateTime.Now
                        };
                        await repoColaEspera.InsertAsync(colaEspera);
                        return NumeroTurno;
                    }
                }
                Colaespera colaEspera1 = new()
                {
                    NumeroTurno = NumeroTurno,
                    DateTurnoCreado = DateTime.Now
                };

                await repoColaEspera.InsertAsync(colaEspera1);
                return NumeroTurno;
            }

            var LastTurnoCreated = turnosDB.Last().NumeroTurno;
            var NuevoNumeroTurno = int.Parse(Regex.Match(LastTurnoCreated, @"\d+").Value);
            Colaespera colaEspera2 = new()
            {
                NumeroTurno = "ATB-" + NuevoNumeroTurno.ToString(),
                DateTurnoCreado = DateTime.Now
            };

            await repoColaEspera.InsertAsync(colaEspera2);
            return NumeroTurno;
        }

        //Al mandar llamar actualizar tabla actualizara el dashboar de la pantalla
        //de espera, y creara un objeto Turno, el cual recibira el cajero
        public async Task<TurnoDTO> AtenderCliente(TurnoAtendiendoDTO dto)
        {
            var turnos = repoTurno.GetAll().Where(x => x.IdUsuario == dto.IdUser && x.Estado == "Atendiendo");
            if(turnos.Count() > 0)
            {
                foreach (var turno in turnos)
                {
                    turno.Estado = "Atendido";
                    await repoTurno.UpdateAsync(turno);
                }
            }

            if (dto != null)
            {
                //Aqui se crea un turno para que el cajero que lo este atendiendo le aparezca en la bd////////
                var turnoEspera = await repoColaEspera.GetTurnoAsync();
                var nombreCajaDB = repoUser.Get(dto.IdUser)?.IdCajaNavigation?.Nombre;

                Turno t = new()
                {
                    IdUsuario = dto.IdUser,
                    HoraInicial = DateTime.UtcNow,
                    HoraFinal = null,
                    NumeroTurno = turnoEspera.NumeroTurno,
                    TiempoInicio = TimeOnly.Parse(turnoEspera.DateTurnoCreado.ToLongTimeString()),
                };

                await repoTurno.InsertAsync(t);
                ///////////////////////////////////////////////////
                TurnoDTO turnoDto = new()
                {
                    Id = t.Id,
                    NumeroTurno = t.NumeroTurno
                };

                await repoColaEspera.DeleteAsync(turnoEspera);
                return turnoDto;
            }
            else
            {
                return null;
            }
        }

        //Este metodo se coloca despues del metodo AtenderCliente, lo que hace al momento de dar EMPEZAR TURNO o ABRIR CAJA
        //EL metodo de arriba crea un turno y este metodo actualiza el dashboard para tener en tiempo real los turnos y cajas que estan
        //Atendiendo
        public async Task<ActualizarTablaDTO> ActualizarTabla(ActualizarTablaDTO dto)
        {
            if (dto != null)
            {
                var turno = await repoTurno.GetTurnoByUserAsync(dto.IdCajero);

                if(turno != null)
                {
                    ActualizarTablaDTO actualizarTablaDTO = new()
                    {
                        IdCajero = dto.IdCajero,
                        NombreCaja = dto.NombreCaja,
                        NumeroTurno = turno.NumeroTurno,
                    };

                    return actualizarTablaDTO;
                }
            }
            return null;
        }

        //ESte metodo hara que al darle al boton cancelar o no se presento el cliente
        //en la tabla Turnos ese cliente aparezca en su estado que fue cancelado y/o no se presento
        public async void ClienteCancelar(int IdTurno)
        {
            if(IdTurno > 0)
            {
                var turno = await repoTurno.GetTurnoByIdAsync(IdTurno);
                if (turno != null)
                {
                    turno.Estado = "Cancelado";
                    repoTurno.UpdateAsync(turno);
                }
            }
        }

        //Este metodo hara que al cerrar el banco el Admin se limpie
        //La tabla ColaEspera
        public async void CerrarBanco()
        {
            var ColaEsperaList = repoColaEspera.GetAll();
            if(ColaEsperaList.Count() > 0)
            {
                foreach (var turno in ColaEsperaList)
                {
                    repoColaEspera.DeleteAsync(turno);
                }
            }
        }
    }
}
