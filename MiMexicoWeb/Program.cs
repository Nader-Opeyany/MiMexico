
using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Data;
using MiMexicoWeb.Dbintializer;
using MiMexicoWeb.Models;
using Stripe;
//SMS Imports
using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDBContext>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

//var accountSid = Configuration.GetValue<string>("AppSettings:Twilio:AccountSID");
//var authToken = Configuration.GetValue<string>("AppSettings:Twilio:AuthToken");

//TwilioClient.Init(accountSid, authToken);
//var TwilioAuthToken = builder.Configuration.GetSection("Twilio:AuthToken").Get<string>();
SeedDatabase();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Landing}/{id?}");

app.Run();

void SeedDatabase()
{
    using(var scope = app.Services.CreateScope())
    {
        // Proivdes implementation inside of the dbInitializer var
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        // Seed Database
        dbInitializer.Initialize();
    }
}
