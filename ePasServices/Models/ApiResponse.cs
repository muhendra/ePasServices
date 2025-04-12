public class ApiResponse
{
    public string Time { get; set; } = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
    public string Message { get; set; }
    public object Data { get; set; }

    public ApiResponse(string message, object data)
    {
        Message = message;
        Data = data;
    }
}
