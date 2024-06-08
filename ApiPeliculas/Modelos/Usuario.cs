using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
