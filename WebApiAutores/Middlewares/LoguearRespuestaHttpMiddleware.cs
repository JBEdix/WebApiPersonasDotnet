namespace WebApiAutores.Middlewares
{
    // Creamos la siguiente clase estatica para poder usar el middleware sin tener que exponer la clase.
    public static class LoguearRespuestaHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHttpMiddleware>();
        }
    }
    public class LoguearRespuestaHttpMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHttpMiddleware> logger;

        public LoguearRespuestaHttpMiddleware(RequestDelegate siguiente, ILogger<LoguearRespuestaHttpMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        //Invoke o InvokeAsync

        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream()) 
            { 
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                await siguiente(contexto); // esto manda al siguiente middleware
                // Cuando se retorne a este middleware se ejecutara la siguientes lineas.

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd(); // con esto guardamos la respuest hhtp
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;

                logger.LogInformation(respuesta);

            }
        }
    }
}
