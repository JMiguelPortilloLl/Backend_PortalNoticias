using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Periodico.Models
{
    public class Detalle_Permiso
    {
        public int Id { get; set; }
        public string Estado { get; set; }

        [ForeignKey("Usuario")]
        public int idusuario { get; set; }
        [JsonIgnore]

        public Usuario Usuario { get; set; }
        [ForeignKey("Permiso")]
        public int idpermiso { get; set; }
        [JsonIgnore]
        public Permiso Permiso { get; set; }
    }
}
