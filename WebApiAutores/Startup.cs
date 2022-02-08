using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;

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
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            }).AddJsonOptions( x => 
                x.JsonSerializerOptions.ReferenceHandler =  ReferenceHandler.IgnoreCycles);
            services.AddDbContext<AplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddTransient<IServicio, ServicioA>(); // cuando necesitemos resolver el IServicio se nos va a intanciar una nueva instancia ServicioA. Esta es para funciones que van a ejecutar alguna funcionalidad sin tener que retener data que se va a usar en diferentes lugares. No utiliza estado.
            //services.AddScoped<IServicio, ServicioA>(); // Lo que cambia es el tiempo de vida del servicio. El tiempo de vida de la clase servicioA aumenta, desde el mismo contexto HTTP se dara la misma instancia. En distintas peticiones HTTP se tendran distintas instancias. Ejemplo, el ApplicationDbContext.
            //services.AddSingleton<IServicio, ServicioA>(); // Con esta, siempre tendremos la misma instancia, incluso para los distintos usuarios con distintas peticiones http.
            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();

            services.AddTransient<MiFiltroDeAccion>();
            services.AddHostedService<EscribirEnArchivo>();

            services.AddEndpointsApiExplorer();

            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); // ctrl + . >> instalar el paquete Jwt

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebApiAutores", Version = "V1" });
            });

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

            // Middlawares son los que dicen Use
            app.Map("/ruta1", app => // con esto bifurcamos la tuberia, si entramos a la ruta localhost:port/ruta1 estariamos entrando en este middleware.
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Estoy interceptando la tuberia");
                });
            });
            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
