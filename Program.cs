using Kanban.Model;
using Kanban.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using WebFamily.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// DbContext (MYSQL)
// ==========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) // ajuste conforme versão do MySQL no Railway
    )
);

// ==========================
// HttpClient (para serviços externos)
// ==========================
builder.Services.AddHttpClient();

// ==========================
// Serviços customizados
// ==========================
builder.Services.AddScoped<MercadoLivreTokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MercadoPagoService>(); // ✅ registra o service do Mercado Pago

// ==========================
// MVC
// ==========================
builder.Services.AddControllersWithViews();

// ==========================
// Session
// ==========================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ==========================
// Cookie Authentication
// ==========================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";        // fluxo ajustado
        options.AccessDeniedPath = "/Login/Index"; // fluxo ajustado
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

// ==========================
// APP
// ==========================
var app = builder.Build();

// ==========================
// PIPELINE
// ==========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⚠️ Ordem correta
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ==========================
// ROTAS
// ==========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente2}/{action=Cadastrar}/{id?}");

app.Run();
