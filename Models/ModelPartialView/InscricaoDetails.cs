namespace SiteSesc.Models.ModelPartialView
{
    public class InscricaoDetails
    {
        public InscricaoDetails(string title, List<string> links, List<string> linkIcons, List<string> linkDescriptions)
        {
            Title = title;
            Links = links;
            LinkIcons = linkIcons;
            LinkDescriptions = linkDescriptions;
        }

        public string Title { get; set; }
        public List<string> Links { get; set; }
        public List<string> LinkIcons { get; set; }
        public List<string> LinkDescriptions { get; set; }
    }
}
