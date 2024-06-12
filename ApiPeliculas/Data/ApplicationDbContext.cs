using ApiPeliculas.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data;

public class ApplicationDbContext : IdentityDbContext<AppUsuario>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    //Agregar los modelos aquí
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<AppUsuario> AppUsuarios { get; set; }
    public DbSet<Pelicula> Peliculas { get; set; }
}