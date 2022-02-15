using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es requerido")] // Campo Nombre requerido
        [StringLength(maximumLength:120, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
        [PrimeralLetraMayuscula]
        public string Nombre { get; set; }
        public List<AutorLibro> autoresLibros { get; set; } // Relacion muchos a muchos
    }
}
