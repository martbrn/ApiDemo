using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;


namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepositorio : IPeliculaRepositorio
    {
        private readonly AplicationDbContext _context;
        public PeliculaRepositorio(AplicationDbContext context)
        {
            _context = context;
        }
        public bool ActualizarPelicula(Pelicula Pelicula)
        {
            Pelicula.FechaCreacion = DateTime.Now;
            _context.Peliculas.Update(Pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula Pelicula)
        {
            _context.Peliculas.Remove(Pelicula);
            return Guardar();
        }

        public bool CrearPelicula(Pelicula Pelicula)
        {
            Pelicula.FechaCreacion = DateTime.Now;
            _context.Peliculas.Add(Pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string nombre)
        {
            bool valor = _context.Peliculas.Any(x => x.Nombre.ToUpper().Trim() == nombre.ToUpper().Trim());
            return valor;
        }

        public bool ExistePelicula(int id)
        {
            bool valor = _context.Peliculas.Any(x => x.Id == id);
            return valor;
        }

        public Pelicula? GetPelicula(int id)
        {
            return _context.Peliculas.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _context.Peliculas.OrderBy(x => x.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculas(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                return _context.Peliculas
                    .Where(x => x.Nombre.Contains(search) || x.Descripcion.Contains(search))
                    .OrderBy(x => x.Nombre)
                    .ToList();
            }
            return _context.Peliculas.ToList();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int id)
        {
            return _context.Peliculas
                .Include(x => x.Categoria)
                .Where(x => x.CategoriaId == id).OrderBy(x => x.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}
