using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.DTOs
{
    public class CrearCategoriaDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(60, ErrorMessage = "El número máximo de caracteres es de 100!")]
        public string Nombre { get; set; } = "";
    }
}