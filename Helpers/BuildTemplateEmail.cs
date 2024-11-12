namespace SiteSesc.Helpers
{
    public class BuildTemplateEmail
    {
        public static string Make(IWebHostEnvironment env, string pathTemplate, Dictionary<string, string> parameters)
        {
            var server = env.WebRootPath;
            var path = server + pathTemplate;
            var contentFile = File.ReadAllText(path);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    contentFile = contentFile.Replace(parameter.Key, parameter.Value);
                }
            }

            return contentFile;
        }
    }
}
