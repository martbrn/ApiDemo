using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace ApiPeliculas.Controllers.V1
{
    [Route("api/v{version:apiVersion}/peliculas")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepositorio _pelRepo;
        private readonly IMapper _mapper;

        public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper)
        {
            _pelRepo = pelRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _pelRepo.GetPeliculas();

            var listaPeliculasDto = new List<PeliculaDto>();

            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            return Ok(listaPeliculasDto);
        }

        [AllowAnonymous]
        [HttpGet("id:int", Name = "GetPelicula")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int id)
        {
            var Pelicula = _pelRepo.GetPelicula(id);

            if (Pelicula == null)
                return NotFound();

            var dto = _mapper.Map<PeliculaDto>(Pelicula);

            return Ok(dto);
        }

        //[Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreatePelicula([FromForm] PeliculaDto dto)
        {
            if (!ModelState.IsValid || dto == null)
                return BadRequest(ModelState);

            if (_pelRepo.ExistePelicula(dto.Nombre))
            {
                ModelState.AddModelError("", "La Pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            var newPelicula = _mapper.Map<Pelicula>(dto);

            //if (!_pelRepo.CrearPelicula(newPelicula))
            //{
            //    ModelState.AddModelError("", $"Algo salio mal guardando el registro {newPelicula.Nombre}");
            //    return StatusCode(500, ModelState);
            //}

            if (dto.Imagen != null)
            {
                string nombreArchivo = newPelicula.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(dto.Imagen.FileName);
                string rutaArchivo = @"wwwroot\ImagenesPeliculas\" + nombreArchivo;

                var ubicacionDirectorio = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);

                FileInfo file = new FileInfo(nombreArchivo);
                if (file.Exists)
                    file.Delete();

                using (var fileStream = new FileStream(ubicacionDirectorio, FileMode.Create))
                {
                    dto.Imagen.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                newPelicula.RutaImagen = baseUrl + "/ImagenesPeliculas/" + nombreArchivo;
                newPelicula.RutaLocalImagen = rutaArchivo;

            }
            else
                newPelicula.RutaImagen = "https://placehold.co/600x400";

            _pelRepo.CrearPelicula(newPelicula);
            return CreatedAtRoute("GetPelicula", new { id = newPelicula.Id }, newPelicula);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("id:int", Name = "ActualizarPelicula")]
        [ProducesResponseType(201, Type = typeof(PeliculaDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPelicula(int id, [FromForm] ActualizaPeliculaDto dto)
        {
            if (!ModelState.IsValid || dto == null || id != dto.Id)
                return BadRequest(ModelState);

            var pelicula = _mapper.Map<Pelicula>(dto);

            //if (!_pelRepo.ActualizarPelicula(Pelicula))
            //{
            //    ModelState.AddModelError("", $"Algo salio mal actualizando el registro {Pelicula.Nombre}");
            //    return StatusCode(500, ModelState);
            //}

            if (dto.Imagen != null)
            {
                string nombreArchivo = pelicula.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(dto.Imagen.FileName);
                string rutaArchivo = @"wwwroot\ImagenesPeliculas\" + nombreArchivo;

                var ubicacionDirectorio = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);

                FileInfo file = new FileInfo(nombreArchivo);
                if (file.Exists)
                    file.Delete();

                using (var fileStream = new FileStream(ubicacionDirectorio, FileMode.Create))
                {
                    dto.Imagen.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                pelicula.RutaImagen = baseUrl + "/ImagenesPeliculas/" + nombreArchivo;
                pelicula.RutaLocalImagen = rutaArchivo;

            }
            else
                pelicula.RutaImagen = "https://placehold.co/600x400";

            _pelRepo.ActualizarPelicula(pelicula);
            return CreatedAtRoute("GetPelicula", new { id = pelicula.Id }, pelicula);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("id:int", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BorrarPelicula(int id)
        {
            if (!_pelRepo.ExistePelicula(id))
                return NotFound();

            var Pelicula = _pelRepo.GetPelicula(id);

            if (Pelicula == null)
                return BadRequest(ModelState);

            if (!_pelRepo.BorrarPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {Pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCateogria/{categoriaId:int}", Name = "GetPeliculasEnCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPeliculas = _pelRepo.GetPeliculasEnCategoria(categoriaId);

            if (listaPeliculas == null)
                return NotFound();

            var listaCateogriaDto = new List<PeliculaDto>();

            foreach (var lista in listaPeliculas)
            {
                listaCateogriaDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            return Ok(listaPeliculas);
        }

        [AllowAnonymous]
        [HttpGet("Buscar")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuscarPeliculas(string search)
        {
            var listaPeliculas = _pelRepo.GetPeliculas(search);

            if (listaPeliculas == null)
                return NotFound();

            var listaCateogriaDto = new List<PeliculaDto>();

            foreach (var lista in listaPeliculas)
            {
                listaCateogriaDto.Add(_mapper.Map<PeliculaDto>(lista));
            }

            return Ok(listaPeliculas);
        }
    }
}