using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    public class Root: ControllerBase
    {
        [HttpGet(Name = "ObtenerRoot")]
        public ActionResult<IEnumerable<DatoHATEOAS>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();
            datosHateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("ObtenerRoot", new { }), 
                descripcion: "self", 
                metodo: "Get"));

            return datosHateoas;
        }
    }
}
