using WorkSchedule.AplicationStartup.RabbitMq;
using WorkSchedule.Mappers;
using WorkSchedule.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<ChromosomeMapper>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar RabbitMQ
new RabbitMqStartUp().ConfigureServices(builder);

// Registrar o Background Service para consumir a fila RabbitMQ
builder.Services.AddHostedService<WorkScheduleBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
