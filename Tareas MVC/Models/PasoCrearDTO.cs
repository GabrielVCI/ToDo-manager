using System.ComponentModel.DataAnnotations;

namespace Tareas_MVC.Models
{
    public class PasoCrearDTO
    {
        [Required]
        public string Descripcion { get; set; }

        public bool Realizado { get; set; }
    }
}
