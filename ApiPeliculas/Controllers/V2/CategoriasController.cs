using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/categorias")]
    [ApiVersion("2.0")]
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
        //[ResponseCache(Duration = 20)]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
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
    }
}