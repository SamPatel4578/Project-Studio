using Microsoft.EntityFrameworkCore;
using UTR_WebApplication.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UtrContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UtrContext") ??
    throw new InvalidOperationException("Connection string 'UtrContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
