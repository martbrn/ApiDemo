using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controllers
{
    [Route("api/v{version:apiVersion}/usuarios")]
    //[ApiVersion("1.0")]
    [ApiVersionNeutral]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usrRepo;
        protected RespuestaApi _respuestaApi;
        private readonly IMapper _mapper;

        public UsuariosController(IUsuarioRepositorio usrRepo, IMapper mapper)
        {
            _usrRepo = usrRepo;
            _respuestaApi = new();
            _mapper = mapper;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var listaUsuario = _usrRepo.GetUsuarios();

            var listaUsuarioDto = new List<UsuarioDto>();

            foreach (var item in listaUsuario)
            {
                listaUsuarioDto.Add(_mapper.Map<UsuarioDto>(item));
            }
            return Ok(listaUsuarioDto);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuario(string id)
        {
            var Usuario = _usrRepo.GetUsuario(id);

            if (Usuario == null)
                return NotFound();

            var dto = _mapper.Map<UsuarioDto>(Usuario);

            return Ok(dto);
        }

        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(201, Type = typeof(UsuarioDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto registro)
        {
            if (!_usrRepo.IsUniqueUser(registro.NombreUsuario))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = await _usrRepo.Registro(registro);

            if (usuario == null)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Error en el registro");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            return Ok(_respuestaApi);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(201, Type = typeof(UsuarioDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto login)
        {
            var respuestaLogin = await _usrRepo.Login(login);

            if (respuestaLogin == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("El nombre de usuario/password son incorrectos");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.Result = respuestaLogin;
            return Ok(_respuestaApi);
        }
    }
}