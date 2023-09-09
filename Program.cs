//Scaffold-DbContext "Server=.;Database=AppnetworkCEOdb;Trusted_connection=true;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force

using AppnetworkCEO2023.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    //options.Cookie.HttpOnly = true;
    //options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppnetworkCeodbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();




app.MapControllerRoute(
    name: "ceo",
    pattern: "dashboard/ceomember",
    defaults: new { controller ="Dashboard", action = "Ceoindex" });
app.MapControllerRoute(
    name: "shareholder",
    pattern: "dashboard/shareholder",
    defaults: new { controller ="Dashboard", action = "Shareholderindex" }); 


 app.MapControllerRoute(
    name: "register",
    pattern: "registration/{regtype}/{formtype?}",
    defaults: new { controller ="Home", action = "Registration" });

app.MapControllerRoute(
    name: "AutologAuth",
    pattern: "AutologAuth/{anpd}",
    defaults: new { controller = "Home", action = "AutologAuth" });

app.MapControllerRoute(
    name: "home",
    pattern: "home",
    defaults: new { controller = "Home", action = "index" });

app.MapControllerRoute(
   name: "default",
   pattern: "",
   defaults: new { controller = "Home", action = "Redirector" });

app.MapControllerRoute(
    name: "default2",
    pattern: "{controller=Home}/{action=index}");//{ urlVal ?}

app.Run();


//app.MapControllerRoute(
//    name: "register",
//    pattern: "registration/{Type:int?}",
//    defaults: new { controller = "Home", action = "Registration" });