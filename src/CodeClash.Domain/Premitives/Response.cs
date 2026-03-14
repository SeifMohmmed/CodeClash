using System.Net;

namespace CodeClash.Domain.Premitives;
public class Response
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public bool IsSuccess { get; set; } = true;

    public object? Data { get; set; }

    public string Message { get; set; } = "";
}
