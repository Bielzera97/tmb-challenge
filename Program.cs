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
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "http://localhost:3000",
                "https://tmb-frontend-five.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
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

builder.Services.AddSignalR(); // Adicione esta linha

var app = builder.Build();

// Garante que as migrations são aplicadas ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PedidoContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Pedidos v1");
        options.RoutePrefix = "docs";
    });
}

// Aplica o CORS para o front-end
app.UseCors("AllowFrontend");

app.MapControllers();
app.MapHub<PedidoApi.Hubs.PedidoHub>("/hub/pedidos"); // Adicione esta linha

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
