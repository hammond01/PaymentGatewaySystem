using MediatR;
using PaymentGateway.Application.Features.Merchants.Handlers;
using PaymentGateway.Application.Features.Merchants.Queries;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Repositories.VNPayRestful;
using PaymentGateway.Domain.Repositories.VNPaySandBox;
using PaymentGateway.Infrastructure.Repositories;
using PaymentGateway.Infrastructure.VNPaySandBox.Repository;
using PaymentGateway.Persistence.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;
using PaymentGateway.Ultils.Loggers;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console().MinimumLevel.Information();
    config.WriteTo.File(
        path: "Log/Logger.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        formatter: new JsonFormatter()).MinimumLevel.Information();
});
builder.Services.AddControllers(options => { options.Filters.Add<LoggingActionFilter>(); })
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
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Helpers>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IDataAccess, DataAccess>();
builder.Services.AddScoped<IVnPayServices, VnPayServices>();
builder.Services.AddScoped<IVNPaySandBoxServices, VnPaySandBoxRepository>();
builder.Services.AddScoped<IMerchantService, MerchantRepository>();
builder.Services.AddScoped<IAuditServices, AuditRepository>();
builder.Services.AddScoped<IPaymentTransactionService, PaymentTransactionRepository>();
builder.Services.AddScoped<ITransactionCodeService, TransactionCodeRepository>();
builder.Services.AddScoped<IDetailTransactionServices, DetailTransactionRepository>();
builder.Services.AddScoped<IDetailPaymentService, DetailPaymentRepository>();

//add mediatR
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(typeof(Program).Assembly));

builder.Services.AddTransient<IRequestHandler<GetAllMerchantsQuery, BaseResultWithData<List<GetMerchantModel>>>, GetAllMerchantsHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();