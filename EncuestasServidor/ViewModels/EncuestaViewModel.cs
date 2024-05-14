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
using System.Text.Json;
using System.IO;
using System.Collections.ObjectModel;

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

		public void Guardar()
		{
			var json = JsonSerializer.Serialize<EncuestaModel>(encuesta);
			File.WriteAllText("encuesta1.json", json);

			var json2 = JsonSerializer.Serialize<Decimal>(TotalEncuestados);
			File.WriteAllText("totalEncuestados2.json", json2);
		}


		private void Cargar()
		{
			if (File.Exists("encuesta1.json"))
			{
				var json = File.ReadAllText("encuesta1.json");
				var datos = JsonSerializer.Deserialize<EncuestaModel>(json);

				if (datos != null)
				{
					encuesta.pregunta1 = datos.pregunta1;
					encuesta.pregunta2 = datos.pregunta2;
					encuesta.pregunta3 = datos.pregunta3;
				}
				else

				{
					encuesta = new();
				}
			}

			if (File.Exists("totalEncuestados2.json"))
			{
				var json = File.ReadAllText("totalEncuestados2.json");
				var datos = JsonSerializer.Deserialize<Decimal>(json);

				if (datos != 0)
				{
					TotalEncuestados = datos;
				}
				else

				{
					TotalEncuestados = 0;
				}
			}

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(encuesta)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalEncuestados)));

		}

		public EncuestaViewModel()
        {
			server.ResultadoObtenido += Server_ResultadoObtenido;
			server.Inicar();
			Cargar();

			Preguntaunosuma = encuesta.pregunta1;
			preguntaunopromedio = TotalEncuestados == 0 ? 0 : (Preguntaunosuma / TotalEncuestados) * 35;

			Preguntadossuma = encuesta.pregunta2;
			preguntadospromedio = TotalEncuestados == 0 ? 0 : (Preguntadossuma / TotalEncuestados) * 35;

			Preguntatressuma = encuesta.pregunta3;
			preguntatrespromedio = TotalEncuestados == 0 ? 0 : (Preguntatressuma / TotalEncuestados) * 35;

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

			Guardar();

            PropertyChanged?.Invoke(this, new(nameof(encuesta)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
		public event PropertyChangedEventHandler? PropertyChanged;
    }
}
