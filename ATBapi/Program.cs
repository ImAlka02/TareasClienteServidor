using ATBapi.Models.Entities;
using ATBapi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddTransient<ColaEsperaRepository>();
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
app.UseStaticFiles();
app.MapControllers();

app.Run();

