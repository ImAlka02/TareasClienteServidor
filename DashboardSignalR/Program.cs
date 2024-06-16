using Microsoft.AspNetCore.SignalR.Client;


HubConnection hub = new HubConnectionBuilder()
                .WithUrl("https://localhost:7069/tickets")
                .WithAutomaticReconnect()
                .Build();

hub.On<ActualizarTablaDTO>("ActualizarTabla", x =>
{
    Console.WriteLine($"Caja {x.NombreCaja} Turno {x.NumeroTurno}");
});

await hub.StartAsync();

while (true)
{
    Console.ReadLine();
}


public class ActualizarTablaDTO
{
    public int IdCajero { get; set; }
    public string NombreCaja { get; set; } = string.Empty;
    public string NumeroTurno { get; set; } = string.Empty;
}