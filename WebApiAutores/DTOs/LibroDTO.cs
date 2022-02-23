namespace WebApiAutores.DTOs
{
    public class LibroDTO
    {
        public int id { get; set; }
        public string Titulo { get; set; }
        //public List<AutorDTO> Autores { get; set; } // esto lo pasamos a la clase LibroDTOConAutores
        public DateTime FechaPublicacion { get; set; }
        public List<ComentarioDTO> Comentarios { get; set; }
    }
}
