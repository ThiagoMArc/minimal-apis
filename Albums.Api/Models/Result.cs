namespace Albums.Api.Models;
public class Result
{
    public Result(bool success, string message, object data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public bool Success { get; private set; }
    public string Message { get; private set; }
    public object Data { get; private set; }
}
