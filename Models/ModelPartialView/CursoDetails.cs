

namespace SiteSesc.Models.ModelPartialView
{
    public class CursoItem
    {
        public CursoItem(int idCategory, string cdelement, string courseImage, string courseTitle, string courseCategory, string courseLocation, string coursePrice, string courseDescription, List<string> courseDetails)
        {
            IdCategory = idCategory;
            CourseImage = courseImage;
            CourseTitle = courseTitle;
            CourseCategory = courseCategory;
            CourseLocation = courseLocation;
            CoursePrice = coursePrice;
            CourseDescription = courseDescription;
            CourseDetails = courseDetails;
            CdElement = cdelement;
        }

        public int IdCategory { get; set; }
        public string CourseImage { get; set; }
        public string CdElement { get; set; }
        public string CourseTitle { get; set; }
        public string CourseCategory { get; set; }
        public string CourseLocation { get; set; }
        public string CoursePrice { get; set; }
        public string CourseDescription { get; set; }
        public List<string> CourseDetails { get; set; }
    }
}
