using CardCraze.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CardCrazeDb");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CardCrazeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CardCrazeDb"))
);

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapDefaultControllerRoute();

FakeCardData.LoadData(app);

app.Run();
