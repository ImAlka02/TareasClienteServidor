using ATBapi.Hubs;
using ATBapi.Models.Entities;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .WithOrigins("https://cajas.labsystec.net","https://adminsmt.labsystec.net","http://localhost:4321"); ;
        });
});

builder.Services.AddDbContext<atbContext>(x => x.UseMySql("server=websitos256.com;database=websitos_atb;user=websitos_atb;password=1h70ak^B4",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.7-mariadb")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Audience = "ATBapp";
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes("PROGRAMACIONCLIENTESERVIDOR_2024OPORDIOS"));
        options.TokenValidationParameters.ValidIssuer = "ATBapi";
    });

builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<CajaRepository>();
builder.Services.AddTransient<TurnoRepository>();
builder.Services.AddTransient<ColaEsperaRepository>();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => { x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials().WithOrigins("https://cajas.labsystec.net","https://adminsmt.labsystec.net","http://localhost:4321","https://clientsm.labsystec.net"); });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.MapHub<TicketsHub>("/tickets");

app.Run();

