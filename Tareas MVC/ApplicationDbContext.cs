using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tareas_MVC.Entidades;

namespace Tareas_MVC
{
    public class ApplicationDbContext: IdentityDbContext

    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        
        }

        //Aqui se crean las entidades desde las clases
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Pasos> Pasos { get; set; }
        public DbSet<ArchivosAdjuntos> ArchivosAdjuntos { get; set; }




    }
}
