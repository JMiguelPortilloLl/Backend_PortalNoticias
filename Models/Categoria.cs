namespace Periodico.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public ICollection<Publicacion> publicaciones { get; set; }
    }
}
