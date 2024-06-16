using Microsoft.AspNetCore.SignalR.Client;

HubConnection hub = new HubConnectionBuilder()
				.WithUrl("https://localhost:7069/tickets")
				.WithAutomaticReconnect()
				.Build();

hub.On<string>("AgregarCajero", x =>
{
	Console.WriteLine(x);
});

hub.On<string>("GenerarTicket", x =>
{
	Console.WriteLine($"Hay clientes esperando. {x}");
});


string turno = "";
hub.On<TurnoDto>("AtenderCliente", x =>
{
    turno = x.NumeroTurno;
	Console.WriteLine($"Esta atendiendo a {x.NumeroTurno}");
});

hub.On<string>("ClienteCancelar", x =>
{
    Console.WriteLine($"Se cancelo el turno {x}");
});

await hub.StartAsync();

Console.WriteLine("Quiere abrir la caja? Si/No");
var respuesta = Console.ReadLine();

if (respuesta == "Si")
{
	await hub.InvokeAsync("AgregarCajero");
}

while (true)
{
    Console.WriteLine("Atender cliente? Si/No");
    var respuestaAtender = Console.ReadLine();
    if (respuesta == "Si")
    {
        await hub.InvokeAsync("AtenderCliente", 2);
        await hub.InvokeAsync("ActualizarTabla", new ActualizarTablaDTO()
        {
            IdCajero = 2,
            NombreCaja = "1",
            NumeroTurno = turno
        });
    }

    Console.WriteLine("Cancelar turno? Si/No");
    var respuestaCancelar = Console.ReadLine();
    if(respuestaCancelar == "Si")
    {
        await hub.InvokeAsync("ClienteCancelar", turno);
    }

    Console.ReadLine();

}


class TurnoDto
{
    public int Id { get; set; }
	public string NumeroTurno { get; set; } = null!;
}

public class ActualizarTablaDTO
{
    public int IdCajero { get; set; }
    public string NombreCaja { get; set; } = string.Empty;
    public string NumeroTurno { get; set; } = string.Empty;
}