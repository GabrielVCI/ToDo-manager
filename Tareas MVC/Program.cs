using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json.Serialization;
using Tareas_MVC;
using Tareas_MVC.Servicios;

var builder = WebApplication.CreateBuilder(args);

//Politica de autenticacion, quiere decir que los usuarios deben estar autenticados para acceder a la aplicacion
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
}).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<ApplicationDbContext>
    (opciones => opciones.UseSqlServer("name=DefaultConnection"));


//Agregando la autentucacion
builder.Services.AddAuthentication();

//Agregando Identity en si. Identity es lo que nos permitira el manejo de la
//seguridad en cuanto a usuarios y roles se refiere.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


//Desactivando las vistas por defecto ofrecidas por identity, aqui colocamos las vistas en donde el usuario se podra autenticar
//tambien se restringen las vistas al usuario si no esta autenticado.
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/usuarios/login";
        opciones.AccessDeniedPath = "/usuarios/login";
    });

//Necesitamos este servicio para globalizar la aplicacion, es decir, que soporte cualquier idioma, caractér, etc.
builder.Services.AddLocalization(opciones =>
{
    opciones.ResourcesPath = "Recursos";
});

builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

var app = builder.Build();

var culturasUISoportadas = new[] { "es", "en" };

//Especificando las culturas que vamos a mostrar en la aplicacion. 
app.UseRequestLocalization(opciones =>
{
    //El idioma que aparecera por defecto
    opciones.DefaultRequestCulture = new RequestCulture("es");

    //Los idiomas que podemos mostrar (es - en)
    opciones.SupportedCultures = culturasUISoportadas
    .Select(cultura => new CultureInfo(cultura)).ToList();
});

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

//Importante agregar estas lineas
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
