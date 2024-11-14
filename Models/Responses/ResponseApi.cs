using System.Text.Json.Serialization;

namespace SiteSesc.Models.Responses
{
    public class ResponseApi<TContent>
    {
        public ResponseApi(TContent content, string? message = null, bool isSuccess = true)
        {
            Content = content;
            Message = message;
            IsSuccess = isSuccess;
        }
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; private set; }

        [JsonPropertyName("message")]
        public string? Message { get; private set; }

        [JsonPropertyName("content")]
        public TContent Content { get; private set; }
    }
}
