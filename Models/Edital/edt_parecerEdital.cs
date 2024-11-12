namespace SiteSesc.Models.Edital
{
    public class edt_parecerEdital
    {
        public int Id { get; set; }

        public int IdEdital { get; set; }

        public string Cpf { get; set; }

        public int IdCargoEdital { get; set; }

        public int Status { get; set; }

        public string Justificativa { get; set; }

        public DateTime Data { get; set; }

    }
}
