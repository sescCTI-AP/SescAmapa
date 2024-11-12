namespace SiteSesc.Models.ModelPartialView
{
    public class ClienteDetails
    {
        public ClienteDetails(string name, string description, string imagePath, string cssClass, string fullTitle)
        {
            Name = name;
            Description = description;
            ImagePath = imagePath;
            CssClass = cssClass;
            FullTitle = fullTitle;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string CssClass { get; set; }
        public string FullTitle { get; set; }
    }
}
