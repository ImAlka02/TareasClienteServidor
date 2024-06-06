using Microsoft.AspNetCore.SignalR.Client;


HubConnection hub = new HubConnectionBuilder()
				.WithUrl("https://localhost:7069/tickets")
				.WithAutomaticReconnect()
				.Build();

hub.On<string>("GenerarTicket", x =>
{
	
	Console.WriteLine(x);
});

await hub.StartAsync();

Console.WriteLine("Quiere generar ticket? Si/No");
var respuesta = Console.ReadLine();

if(respuesta == "Si") 
{
	await hub.InvokeAsync("GenerarTicket");
}

Console.ReadLine();
