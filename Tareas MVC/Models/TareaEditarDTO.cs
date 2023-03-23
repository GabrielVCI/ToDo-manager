using System.ComponentModel.DataAnnotations;

namespace Tareas_MVC.Models
{
    public class TareaEditarDTO
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [StringLength(50)]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
    }
}
