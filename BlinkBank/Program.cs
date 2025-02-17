using BlinkBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BankDBContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    option =>
    {
        option.Password.RequireUppercase = false;
        option.Password.RequiredUniqueChars = 0;
        option.Password.RequiredLength = 8;
        option.Password.RequireNonAlphanumeric = false;
        option.Password.RequireLowercase = false;
    }
    )
    .AddEntityFrameworkStores<BankDBContext>().AddDefaultTokenProviders().AddRoles<IdentityRole>();
builder.Services.AddAuthentication().AddCookie("cookie");
builder.Services.AddAuthorization();

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
