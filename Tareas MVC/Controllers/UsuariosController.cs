using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Tareas_MVC.Models;
using Tareas_MVC.Servicios;

namespace Tareas_MVC.Controllers
{
    public class UsuariosController : Controller
    {

        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;

        //Inyectando la dependencia identity para la seguridad a nivel del controlador.
        public UsuariosController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        [AllowAnonymous]
        public ActionResult Registro() {

            return View();
        
        }

        //Action para registrarse en la aplicacion
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new IdentityUser()
            {
                Email = modelo.Email,
                UserName = modelo.Email
            };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if(resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach(var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);    
            }
        }


        [AllowAnonymous]
        public ActionResult Login(string mensaje = null)
        {
            if(mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;
            }
            return View();
        }

        //Action para loguarse en la aplicacion
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewMode modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame
                , lockoutOnFailure: false); 

            if(resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto.");
                return View(modelo);
            }
        }

        //Action para desloguearse de la aplicacion
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]

        //Redirigiendo al usuario a una fuente en donde pueda loguearse con ChallengeResult
        public ChallengeResult LoginExterno(string proveedor, string UrlRetorno)
        {
            var UrlRedireccion = Url.Action("RegistrarUsuarioExterno", values: new { UrlRetorno });
            var propiedades = signInManager.ConfigureExternalAuthenticationProperties(proveedor, UrlRedireccion);

            return new ChallengeResult(proveedor, propiedades);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null, string remoteError = null)
        {
            //Utilizando ??, lo que nos devuelve un valor booleando, en este caso, si es nulo o no.
            urlRetorno = urlRetorno ?? Url.Content("~/");

            var mensaje = "";

            //Verificamos si el mensaje es nulo,luego le pasamos el mensaje no lo es para despues
            //redirigir al usuario al login y pasamos el mensaje para mostrarlo en la vista usando 
            //routeValues
            if (remoteError is not null)
            {
                mensaje = $"Mensaje del proveedor externo: {remoteError}";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info is null)
            {
                mensaje = $"Mensaje del proveedor externo: {remoteError}";  
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            //La cuenta ya existe
            //Intentando loguear al usuario con la cuenta del proveedor, si existe el resultado de
            //la siguiente condicion sera true
            if (resultadoLoginExterno.Succeeded)
            {

                return LocalRedirect(urlRetorno);
            }


            //Si la condicion anterior no se cumple, significa que debemos crearle una cuenta al usuario.
            string email = "";

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }

            else
            {
                mensaje = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("login", routeValues: new {mensaje});

            }

            var usuario = new IdentityUser { Email= email, UserName = email};

            var resultadoCrearUsuario = await userManager.CreateAsync(usuario);

            if(!resultadoCrearUsuario.Succeeded)
            {
                mensaje = resultadoCrearUsuario.Errors.First().Description;
                return RedirectToAction("login", routeValues: new {mensaje});   
            }

            var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, info);

            if(resultadoAgregarLogin.Succeeded) {

                await signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }

            mensaje = "Ha ocurrido un error agregando el login.";

            return RedirectToAction("login", routeValues: new { mensaje });

        }

        //Necesitamos mostrar el listado de usuarios.
        [HttpGet]
        //Dandole acceso a esta accion a un rol especifico
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            var usuario = await context.Users.Select(u => new UsuarioViewModel
            {
                Email = u.Email,
            }).ToListAsync();

            var modelo = new UsuariosListadoViewModel();
            modelo.Usuarios= usuario;   
            modelo.Mensajes = mensaje;
            return View(modelo);    
        }

        //Para colocar el rol de admin a un usuario
        [HttpPost]
        //Dandole acceso a esta accion a un rol especifico
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> HacerAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            //Si el email no existe
            if (usuario == null)
            {
                return NotFound();
            }

            //Si el email existe, necesitamos agregar el rol de admin al email que obtenemos, usando 
            //AddRoleAsync.
            await userManager.AddToRoleAsync(usuario, Constantes.RolAdmin);

            //Luego necesitamos redireccionar al usuario, dejando claro con un mensaje que el rol fue asignado.
            return RedirectToAction("Listado", 
                routeValues: new { mensaje = "Rol asignado correctamente a " + email });

        }


        //Para remover el rol de admin a un usuario
        [HttpPost]
        //Dandole acceso a esta accion a un rol especifico
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            //Si el email no existe
            if (usuario == null)
            {
                return NotFound();
            }

            //Si el email existe, necesitamos remover el rol de admin del email que obtenemos, usando 
            //RemoveFromRoleAsync.
            await userManager.RemoveFromRoleAsync(usuario, Constantes.RolAdmin);

            //Luego necesitamos redireccionar al usuario, dejando claro con un mensaje que el rol fue removido.
            return RedirectToAction("Listado",
                routeValues: new { mensaje = "Rol removido correctamente a " + email });

        }
    }
}
