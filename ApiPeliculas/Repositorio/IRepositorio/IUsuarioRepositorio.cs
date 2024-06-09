using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;

namespace ApiPeliculas.Repositorio.IRepositorio;

public interface IUsuarioRepositorio
{
    ICollection<Usuario> GetUsuarios();
    Usuario? GetUsuario(int id);
    bool IsUniqueUser(string usuario);
    UsuarioLoginRespuestaDto Login(UsuarioLoginDto dto);
    Task<Usuario> Registro(UsuarioRegistroDto dto);
}