using HastaneRandevu.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using HastaneRandevu.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new ArgumentNullException(nameof(args));

// Context, IdentityDbContext'ten türediği için hem kimlik hem uygulama verilerini tutar
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<HastaneRandevuUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>() // Rol desteği
.AddEntityFrameworkStores<Context>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // <- EKLENMESİ GEREKİR
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<HastaneRandevuUser>>();

    SeedAdminUserAndRoleAsync(userManager, roleManager).GetAwaiter().GetResult();
}

async Task SeedAdminUserAndRoleAsync(UserManager<HastaneRandevuUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminRoleName = "Admin";
    const string adminEmail = "admin@admin.com";
    const string adminPassword = "Admin123!";

    // Rolü oluştur
    if (!await roleManager.RoleExistsAsync(adminRoleName))
        await roleManager.CreateAsync(new IdentityRole(adminRoleName));

    // Admin kullanıcısını bul ya da oluştur
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new HastaneRandevuUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
            throw new Exception("Admin kullanıcısı oluşturulamadı.");
    }

    // Kullanıcıyı Admin rolüne ata
    if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
        await userManager.AddToRoleAsync(adminUser, adminRoleName);
}
app.Run();

