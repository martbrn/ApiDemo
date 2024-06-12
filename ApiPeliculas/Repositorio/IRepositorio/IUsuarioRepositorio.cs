using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;

namespace ApiPeliculas.Repositorio.IRepositorio;

public interface IUsuarioRepositorio
{
    ICollection<AppUsuario> GetUsuarios();
    AppUsuario? GetUsuario(string id);
    bool IsUniqueUser(string usuario);
    Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto dto);
    //    Task<Usuario> Registro(UsuarioRegistroDto dto);
    Task<UsuarioDatosDto> Registro(UsuarioRegistroDto dto);
}