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

hub.On<TurnoDto>("AtenderCliente", x =>
{
	Console.WriteLine($"Esta atendiendo a {x.NumeroTurno}");
});

await hub.StartAsync();

Console.WriteLine("Quiere abrir la caja? Si/No");
var respuesta = Console.ReadLine();

if (respuesta == "Si")
{
	await hub.InvokeAsync("AgregarCajero");
}

Console.WriteLine("Atender cliente? Si/No");
var respuestaAtender = Console.ReadLine();
if (respuesta == "Si")
{
	await hub.InvokeAsync("AtenderCliente", 2);
}

Console.ReadLine();

class TurnoDto
{
    public int Id { get; set; }
	public string NumeroTurno { get; set; } = null!;
}
