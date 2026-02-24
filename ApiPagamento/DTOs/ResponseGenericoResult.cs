namespace PagamentoApi.DTOs
{
    public class ResponseGenericoResult
    {
        public ResponseGenericoResult() { }

        public ResponseGenericoResult(bool success, string message, object content)
        {
            Success = success;
            Message = message;
            Content = content;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Content { get; set; }
    }
}
