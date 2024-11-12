namespace SiteSesc.Models.ApiPagamento
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Content { get; set; }

        public ApiResponse(bool success, string message, object content)
        {
            Success = success;
            Message = message;
            Content = content;
        }
    }

}
