using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas_MVC.Entidades;
using Tareas_MVC.Servicios;

namespace Tareas_MVC.Controllers
{
   
    //Usando un controller en forma de WebApi, que retornann archivos JSON en vez de las vistas tradicionales.
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiciosUsuarios serviciosUsuarios;

        public TareasController(ApplicationDbContext _context, IServiciosUsuarios serviciosUsuarios)
        {
            this._context = _context;
            this.serviciosUsuarios = serviciosUsuarios;
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();

            var existenTareas = await _context.Tareas.AnyAsync(t => t.UsuarioCreacionId == usuarioId);

            var ordenMayor = 0;

        } 
 
    }
}
