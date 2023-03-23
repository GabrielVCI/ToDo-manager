using AutoMapper;
using Tareas_MVC.Entidades;
using Tareas_MVC.Models;

namespace Tareas_MVC.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {

            CreateMap<Tarea, TareasDTO>().
                ForMember(
                dto => dto.PasosTotal, 
                entidad => entidad.MapFrom(x => x.Pasos.Count())).
                ForMember(
                dto => dto.PasosRealizados, 
                entidad => entidad.MapFrom(x => x.Pasos.Where(p => p.Realizado).Count()));
        }
    }
}
