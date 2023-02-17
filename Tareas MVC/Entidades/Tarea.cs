using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tareas_MVC.Entidades
{
    public class Tarea
    {
        public int Id { get; set; } //Esta sera la llave primaria de nuestra tabla
        [Required]
        [StringLength(250)]
        public string Titulo { get; set; }
        public string Descrpcion { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacionId { get; set; }

        // Esto es como si fuera una Foreign Key.
        public IdentityUser UsuarioCreacion { get; set; }
        public List<Pasos> Pasos { get; set; } 
        public List<ArchivosAdjuntos> ArchivosAdjuntos { get; set; }

    }
}
