using DragonVu.Data;
using DragonVu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found."); ;

//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer("MyConnection"));
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
builder.Configuration.GetConnectionString("MyConnection")));
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Lockout.AllowedForNewUsers = false;
    // ÿÊ· ﬂ·„… «·„—Ê—
    options.Password.RequiredLength = 6;

    // «·‘—Êÿ
    options.Password.RequireDigit = false;          // —ﬁ„
    options.Password.RequireUppercase = false;      // Õ—› ﬂ»Ì—
    options.Password.RequireLowercase = false;       // Õ—› ’€Ì—
    options.Password.RequireNonAlphanumeric = false; // —„“ Œ«’

    // ⁄œœ «·„Õ«Ê·«  «·›«‘·… («Œ Ì«—Ì)
    options.Lockout.MaxFailedAccessAttempts = 20;

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();


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

app.MapStaticAssets();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.Run();
