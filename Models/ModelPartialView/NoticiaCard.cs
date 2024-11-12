

namespace SiteSesc.Models.ModelPartialView
{
    public class NoticiaCard
    {
        public NoticiaCard(string Id, string eventImage, string eventCategory, string eventLocation, string Slug, string eventTitle, string areaRoute)
        {
            this.Id = Id;
            EventImage = eventImage;
            EventCategory = eventCategory;
            EventLocation = eventLocation;
            slug = Slug;
            nameRoute = areaRoute;
            EventTitle = eventTitle;

        }

        public string Id { get; set; }
        public string EventImage { get; set; }
        public string EventCategory { get; set; }
        public string EventLocation { get; set; }
        public string EventTitle { get; set; }
        public string slug { get; set; }
        public string nameRoute { get; set; }
    }
}
