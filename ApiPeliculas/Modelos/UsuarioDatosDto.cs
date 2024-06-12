using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos
{
    public class UsuarioDatosDto
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}
