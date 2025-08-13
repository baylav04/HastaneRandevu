using AspNetCoreHero.ToastNotification;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using HastaneRandevu.Areas.Identity.Data;
using HastaneRandevu.Data;
using HastaneRandevu.Middlewares;
using HastaneRandevu.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Email servisi kaydı (SmtpEmailSender kaldırıldı, sadece EmailService var)
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailSender, EmailService>();

// Veritabanı bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new ArgumentNullException("DefaultConnection yok!");

// DbContext kaydı
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(connectionString));

// Identity yapılandırması
builder.Services.AddDefaultIdentity<HastaneRandevuUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<Context>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Toast Notification
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

// Session ve Cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// FluentValidation
builder.Services.AddControllersWithViews();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// API Validation hata formatı
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .Select(e => new
            {
                Field = e.Key,
                Errors = e.Value.Errors.Select(x => x.ErrorMessage)
            });

        return new BadRequestObjectResult(new
        {
            Message = "Validation failed",
            Errors = errors
        });
    };
});

builder.Services.AddRazorPages();

// Hangfire servisleri
builder.Services.AddHangfire(cfg =>
    cfg.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

// Randevu hatırlatma işi
builder.Services.AddScoped<RandevuHatirlatmaJob>();

var app = builder.Build();

// Ortam kontrolü
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware’ler
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRouting();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Hangfire dashboard (isteğe göre sadece admin’e aç)
app.UseHangfireDashboard("/hangfire");

// Route’lar
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Initialize
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Context>();
    InitializeDatabase.Initialize(context);
}

// Rol ve admin kullanıcı seed işlemi
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<HastaneRandevuUser>>();

    await SeedAdminUserAndRoleAsync(userManager, roleManager);
}

async Task SeedAdminUserAndRoleAsync(UserManager<HastaneRandevuUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminRoleName = "Admin";
    const string adminEmail = "admin@admin.com";
    const string adminPassword = "Admin123!";

    if (!await roleManager.RoleExistsAsync(adminRoleName))
        await roleManager.CreateAsync(new IdentityRole(adminRoleName));

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
            throw new Exception("Admin kullanıcısı oluşturulamadı: " + string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
        await userManager.AddToRoleAsync(adminUser, adminRoleName);
}

// Hangfire tekrarlayan iş planlama
RecurringJob.AddOrUpdate<RandevuHatirlatmaJob>(
    "randevu-hatirlat",
    job => job.GonderAsync(),
    "0 * * * *"
);

app.Run();


