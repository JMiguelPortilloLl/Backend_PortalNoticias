using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Periodico.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Telefono { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Estado { get; set; }
        public ICollection<Publicacion> publicaciones { get; set; }
        public ICollection<Detalle_Permiso> detallePermisos { get; set; }
        public ICollection<UsuarioRol> UsuariosRoles { get; set; }
    }
}
