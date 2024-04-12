using CommunityToolkit.Mvvm.ComponentModel;
using EncuestasServidor.Models;
using EncuestasServidor.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Navigation;
using System.Net;

namespace EncuestasServidor.ViewModels
{
	public partial class EncuestaViewModel : INotifyPropertyChanged
	{
		EncuestaServer server = new();

		public string IP
		{
			get
			{
				return string.Join(",", Dns.GetHostAddresses(Dns.GetHostName()).
					Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					.Select(x => x.ToString()));
			}
		}

		public decimal TotalEncuestados { get; set; } 
		public decimal Preguntaunosuma { get; set; }

		public decimal Preguntadossuma { get; set; } 
		public decimal preguntaunopromedio { get; set; }

		public decimal preguntadospromedio
		{
			get;set;
		}


		public decimal Preguntatressuma { get; set; } 

		public decimal preguntatrespromedio
		{
			get;set;
		}



		public EncuestaModel encuesta { get; set; } = new();


        public EncuestaViewModel()
        {
			server.ResultadoObtenido += Server_ResultadoObtenido;
			server.Inicar();
        }


		private void Server_ResultadoObtenido(object? sender, Models.EncuestaModel e)
		{
			TotalEncuestados++;
			encuesta.pregunta1 = encuesta.pregunta1 + e.pregunta1;

			Preguntaunosuma = encuesta.pregunta1;	
			preguntaunopromedio =  TotalEncuestados == 0 ? 0 : (Preguntaunosuma/TotalEncuestados)*35; 

            encuesta.pregunta2 = encuesta.pregunta2 + e.pregunta2;
			Preguntadossuma = encuesta.pregunta2;
			preguntadospromedio = TotalEncuestados == 0 ? 0 : (Preguntadossuma/TotalEncuestados)*35;

            encuesta.pregunta3 = encuesta.pregunta3 + e.pregunta3;
			 Preguntatressuma = encuesta.pregunta3;
			preguntatrespromedio = TotalEncuestados == 0 ? 0 : (Preguntatressuma/ TotalEncuestados)*35;


            PropertyChanged?.Invoke(this, new(nameof(encuesta)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
		public event PropertyChangedEventHandler? PropertyChanged;
    }
}
