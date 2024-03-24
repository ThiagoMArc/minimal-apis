namespace Albums.Api.Models;
public class Result(bool success, string message, object data)
{
    public bool Success { get; private set; } = success;
    public string Message { get; private set; } = message;
    public object Data { get; private set; } = data;
}
