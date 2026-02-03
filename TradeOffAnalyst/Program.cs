using Infrastructure;
using Microsoft.Extensions.Logging.Console;
using Observability;
using Services;
using TradeOffAnalyst;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Observability
builder.Services.AddSingleton<ICorrelationContext, CorrelationContext>();
builder.Services.AddSingleton<IAuditLogger, ConsoleAuditLogger>();

// Infrastructure
builder.Services.AddSingleton<ILeaveRepository, InMemoryLeaveRepository>();
builder.Services.AddSingleton<IPdfGenerator, FakePdfGenerator>();

// Services
builder.Services.AddSingleton<ILeaveService, LeaveService>();

// Built-in logging
// builder.Logging.ClearProviders();
// builder.Logging.AddSimpleConsole(options =>
// {
//     options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
//     options.IncludeScopes = false;
// });

builder.Logging.ClearProviders();

builder.Services.AddSingleton<ConsoleFormatter, CorrelationOnlyConsoleFormatter>();

builder.Logging.AddConsole(o =>
{
    o.FormatterName = "corr";
    o.IncludeScopes = true;
});

var app = builder.Build();

//middleware
app.UseMiddleware<CorrelationIdMiddleware>();

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
