using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tareas_MVC.Entidades
{
    public class ArchivosAdjuntos
    {

        public Guid Id { get; set; }
        public int TareaId { get; set; }
        public Tarea Tarea{ get; set; }

        [Required]
        [StringLength(250)]
        public string Titulo { get; set; }
        [Unicode]   
        public string Url { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
