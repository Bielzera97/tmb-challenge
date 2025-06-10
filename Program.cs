using PedidoApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<PedidoContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Habilita CORS para qualquer origem
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// swagger com informações customizadas
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Pedidos",
        Version = "v1",
        Description = "API para gerenciamento de pedidos (exemplo com ASP.NET Core, PostgreSQL e Swagger).",
        Contact = new OpenApiContact
        {
            Name = "Seu Nome",
            Email = "seu@email.com"
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Pedidos v1");
        options.RoutePrefix = "docs";
    });
}


// Aplica o CORS para qualquer origem
app.UseCors("AllowAll");

app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
