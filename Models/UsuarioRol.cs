using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Periodico.Models
{
    public class UsuarioRol
    {
        public int Id { get; set; }
        public string Estado { get; set; }

        [ForeignKey("Usuario")]
        public int idusuario { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }

        [ForeignKey("Rol")]
        public int idrol { get; set; }
        [JsonIgnore]
        public Rol Rol { get; set; }

    }
}
