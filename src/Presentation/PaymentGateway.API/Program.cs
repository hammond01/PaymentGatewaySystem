using PaymentGateway.Domain.Entities.ThirdParty;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Infrastructure.Repositories;
using PaymentGateway.Ultils.Extension;
using PaymentGateway.Ultils.Loggers;
using Serilog;
using Serilog.Formatting.Json;
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((hm, config) =>
{
    config.WriteTo.Console().MinimumLevel.Information();
    config.WriteTo.File(
        path: "Log/Logger.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        formatter: new JsonFormatter()).MinimumLevel.Information();
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add<LoggingActionFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Loại bỏ log của ModelStateInvalidFilter
    options.SuppressModelStateInvalidFilter = true;
});
//Register ILogger and LoggerFactory
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Helpers>();
builder.Services.AddSingleton<CreateQR>();
builder.Services.AddScoped<IVNPayservices, VNPayservices>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
