namespace SiteSesc.Models.ModelPartialView
{
    public class PostsRecentes
    {
        public PostsRecentes(string Id, string postImage, DateTime postDate, string postTitle, string Slug, string areaRoute)
        {
            this.Id = Id;
            PostImage = postImage;
            PostDate = postDate;
            PostTitle = postTitle;
            slug = Slug;
            nameRoute = areaRoute;

        }

        public string Id { get; set; }
        public string PostImage { get; set; }
        public DateTime PostDate { get; set; }
        public string PostTitle { get; set; }
        public string slug { get; set; }
        public string nameRoute { get; set; }
    }
}
