namespace WebApiAutores.DTOs
{
    public class AutorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        //public List<LibroDTO> Libros { get; set; } -> esto lo pasamos a otra clase AutorDTOConLibros.
    }
}
