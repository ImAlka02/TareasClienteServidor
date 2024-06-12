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

        public async void AgregarCajero() //FUNCIONA
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Cajeros");
			await Clients.Caller.SendAsync("AgregarCajero","Se agrego al grupo de Cajeros.");
		}
        //Este metodo genera un ticket el cual aparecera en la pantalla
        //luego lo manda a la cola de espera en la base de datos esperando
        //A que un cajero lo reciba
        public async void GenerarTicket() //FUNCIONA
        {
            var NumeroTurno = "ATB-0001";
            var colaEsperaList = repoColaEspera.GetAll(); //TRAE TODA LA COLA DE ESPERA
            var turnosDB =  repoTurno.GetAll().Where(x => x.HoraInicial.Date == DateTime.Now.Date).ToList();
            if (turnosDB.Count() != 0)
            {
                if (colaEsperaList.Count() > 0)
                {
                    
                        int n = int.Parse(colaEsperaList.Last().NumeroTurno.Substring(4, 4)) + 1;
                        NumeroTurno = "ATB-" + n.ToString("0000");
                        Colaespera colaEspera = new()
                        {
                            NumeroTurno = NumeroTurno,
                            DateTurnoCreado = DateTime.Now
                        };
                        repoColaEspera.Insert(colaEspera);
                        await Clients.Caller.SendAsync("GenerarTicket", colaEspera.NumeroTurno);
                        await Clients.Groups("Cajeros").SendAsync("GenerarTicket", colaEspera.NumeroTurno);
                    
                }
                else
                {
					Colaespera colaEspera1 = new()
					{
						NumeroTurno = NumeroTurno,
						DateTurnoCreado = DateTime.Now
					};

					repoColaEspera.Insert(colaEspera1);
					await Clients.Caller.SendAsync("GenerarTicket", colaEspera1.NumeroTurno);
					await Clients.Groups("Cajeros").SendAsync("GenerarTicket", colaEspera1.NumeroTurno);
				}
                
            }
            else
            {


                var LastTurnoCreated = turnosDB.LastOrDefault().NumeroTurno;
                var NuevoNumeroTurno = int.Parse(Regex.Match(LastTurnoCreated, @"\d+").Value) + 1;
                Colaespera colaEspera2 = new()
                {
                    NumeroTurno = "ATB-" + NuevoNumeroTurno.ToString("0000"),
                    DateTurnoCreado = DateTime.Now
                };

                repoColaEspera.Insert(colaEspera2);
                await Clients.Caller.SendAsync("GenerarTicket", colaEspera2.NumeroTurno);
                await Clients.Groups("Cajeros").SendAsync("GenerarTicket", colaEspera2.NumeroTurno);
            }
		}

        //Al mandar llamar actualizar tabla actualizara el dashboar de la pantalla
        //de espera, y creara un objeto Turno, el cual recibira el cajero
        public async void AtenderCliente(int IdCajero) //Funciona
        {
            var turnos = repoTurno.GetAll().Where(x => x.IdUsuario == IdCajero && x.Estado == "Atendiendo").ToList();
            if(turnos.Count() > 0)
            {
                foreach (var turno in turnos)
                {
                    turno.Estado = "Atendido";
                    repoTurno.Update(turno);
                }
            }

            
                //Aqui se crea un turno para que el cajero que lo este atendiendo le aparezca en la bd////////
                var turnoEspera = repoColaEspera.GetFirstTurno();

                Turno t = new()
                {
                    IdUsuario = IdCajero,
                    HoraInicial = DateTime.Now,
                    HoraFinal = null,
                    NumeroTurno = turnoEspera.NumeroTurno,
                    TiempoInicio = TimeOnly.Parse(turnoEspera.DateTurnoCreado.ToLongTimeString())
                };

                repoTurno.Insert(t);
                ///////////////////////////////////////////////////
                TurnoDTO turnoDto = new()
                {
                    Id = t.Id,
                    NumeroTurno = t.NumeroTurno
                };

                repoColaEspera.Delete(turnoEspera);
                await Clients.Groups("Cajeros").SendAsync("AtenderCliente", turnoDto);
        }

        //Este metodo se coloca despues del metodo AtenderCliente, lo que hace al momento de dar EMPEZAR TURNO o ABRIR CAJA
        //EL metodo de arriba crea un turno y este metodo actualiza el dashboard para tener en tiempo real los turnos y cajas que estan
        //Atendiendo
        public async void ActualizarTabla(ActualizarTablaDTO dto)
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

                    await Clients.Caller.SendAsync("ActualizarTabla", actualizarTablaDTO);
                }
            }
			await Clients.Caller.SendAsync("ActualizarTabla", "No hay clientes");
		}

		//ESte metodo hara que al darle al boton cancelar o no se presento el cliente
		//en la tabla Turnos ese cliente aparezca en su estado que fue cancelado y/o no se presento
		public async void ClienteCancelar(int IdTurno, int IdCajero)
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
			//Pasar al siguiente cliente
			var turnoEspera = repoColaEspera.GetFirstTurno();

			Turno t = new()
			{
				IdUsuario = IdCajero,
				HoraInicial = DateTime.Now,
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
			await Clients.Groups("Cajeros").SendAsync("GenerarTicket", turnoDto);
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
