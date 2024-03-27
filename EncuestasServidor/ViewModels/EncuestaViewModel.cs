using CommunityToolkit.Mvvm.ComponentModel;
using EncuestasServidor.Models;
using EncuestasServidor.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncuestasServidor.ViewModels
{
	public partial class EncuestaViewModel : INotifyPropertyChanged
	{
		EncuestaServer server = new();


		public EncuestaModel encuesta { get; set; } = new();


        public EncuestaViewModel()
        {
			server.ResultadoObtenido += Server_ResultadoObtenido;
			server.Inicar();
        }


		private void Server_ResultadoObtenido(object? sender, Models.EncuestaModel e)
		{
			encuesta.pregunta1 = encuesta.pregunta1 + e.pregunta1;
			encuesta.pregunta2 = encuesta.pregunta2 + e.pregunta2;
			encuesta.pregunta3 = encuesta.pregunta3 + e.pregunta3;

			PropertyChanged?.Invoke(this, new(nameof(encuesta)));

		}
		public event PropertyChangedEventHandler? PropertyChanged;

	}
}
