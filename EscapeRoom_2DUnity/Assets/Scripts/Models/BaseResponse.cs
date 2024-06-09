namespace Models
{
    public class BaseResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        // public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }

    public class BaseResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
        // public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}