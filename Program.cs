using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebAPIAngular.Models;
using WebAPIAngular.wwwroot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GamesContext>(
    opciones =>  opciones.UseNpgsql("Host=localhost;Database=games;UserName=admin_gemes;Password=admin123")
    //opciones => opciones.UseNpgsql("name=ConnectionStrings:postgresConnection")
);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAlmacenamiento, Almacenamiento>();
builder.Services.AddControllers().AddJsonOptions(
    opciones =>
    {
        // opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        // opciones.JsonSerializerOptions.WriteIndented = true;
        opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    }
);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(
    opciones =>
    {
        opciones.AddPolicy(name: "Default", policy =>
        {
            policy.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
    }
);
var app = builder.Build();

// Configure the HTTP request pipeline.clar 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("Default");

app.UseAuthorization();

app.MapControllers();

app.Run();
