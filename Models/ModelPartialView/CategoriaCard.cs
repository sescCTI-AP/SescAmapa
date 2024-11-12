namespace SiteSesc.Models.ModelPartialView
{
    public class CategoriaCard
    {
        public CategoriaCard(string categoryImage, string categoryName, int courseCount)
        {
            CategoryImage = categoryImage;
            CategoryName = categoryName;
            CourseCount = courseCount;
        }

        public string CategoryImage { get; set; }
        public string CategoryName { get; set; }
        public int CourseCount { get; set; }
    }
}
