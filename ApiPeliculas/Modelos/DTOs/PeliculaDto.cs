using System.ComponentModel.DataAnnotations;
using static ApiPeliculas.Modelos.Pelicula;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Modelos.DTOs;

public class PeliculaDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(60, ErrorMessage = "El número máximo de caracteres es de 100!")]
    public string Nombre { get; set; } = "";
    public byte[] RutaImagen { get; set; }
    [Required(ErrorMessage = "La descripción es obligatoria")]
    public string Descripcion { get; set; } = string.Empty;
    [Required(ErrorMessage = "La duración es obligatoria")]
    public int Duracion { get; set; }
    public TipoClasificacion Clasificacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int CategoriaId { get; set; }
}