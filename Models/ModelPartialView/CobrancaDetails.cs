namespace SiteSesc.Models.ModelPartialView
{
    public class CobrancaDetails
    {
        public CobrancaDetails(string iconClass, string title, string expirationDate, string value, string discountedValue)
        {
            IconClass = iconClass;
            Title = title;
            ExpirationDate = expirationDate;
            Value = value;
            DiscountedValue = discountedValue;
        }

        public string IconClass { get; set; }
        public string Title { get; set; }
        public string ExpirationDate { get; set; }
        public string Value { get; set; }
        public string DiscountedValue { get; set; }
    }
}
