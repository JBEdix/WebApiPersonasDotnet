using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones => { opciones.Filters.Add(typeof(FiltroDeExcepcion)); })
                .AddJsonOptions( x =>  x.JsonSerializerOptions.ReferenceHandler =  ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();
            services.AddDbContext<AplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            

            services.AddEndpointsApiExplorer();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); // ctrl + . >> instalar el paquete Jwt

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebApiAutores", Version = "V1" });
            });

            services.AddAutoMapper(typeof(Startup));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Agregando un middleware directamente===================
            //app.Use(async (contexto, siguiente) =>
            //{
            //    using (var ms = new MemoryStream())
            //    {
            //        var cuerpoOriginalRespuesta = contexto.Response.Body;
            //        contexto.Response.Body = ms;

            //        await siguiente.Invoke(); // esto manda al siguiente middleware
            //        // Cuando se retorne a este middleware se ejecutara la siguientes lineas.

            //        ms.Seek(0, SeekOrigin.Begin);
            //        string respuesta = new StreamReader(ms).ReadToEnd(); // con esto guardamos la respuest hhtp
            //        ms.Seek(0, SeekOrigin.Begin);

            //        await ms.CopyToAsync(cuerpoOriginalRespuesta);
            //        contexto.Response.Body = cuerpoOriginalRespuesta;

            //        logger.LogInformation(respuesta);

            //    }
            //});

            // Agregando un middleware desde una clase===============================
            // app.UseMiddleware<LoguearRespuestaHttpMiddleware>(); // Aqui exponemos la clase
            app.UseLoguearRespuestaHTTP(); // No exponemos la clase.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
