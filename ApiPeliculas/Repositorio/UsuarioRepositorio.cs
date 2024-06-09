using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly AplicationDbContext _context;
    private string claveSecreta;
    public UsuarioRepositorio(AplicationDbContext context, IConfiguration config)
    {
        _context = context;
#pragma warning disable CS8601 // Posible asignación de referencia nula
        claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
#pragma warning restore CS8601 // Posible asignación de referencia nula
    }

    public Usuario? GetUsuario(int id)
    {
        return _context.Usuarios.FirstOrDefault(x => x.Id == id);
    }

    public ICollection<Usuario> GetUsuarios()
    {
        return _context.Usuarios
            .OrderBy(x => x.NombreUsuario)
            .ToList();
    }

    public bool IsUniqueUser(string usuario)
    {
        var existe = _context.Usuarios.Any(x => x.Nombre == usuario.Trim());
        return existe;
    }

    public async Task<Usuario> Registro(UsuarioRegistroDto dto)
    {
        var passwordEncriptado = obtenermd5(dto.Password);

        Usuario newUsuario = new Usuario()
        {
            Nombre = dto.Nombre,
            Password = passwordEncriptado,
            NombreUsuario = dto.NombreUsuario,
            Rol = dto.Rol,
        };
        _context.Usuarios.Add(newUsuario);
        await _context.SaveChangesAsync();
        return newUsuario;
    }

    public UsuarioLoginRespuestaDto Login(UsuarioLoginDto dto)
    {
        var passwordEncriptado = obtenermd5(dto.Password);

        var usuario = _context.Usuarios.FirstOrDefault(
             x =>
             x.NombreUsuario.ToLower() == dto.NombreUsuario.ToLower()
             && x.Password == passwordEncriptado
            );

        // No encontro el usuario
        if (usuario == null)
        {
            return new UsuarioLoginRespuestaDto()
            {
                Token = "",
                Usuario = null
            };
        }

        // Encontro el usuario
        var manejadorToken = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(claveSecreta);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol)
            }),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = manejadorToken.CreateToken(tokenDescriptor);

        var respuesta = new UsuarioLoginRespuestaDto()
        {
            Token = manejadorToken.WriteToken(token),
            Usuario = usuario
        };

        return respuesta;

    }

    //Método para encriptar contraseña con MD5 se usa tanto en el Acceso como en el Registro
    public static string obtenermd5(string valor)
    {
        MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
        data = x.ComputeHash(data);
        string resp = "";
        for (int i = 0; i < data.Length; i++)
            resp += data[i].ToString("x2").ToLower();
        return resp;
    }
}
