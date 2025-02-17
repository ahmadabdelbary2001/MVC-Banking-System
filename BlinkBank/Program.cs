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
    options =>
    {
        options.Password.RequireUppercase = true; // يجب أن تحتوي على حرف كبير
        options.Password.RequireLowercase = true; // يجب أن تحتوي على حرف صغير
        options.Password.RequireNonAlphanumeric = true; // يجب أن تحتوي على رمز خاص
        options.Password.RequireDigit = true; // يجب أن تحتوي على رقم
        options.Password.RequiredUniqueChars = 3; // يجب أن تحتوي على 3 أحرف فريدة على الأقل
        options.Password.RequiredLength = 10; // الحد الأدنى للطول هو 10
    })
    .AddEntityFrameworkStores<BankDBContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>();
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
