using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas_MVC.Entidades;
using Tareas_MVC.Servicios;


namespace Tareas_MVC.Controllers
{
    [Route("api/archivos")]
    public class ArchivosController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ServiciosUsuarios serviciosUsuario;
        private readonly string contenedor = "archivosadjuntos";

        public ArchivosController(ApplicationDbContext context, 
                                  IAlmacenadorArchivos almacenadorArchivos, 
                                  ServiciosUsuarios serviciosUsuario)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.serviciosUsuario = serviciosUsuario;
        }

        [HttpPost("{tareaId:int}")]
        public async Task<ActionResult<IEnumerable<ArchivosAdjuntos>>> Post(int tareaId, 
            [FromForm] IEnumerable<IFormFile> archivo) //FromForm permite recibir archivos por medio del API.
        {

            var usuarioId = serviciosUsuario.ObtenerUsuarioId();

            var tarea = await context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaId);

            if(tarea is null)
            {
                return NotFound();
            }

            if(tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            var existenArchivosAdjuntos = await context.ArchivosAdjuntos.AnyAsync(a => a.TareaId == tareaId);

            var ordenMayor = 0;

            if (existenArchivosAdjuntos)
            {
                var archivos = await context.ArchivosAdjuntos.Where(a => a.TareaId == tareaId)
                    .Select(o => o.Orden).MaxAsync();
            }

            var resultado = await almacenadorArchivos.Almacenar(contenedor, archivo);

            var archivosAdjuntos = resultado.Select((resultado, indice) => new ArchivosAdjuntos
            {
                TareaId = tareaId,
                FechaCreacion = DateTime.UtcNow,
                Url = resultado.URL,
                Titulo = resultado.Titulo,
                Orden = ordenMayor + indice + 1

            });

            context.AddRange(archivosAdjuntos);

            await context.SaveChangesAsync();
            return archivosAdjuntos.ToList();
        }
    }
}
