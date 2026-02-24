namespace PagamentoApi.Models.Pix
{
    public class ResponseResult<T>
    {
        public ResponseResult() { }

        public ResponseResult(bool success, string message, T content)
        {
            Success = success;
            Message = message;
            Content = content;
        }

        public bool Success { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Content { get; set; }
    }
}
