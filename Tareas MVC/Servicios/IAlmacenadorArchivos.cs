using Tareas_MVC.Models;

namespace Tareas_MVC.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task Borrar(string ruta, string contenedor);

        Task<AlmacenarArchivoResultado[]> Almacenar(string contenedor, IEnumerable<IFormFile> archivos); 
        //FormFile es el tipo de dato utilizado para representar a cualquier tipo de archivo en ASP.NET Core

    }
}
