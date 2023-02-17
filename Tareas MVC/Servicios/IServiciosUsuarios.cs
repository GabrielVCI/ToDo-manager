using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tareas_MVC.Servicios
{
    public interface IServiciosUsuarios 
    {
        string ObtenerUsuarioId();
    }

    public class ServiciosUsuarios : IServiciosUsuarios
    {
        private HttpContext httpContext;

        public ServiciosUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }
        public string ObtenerUsuarioId()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where
                    (x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

                return idClaim.Value;
            }
            else
            {
                throw new Exception("El usuario no está autenticado");
            }
        }


    }
}
