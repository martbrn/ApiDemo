using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Configuramos la conexion a SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
{
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql"));
});

//Soporte para autenticacion con .NET Identity
builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

//Añadimos cache
builder.Services.AddResponseCaching();

// Agregamos los repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Agregar el AutoMapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//Soporte para versionamiento

var apiVersioningBuilder = builder.Services.AddApiVersioning(opcion =>
{
    opcion.AssumeDefaultVersionWhenUnspecified = false;
    opcion.DefaultApiVersion = new ApiVersion(1, 0);
    opcion.ReportApiVersions = true;
    //opcion.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version"));
});

apiVersioningBuilder.AddApiExplorer(
    opciones =>
    {
        opciones.GroupNameFormat = "'v'VVV";
        opciones.SubstituteApiVersionInUrl = true;
    }
    );

// Aqui se configura la Autenticacion
builder.Services.AddAuthentication(x =>
    {
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Add services to the container.
builder.Services.AddControllers(opcion =>
{
    //Cache profile. Un cache global
    opcion.CacheProfiles.Add("PorDefecto20Segundos", new CacheProfile() { Duration = 20 });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer.\r\n\r\n" +
        "Ingrea la palabra 'Bearer' seguida de un espacio y despues su token en el campo de abajo \r\n\r\n" +
        "Ejemplo: \"Bearer tkjngdfjnkfd\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Peliculas Api V1",
        Description = "Api de Peliculas Versión 1",
        TermsOfService = new Uri("https://render2web.com/promociones"),
        Contact = new OpenApiContact
        {
            Name = "render2web",
            Url = new Uri("https://render2web.com/promociones")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://render2web.com/promociones")
        }
    }
    );
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Peliculas Api V2",
        Description = "Api de Peliculas Versión 2",
        TermsOfService = new Uri("https://render2web.com/promociones"),
        Contact = new OpenApiContact
        {
            Name = "render2web",
            Url = new Uri("https://render2web.com/promociones")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://render2web.com/promociones")
        }
    }
    );
});

//Soporte para CORS
// En este caso para cualquier dominio : "*"
builder.Services.AddCors(p => p.AddPolicy("PolicyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculasV1");
        opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
    });
}

app.UseHttpsRedirection();


//Soporte para CORS
app.UseCors("PolicyCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();