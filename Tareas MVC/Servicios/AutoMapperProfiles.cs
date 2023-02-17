using AutoMapper;
using Tareas_MVC.Entidades;
using Tareas_MVC.Models;

namespace Tareas_MVC.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {

            CreateMap<Tarea, TareasDTO>();
        }
    }
}
