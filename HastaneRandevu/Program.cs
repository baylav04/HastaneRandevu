using HastaneRandevu.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper; // 🔹 Bunu da eklemeyi unutma!

var builder = WebApplication.CreateBuilder(args);

// 🔹 Veritabanı bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new ArgumentNullException(nameof(args));
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(connectionString));

// 🔹 AutoMapper'ı tanıt
builder.Services.AddAutoMapper(typeof(Program));

// 🔹 Session'ı etkinleştir
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 🔹 Controller ve View'lar
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Hata ayıklama ve HSTS ayarı
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Veritabanı başlatma işlemi
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
        InitializeDatabase.Initialize(dbContext);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 🔹 Session'ı kullan
app.UseSession();

// 🔹 Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

