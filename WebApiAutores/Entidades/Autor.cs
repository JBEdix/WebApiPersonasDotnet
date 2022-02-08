using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es requerido")] // Campo Nombre requerido
        [StringLength(maximumLength:120, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
        //[PrimeralLetraMayuscula]
        public string Nombre { get; set; }
        //[Range(18, 20)]
        //[NotMapped] // con esto el atributo no correspondera con una columna de la tabla en la base de datos
        //public int Edad { get; set; }
        //[CreditCard]
        //[NotMapped]
        //public string TarjetaCredito { get; set; }
        //[NotMapped]
        //[Url]
        //public string URL { get; set; }
        public List<Libro> Libros { get; set; }

        //[NotMapped]
        //public int Menor { get; set; }
        //[NotMapped]
        //public int Mayor { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper())
                {
                    // El yield lo que hace es que inserta la validacion en el IEnumerable
                    yield return new ValidationResult("La primera letra debe ser mayuscula", new string[] { nameof(Nombre) });
                }
            }

            //if (Menor > Mayor) {
            //    yield return new ValidationResult("ESte valor no puede ser mas grande que el campo Mayor", new string[] { nameof(Menor) });
            //}
        }
    }
}
