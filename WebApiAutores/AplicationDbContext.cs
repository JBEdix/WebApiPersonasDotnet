﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class AplicationDbContext : IdentityDbContext
    {
        public AplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AutorLibro>()
                .HasKey(auLib => new { auLib.AutorId, auLib.LibroId });
        }

        public DbSet<Autor> Autores { get; set; } 
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<AutorLibro> AutoresLibros { get; set;} // Relacion muchos a muchos
    }
}
