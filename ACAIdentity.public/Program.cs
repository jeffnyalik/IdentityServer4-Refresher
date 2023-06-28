using ACAIdentity;
using ACAIdentity.Data;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NLog.Fluent;
using Serilog;
using System.Reflection;

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}



var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString, opt => opt.MigrationsAssembly(migrationAssembly));
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

if (seed)
{
    //Log.Information("Seeding database...");
    Console.WriteLine("Seeding database...");
    SeedData.EnsureSeedData(connectionString);
    Console.WriteLine("Done seeding database.");
    //Log.Information("Done seeding database.");
}

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = builder => builder.UseSqlite(connectionString, opt => opt.MigrationsAssembly(migrationAssembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = builder => builder.UseSqlite(connectionString, opt => opt.MigrationsAssembly(migrationAssembly));
    })
    .AddDeveloperSigningCredential();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(options => options.MapDefaultControllerRoute());

app.Run();
