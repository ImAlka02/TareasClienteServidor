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

namespace EncuestasServidor.ViewModels
{
	public partial class EncuestaViewModel : INotifyPropertyChanged
	{
		EncuestaServer server = new();

		public int TotalEncuestados { get; set; } = 1;
		public decimal Preguntaunosuma { get; set; } = 1;
		private decimal Preguntaunopromedio;

		public decimal preguntaunopromedio
		{
			get { return (Preguntaunopromedio / Preguntadossuma )*40; }
			set { Preguntaunopromedio = value; }
		}

		public decimal Preguntadossuma { get; set; } = 1;

		private decimal Preguntadospromedio;

		public decimal preguntadospromedio
		{
			get { return (Preguntadospromedio/ TotalEncuestados)*40; }
			set { Preguntadospromedio = value; }
		}


		public decimal Preguntatressuma { get; set; } = 1;
		private decimal Preguntatrespromedio;

		public decimal preguntatrespromedio
		{
			get { return (Preguntatrespromedio/TotalEncuestados)*40; }
			set { Preguntatrespromedio = value; }
		}



		public EncuestaModel encuesta { get; set; } = new();


        public EncuestaViewModel()
        {
			server.ResultadoObtenido += Server_ResultadoObtenido;
			server.Inicar();
        }


		private void Server_ResultadoObtenido(object? sender, Models.EncuestaModel e)
		{
			encuesta.pregunta1 = encuesta.pregunta1 + e.pregunta1;
			Preguntaunosuma = encuesta.pregunta1;
			encuesta.pregunta2 = encuesta.pregunta2 + e.pregunta2;
			Preguntadossuma = encuesta.pregunta2;
			encuesta.pregunta3 = encuesta.pregunta3 + e.pregunta3;
			Preguntatressuma = encuesta.pregunta3;
			TotalEncuestados++;

			PropertyChanged?.Invoke(this, new(nameof(encuesta)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
		public event PropertyChangedEventHandler? PropertyChanged;
    }
}
