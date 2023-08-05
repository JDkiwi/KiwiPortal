using KiwiPortal;
using KiwiPortal.Models;
using KiwiPortal.Services;
using ConfigurationProvider = KiwiPortal.Services.ConfigurationProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAutoMapper(typeof(ModelMappingProfile));
builder.Services.AddSingleton<ConfigurationProvider>();
builder.Services.AddSingleton<EnergyConsumptionService>();
builder.Services.AddSingleton<AwattarService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var configurationProvider = app.Services.GetService<ConfigurationProvider>();

configurationProvider?.LoadConfiguration();

app.Run();
