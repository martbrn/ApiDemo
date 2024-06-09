using System.Net;

namespace ApiPeliculas.Modelos;

public class RespuestaApi
{
    public RespuestaApi()
    {
    }

    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = true;
    public List<string> ErrorMessages { get; set; } = new();
    public object Result { get; set; }
}
