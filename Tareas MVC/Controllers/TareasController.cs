using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas_MVC.Entidades;
using Tareas_MVC.Models;
using Tareas_MVC.Servicios;

namespace Tareas_MVC.Controllers
{
   
    //Usando un controller en forma de WebApi, que retornann archivos JSON en vez de las vistas tradicionales.
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiciosUsuarios serviciosUsuarios;
        private readonly IMapper mapper;

        public TareasController(ApplicationDbContext _context, IServiciosUsuarios serviciosUsuarios, IMapper mapper)
        {
            this._context = _context;
            this.serviciosUsuarios = serviciosUsuarios;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<TareasDTO>> Get()
        {
            var UsuarioId = serviciosUsuarios.ObtenerUsuarioId();
            //Listar todas las tareas para mostrarlas en el View filtradas por usuario.
            //Tambien ordenadas por la columna orden de la tabla tareas.
            var tareas = await _context.Tareas.Where(t => t.UsuarioCreacionId == UsuarioId).
                OrderBy(t => t.Orden)
                .ProjectTo<TareasDTO>(mapper.ConfigurationProvider).
                ToListAsync();

            return tareas;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tarea>> Get(int id)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();

            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == id &&
            t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }
            Console.WriteLine("se esta ejecutando");

            return tarea;
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();

            var existenTareas = await _context.Tareas.AnyAsync(t => t.UsuarioCreacionId == usuarioId);

            var ordenMayor = 0;

            if (existenTareas)
            {
                ordenMayor = await _context.Tareas.Where(x => x.UsuarioCreacionId == usuarioId).
                    Select(x => x.Orden).MaxAsync();
            }

            var tarea = new Tarea
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Orden = ordenMayor + 1
            };
            _context.Add(tarea);
            await _context.SaveChangesAsync();
            return tarea;
        }

        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] int[] id)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();

            var tareas = await _context.Tareas.
                Where(t => t.UsuarioCreacionId == usuarioId).ToListAsync();

            var tareasId = tareas.Select(t => t.Id);

            var idsNoPertenecenAlUsuario = id.Except(tareasId).ToList();

            if (idsNoPertenecenAlUsuario.Any())
            {
                return Forbid();
            }

            var tareasDiccionario = tareas.ToDictionary(x => x.Id);

            for (int i = 0; i < id.Length; i++)
            {
                var Id = id[i];
                var tarea = tareasDiccionario[Id];
                tarea.Orden = i + 1;
            }

            await _context.SaveChangesAsync();

            return Ok();

        }
    }
}
