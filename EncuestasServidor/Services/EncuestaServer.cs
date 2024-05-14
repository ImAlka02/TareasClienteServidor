using EncuestasServidor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace EncuestasServidor.Services
{
	public class EncuestaServer
	{
		HttpListener server = new();

        public EncuestaServer()
        {
            server.Prefixes.Add("http://*:7777/encuesta/");
        }

        public void Inicar()
        {
            if(!server.IsListening)
            {
                server.Start();

                new Thread(Escuchar)
                {
                    IsBackground = true
                }.Start();
            }
        }

        public void Detener()
        {
            server.Stop();  
        }

        public event EventHandler<EncuestaModel>? ResultadoObtenido;

        void Escuchar()
        {
            while(true)
            {
                var context = server.GetContext();
				var pagina = File.ReadAllText("Assets/Index.html"); //C:\\Users\\ajiju\\source\\repos\\TareasClienteServidor\\EncuestasServidor\\Assets\\Index.html
                var bufferPagina = Encoding.UTF8.GetBytes(pagina);


				var paginaGracias = File.ReadAllText("Assets/Gracias.html");
				var bufferPaginaGracias = Encoding.UTF8.GetBytes(paginaGracias);


				if (context.Request.Url != null)
                {
                    if (context.Request.Url.LocalPath == "/encuesta/")
                    {
                        context.Response.ContentLength64 = bufferPagina.Length;
                        context.Response.OutputStream.Write(bufferPagina, 0, bufferPagina.Length);
                        context.Response.StatusCode = 200;
                        context.Response.Close();
                    }
                    else if (context.Request.HttpMethod == "POST" &&
                        context.Request.Url.LocalPath == "/encuesta/enviado")
                    {
						

						byte[] bufferDatos = new byte[context.Request.ContentLength64];
                        context.Request.InputStream.Read(bufferDatos, 0, bufferDatos.Length);
                        string datos = Encoding.UTF8.GetString(bufferDatos);

						context.Response.ContentLength64 = bufferPaginaGracias.Length;
						context.Response.OutputStream.Write(bufferPaginaGracias, 0, bufferPaginaGracias.Length);
						context.Response.StatusCode = 200;

						var diccionario = HttpUtility.ParseQueryString(datos);

                        EncuestaModel encuesta = new EncuestaModel()
                        {
                            pregunta1 = int.Parse(diccionario["pregunta1"]),
                            pregunta2 = int.Parse(diccionario["pregunta2"]),
                            pregunta3 = int.Parse(diccionario["pregunta3"])
                        };

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ResultadoObtenido?.Invoke(this, encuesta);
                        });

                        context.Response.StatusCode = 200;
                        context.Response.Close();
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                    }
                }
			}
        }
    }
}
