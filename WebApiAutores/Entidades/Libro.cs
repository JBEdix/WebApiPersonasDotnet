using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [PrimeralLetraMayuscula]
        [StringLength(maximumLength:250)]
        public string Titulo { get; set;}
        public DateTime? FechaPublicacion { get; set; } // ?: para hacerlo nullable
        public List<Comentario> comentarios { get; set; }
        public List<AutorLibro> autoresLibros { get; set; } // Relacion muchos a muchos
    }
}
