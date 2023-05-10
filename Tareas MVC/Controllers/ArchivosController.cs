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
        private readonly IServiciosUsuarios serviciosUsuario;
        private readonly string contenedor = "archivosadjuntos";

        public ArchivosController(ApplicationDbContext context, 
                                  IAlmacenadorArchivos almacenadorArchivos, 
                                  IServiciosUsuarios serviciosUsuario)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.serviciosUsuario = serviciosUsuario;
        }

        [HttpPost("{tareaId:int}")]
        public async Task<ActionResult<IEnumerable<ArchivosAdjuntos>>> Post(int tareaId, 
            [FromForm] IEnumerable<IFormFile> archivos) //FromForm permite recibir archivos por medio del API.
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
                ordenMayor = await context.ArchivosAdjuntos.Where(a => a.TareaId == tareaId)
                    .Select(o => o.Orden).MaxAsync();
            }

            var resultado = await almacenadorArchivos.Almacenar(contenedor, archivos);

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] string titulo)
        {
            var usuarioId = serviciosUsuario.ObtenerUsuarioId();

            var archivoAdjunto = await context.ArchivosAdjuntos.Include(a => a.Tarea).FirstOrDefaultAsync(a => a.Id == id);

            if(archivoAdjunto is null)
            {
                return NotFound();
            }

            if(archivoAdjunto.Tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            archivoAdjunto.Titulo = titulo;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var usuarioId = serviciosUsuario.ObtenerUsuarioId();

            var archivoAdjunto = await context.ArchivosAdjuntos.Include(a => a.Tarea).FirstOrDefaultAsync(a => a.Id == id);

            if(archivoAdjunto is null)
            {
                return NotFound();  
            }

            if(archivoAdjunto.Tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            context.Remove(archivoAdjunto);
            await context.SaveChangesAsync();
            await almacenadorArchivos.Borrar(archivoAdjunto.Url, contenedor);
            return Ok();

        }
    }
}
