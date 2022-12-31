using System.ComponentModel.DataAnnotations;

namespace Tareas_MVC.Models
{
    public class LoginViewMode
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El correo es invalido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display]
        public bool Recuerdame { get; set;}

        
        

        
    }
}
