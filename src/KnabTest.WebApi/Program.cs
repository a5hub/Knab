using KnabTest.CoinmarketcapApiClient.Extensions;
using KnabTest.CoinmarketcapApiClient.Options;
using KnabTest.Common.ApiClient.ApiPolicies;
using KnabTest.ExchangeRates.ApiClient.Extensions;
using KnabTest.ExchangeRates.ApiClient.Options;
using KnabTest.Logic.Services;
using KnabTest.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureServices((context, services) =>
{
    var configurationRoot = context.Configuration;

    // Configuration
    services.Configure<CoinmarketcapClientOptions>(configurationRoot.GetSection(CoinmarketcapClientOptions.Key));
    services.Configure<ExchangeRatesClientOptions>(configurationRoot.GetSection(ExchangeRatesClientOptions.Key));
    services.Configure<ApiPolicyOptions>(configurationRoot.GetSection(ApiPolicyOptions.Key));

    // Services
    services.AddTransient<IExchangeService, ExchangeService>();
    services.AddTransient<ICryptocurrencyService, CryptocurrencyService>();
    services.AddTransient<ICurrencyService, CurrencyService>();
    services.AddTransient<IApiPolicies, ApiPolicies>();
    
    // Other
    services.AddMemoryCache();
});

// Serilog
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCoinmarketcapApiServices();
builder.Services.AddExchangeRatesApiServices();

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

app.UseMiddleware<ExceptionMiddleware>();

app.Run();