using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.DTOs;

public class UsuarioLoginRespuestaDto
{
    public UsuarioDatosDto? Usuario { get; set; }
    public required string Token { get; set; }

}