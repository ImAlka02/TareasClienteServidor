using ApiActividades.Models.Entities;
using ApiActividades.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ItesrcneActividadesContext>(x => x.UseMySql("server=204.93.216.11;database=itesrcne_actividades;user=itesrcne_deptos;password=sistemaregistrotec24", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.29-mariadb")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {


        options.Audience = "ActividadesApp";
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes("PROGRAMACIONCLIENTESERVIDOR_2024OPORDIOS"));
        options.TokenValidationParameters.ValidIssuer = "ApiActividades";

        //ValidateIssuer = true,
        //ValidateAudience = true,
        //ValidateLifetime = true, //No se que haga, PREGUNTAR
        //ValidateIssuerSigningKey = true, //PREGUNTAR
        //ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //ValidAudience = builder.Configuration["Jwt:Audience"],
        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "CLIENTESERVIDOR2024"))
    
    });

builder.Services.AddTransient<ActividadesRepository>();
builder.Services.AddTransient<DepartamentoRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
