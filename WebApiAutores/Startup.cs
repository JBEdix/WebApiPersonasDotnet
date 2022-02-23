using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones => { 
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
                opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
            })
                .AddJsonOptions( x =>  x.JsonSerializerOptions.ReferenceHandler =  ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();
            services.AddDbContext<AplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddEndpointsApiExplorer();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {  // ctrl + . >> instalar el paquete Jwt
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { 
                    Title = "WebApiAutores", 
                    Version = "V1",
                    Description = "Descripcion de mi web api",
                    Contact = new OpenApiContact
                    {
                        Email = "jheral.blanco@gmail.com",
                        Name = "Jheral Blanco",
                        Url = new Uri("https://www.linkedin.com/in/jheral-blanco-8b5934180/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }
                });
                c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebApiAutores", Version = "V2" });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }                    }
                });

                var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
                c.IncludeXmlComments(rutaXML);

            });

            services.AddAutoMapper(typeof(Startup));

            // Configurando el servicio de Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configurando la autorizacion basada en Claims
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("EsAdmin"));
            });

            services.AddDataProtection();
            services.AddTransient<HashService>();

            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "CantidadTotalRegistros" });
                });
            });
            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);
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
                
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAutores v2");

            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(); // Uso de cors

            app.UseAuthorization();

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
