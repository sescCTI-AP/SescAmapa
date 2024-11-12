

namespace SiteSesc.Models.ModelPartialView
{
    public class EventoCard
    {
        public EventoCard(string EventTitle, string EventArea, string EventUnidade, string EventCidade, string EventDescription, DateTime EventDataInicio, DateTime EventDateFim, Guid EventId, string nameRoute2, string slug)
        {
            this.EventTitle = EventTitle;
            this.EventCategory = EventArea;
            this.EventLocation = EventUnidade;
            this.EventCidade = EventCidade;
            this.EventDescription = EventDescription;
            this.EventDataInicio = EventDataInicio;
            this.EventDataFim = EventDateFim;
            this.EventId = EventId;
            this.NameRoute = nameRoute2;
            this.Slug = slug;
        }

        public string EventImage { get; set; }
        public string EventTitle { get; set; }
        public string EventCategory { get; set; }
        public string EventLocation { get; set; }
        public string EventCidade { get; set; }
        public string EventDescription { get; set; }
        public DateTime EventDataInicio { get; set; }
        public DateTime EventDataFim { get; set; }
        public Guid EventId { get; set; }

        public string NameRoute { get; set; }

        public string Slug { get; set; }

    }
}
