using Microsoft.AspNetCore.Mvc;

namespace SiteSesc.Services
{
    public class SafeExecutor
    {
        public async Task<IActionResult> ExecuteSafe(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500); // Retorna Internal Server Error
            }
        }
    }
}
