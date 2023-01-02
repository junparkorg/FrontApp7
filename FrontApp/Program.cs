using FrontApp.Data;
using FrontApp.Util;
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
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

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
