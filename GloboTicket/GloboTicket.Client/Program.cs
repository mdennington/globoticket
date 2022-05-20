using GloboTicket.Web.Models;
using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container

builder.Services.AddControllersWithViews();
var services = builder.Services;

if (builder.Environment.IsDevelopment())
    builder.Services.AddRazorPages()
        .AddRazorRuntimeCompilation();

services.AddHttpClient<IEventCatalogService, EventCatalogService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiConfigs:EventCatalog:Uri"]));
services.AddHttpClient<IShoppingBasketService, ShoppingBasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiConfigs:ShoppingBasket:Uri"]));

services.AddSingleton<Settings>();


var app = builder.Build();

// configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=EventCatalog}/{action=Index}/{id?}");
});

//app.MapControllers();


app.Run();
