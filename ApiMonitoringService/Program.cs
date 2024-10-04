using ApiMonitoringService.Contacts;
using ApiMonitoringService.Data;
using ApiMonitoringService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IMonitoringRepository, MonitoringRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // This maps attribute-based routes
    endpoints.MapDefaultControllerRoute(); // Optional: If you want default routing like /{controller}/{action}/{id}
});

app.Run();
