using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ApiPeliculas.Modelos;

public class Pelicula
{
    public enum TipoClasificacion
    {
        Siete,
        Trece,
        Dieciseis,
        Dieciocho
    }

    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public byte[] RutaImagen { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Duracion { get; set; }
    public TipoClasificacion Clasificacion { get; set; }
    public DateTime FechaCreacion { get; set; }

    [Required]
    [ForeignKey("categoriaId")]
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; }
}