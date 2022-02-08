using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Autor> Autores { get; set; } // Estamos diciendo que se cree una tabla en sqlServer a partir del esquema de la clase autor
        public DbSet<Libro> Libros { get; set; }
    }
}
