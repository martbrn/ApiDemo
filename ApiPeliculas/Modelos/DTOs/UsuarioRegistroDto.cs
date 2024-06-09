using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.DTOs;

public class UsuarioRegistroDto
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    public string NombreUsuario { get; set; } = string.Empty;
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El password es obligatorio")]
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}