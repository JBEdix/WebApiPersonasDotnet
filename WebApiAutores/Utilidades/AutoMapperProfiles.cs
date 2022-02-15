using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));

            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(libro => libro.autoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPacthDTO, Libro>().ReverseMap(); // Esto es para el HTTP patch

            CreateMap<ComentarioCreacionDTO, Comentario>(); 
            CreateMap<Comentario, ComentarioDTO>();
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();
            if (autor.autoresLibros == null) { return resultado; }

            foreach (var autorLibro in autor.autoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    id = autorLibro.LibroId,
                    Titulo = autorLibro.libro.Titulo
                });
            }
            return resultado;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var ressultado = new List<AutorDTO>();
            if (libro.autoresLibros == null) { return ressultado; }

            foreach (var autorLibro in libro.autoresLibros) {
                ressultado.Add(new AutorDTO()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return ressultado;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultados = new List<AutorLibro>();
            if (libroCreacionDTO.AutoresIds == null)
            {
                return resultados;
            }

            foreach (var autorId in libroCreacionDTO.AutoresIds)
            {
                resultados.Add(new AutorLibro() { AutorId = autorId });
            }
            return resultados;
        }
    }
}
