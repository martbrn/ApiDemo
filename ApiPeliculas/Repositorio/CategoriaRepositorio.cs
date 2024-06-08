using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;


namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly AplicationDbContext _context;
        public CategoriaRepositorio(AplicationDbContext context)
        {
            _context = context;
        }
        public bool ActualizarCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _context.Categorias.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _context.Categorias.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _context.Categorias.Any(x => x.Nombre.ToUpper().Trim() == nombre.ToUpper().Trim());
            return valor;
        }

        public bool ExisteCategoria(int id)
        {
            bool valor = _context.Categorias.Any(x => x.Id == id);
            return valor;
        }

        public Categoria? GetCategoria(int id)
        {
            return _context.Categorias.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _context.Categorias.OrderBy(x => x.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}
