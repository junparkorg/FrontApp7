using FrontApp.Data;
using FrontApp.Util;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Logging;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// App insight - telemetry
builder.Services.AddTransient<ICacheTelemetry, WebCacheTelemetry>();

// Redis Service 
string redisConnectionString = builder.Configuration["ConnectionStrings:Redis"];
builder.Services.AddSingleton<IRedisService, RedisService>();
ConnectionMultiplexer multiPlex = ConnectionMultiplexer.Connect(redisConnectionString);
if (multiPlex != null)
{
    IDatabase database = multiPlex.GetDatabase();
    builder.Services.AddSingleton(_ => database);
}

var appinsightsConnection = builder.Configuration["ApplicationInsights:ConnectionString"];

var options = new ApplicationInsightsServiceOptions { ConnectionString = appinsightsConnection };
builder.Services.AddApplicationInsightsTelemetry(options: options);

// add app insights logging
builder.Logging.AddApplicationInsights(
        configureTelemetryConfiguration: (config) =>
            config.ConnectionString = appinsightsConnection,
            configureApplicationInsightsLoggerOptions: (options) => { }
);
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Category", LogLevel.Trace);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// prometheus metrics 
app.UseMetricServer();


//app.UseHttpMetrics();
app.UseRequestMiddleware();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
