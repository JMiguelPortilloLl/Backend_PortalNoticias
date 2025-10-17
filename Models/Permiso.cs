namespace Periodico.Models
{
    public class Permiso
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public ICollection<Detalle_Permiso> detallePermisos { get; set; } 
    }
}
