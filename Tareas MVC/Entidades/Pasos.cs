namespace Tareas_MVC.Entidades
{
    public class Pasos
    {
        public Guid Id { get; set; } //Guid viene de las siglas Global unique identifier, lo que significa que el id tendra una estructura diferente y unica 46da-f5as-65d6-fasd
        public int TareaId { get; set; }
        public Tarea Tarea { get; set; }//Esto es una propiedad de navegacion, es como si fuera un INNER JOIN.
        public string Descripcion { get; set; } 
        public bool Realizado { get; set; }
        public int Orden { get; set; }
    }
}
