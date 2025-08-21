using FONEXPO2024.DataAccess;
using FONEXPO2024.Interfaces.Interfaces;
using FONEXPO2024.Services.Sevices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fon Expo 2024 API", Version = "v1" });
    }
    );


// Configure DbContext with SQL Server
builder.Services.AddDbContext<FonExpoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IManifestacijaService, ManifestacijaService>();
builder.Services.AddScoped<IRegistracijaService, RegistracijaService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin() 
            .AllowAnyMethod()
            .AllowAnyHeader()
            );
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fon Expo 2024 API"));
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");


app.UseAuthorization();

app.MapControllers();

app.Run();
