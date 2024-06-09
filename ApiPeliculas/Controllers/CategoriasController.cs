using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias()
        {
            var listaCategorias = _ctRepo.GetCategorias();

            var listaCateogriaDto = new List<CategoriaDto>();

            foreach (var lista in listaCategorias)
            {
                listaCateogriaDto.Add(_mapper.Map<CategoriaDto>(lista));
            }
            return Ok(listaCategorias);
        }

        [AllowAnonymous]
        [HttpGet("id:int", Name = "GetCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int id)
        {
            var categoria = _ctRepo.GetCategoria(id);

            if (categoria == null)
                return NotFound();

            var dto = _mapper.Map<CategoriaDto>(categoria);

            return Ok(dto);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategoria([FromBody] CrearCategoriaDto dto)
        {
            if (!ModelState.IsValid || dto == null)
                return BadRequest(ModelState);

            if (_ctRepo.ExisteCategoria(dto.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe");
                return StatusCode(404, ModelState);
            }

            var newCategoria = _mapper.Map<Categoria>(dto);

            if (!_ctRepo.CrearCategoria(newCategoria))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {newCategoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { id = newCategoria.Id }, newCategoria);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("id:int", Name = "ActualizarCategoria")]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarCategoria(int id, [FromBody] CategoriaDto dto)
        {
            if (!ModelState.IsValid || dto == null || id != dto.Id)
                return BadRequest(ModelState);

            var categoria = _mapper.Map<Categoria>(dto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { id = categoria.Id }, categoria);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("id:int", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BorrarCategoria(int id)
        {
            if (!_ctRepo.ExisteCategoria(id))
                return NotFound();

            var categoria = _ctRepo.GetCategoria(id);

            if (categoria == null)
                return BadRequest(ModelState);

            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}