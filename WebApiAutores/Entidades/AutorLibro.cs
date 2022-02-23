namespace WebApiAutores.Entidades
{
    public class AutorLibro
    {
        // Clase relacion de muchos a muchos
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public int Orden { get; set; }
        public Libro libro { get; set; }
        public Autor Autor { get; set; }


    }
}
