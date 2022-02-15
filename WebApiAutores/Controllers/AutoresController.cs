using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    //[Route("api/[controller]")] // En tiempo de ejecucion el placeholder [controller] se sustituira por el nombre del controlador. En este caso, esta linea es practimente igual a la linea anterior, solo que la linea anterior se define el nombre explicitamente.
    public class AutoresController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(AplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }
        [HttpGet("Configuraciones")]
        public ActionResult<string> ObtenerConfiguracion()
        {
            //return configuration["connectionStrings:defaultConnection"]; // Accediendo a los atributos de appsettings.Development.json
            return configuration["nombre"];
        }

        [HttpGet] // api/autores
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            //return await context.Autores.Include(x => x.Libros).ToListAsync();
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
            //return new List<Autor>()
            //{
            //new Autor() { Id = 1, Nombre = "Jheral"},
            //new Autor() { Id = 1, Nombre = "Paty"},

            //};
        }

        [HttpGet("{id:int}", Name = "obtenerAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.autoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.libro)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }
            return mapper.Map<AutorDTOConLibros>(autor);
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync(); // Traer todos los que tengan el nombre enviado.
            if (autores == null)
            {
                return NotFound();
            }
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("first")]
        public async Task<ActionResult<Autor>> First([FromHeader] int MiValor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post( [FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var alreadyName = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
            if (alreadyName)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }
            //var autor = new Autor() 
            //{
            //    Nombre = autorCreacionDTO.Nombre // Esto tiene un problema, ya que si queremos agregar mas atributos a la clase Autores tenemos que hacerlo manualmente en AutorCreacionDTO y esto puede ser tedioso ya que podemos tener infinidad de atributos. Para evitar esto, usaremos AutoMapper. del paquetes Nugget.
            //};
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}")] //api/autores/1
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            //if (autor.Id != id)
            //{
            //    return BadRequest("El id del autor no coincide con el id de la url");
            //}
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")] //api/autores/1
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Autor() { Id = id});
            await context.SaveChangesAsync();
            return Ok();
        }

        

    }
}
