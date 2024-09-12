using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UTR_WebApplication.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UtrContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UtrContext") ??
    throw new InvalidOperationException("Connection string 'UtrContext' not found.")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
        options.SlidingExpiration = true; 
    });

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
