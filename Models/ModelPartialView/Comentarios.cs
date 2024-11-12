

namespace SiteSesc.Models.ModelPartialView
{
    public class Comentarios
    {
        public Comentarios(string commentImage, string commentAuthor, DateTime commentDate, string commentContent)
        {
            CommentImage = commentImage;
            CommentAuthor = commentAuthor;
            CommentDate = commentDate;
            CommentContent = commentContent;
        }

        public string CommentImage { get; set; }
        public string CommentAuthor { get; set; }
        public DateTime CommentDate { get; set; }
        public string CommentContent { get; set; }
    }
}
