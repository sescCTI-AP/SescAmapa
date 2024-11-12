using Microsoft.AspNetCore.Mvc;

namespace SiteSesc.Helpers
{
    public class JsonViewReturn
    {
        public static JsonResult JsonSuccess(string message) => new JsonResult(new { code = 1, message = message, classAlert = "success" });
        public static JsonResult JsonWarning(string message) => new JsonResult(new { code = 2, message = message, classAlert = "warning" });
        public static JsonResult JsonError(string message) => new JsonResult(new { code = 0, message = message, classAlert = "error" });
    }
}
