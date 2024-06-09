using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.DTOs;

public class UsuarioLoginDto
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    public string NombreUsuario { get; set; } = string.Empty;
    [Required(ErrorMessage = "El password es obligatorio")]
    public string Password { get; set; } = string.Empty;
}