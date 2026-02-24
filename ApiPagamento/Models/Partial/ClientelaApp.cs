namespace PagamentoApi.Models.Partial
{
    public class ClientelaApp
    {
        public int Id { get; protected set; }
        public int SQMATRIC { get; set; }
        public int CDUOP { get; set; }
        public short CDCATEGORI { get; set; }
        public string NMCLIENTE { get; set; }
        public string NUREGGERAL { get; set; }
        public string NMSOCIAL { get; set; }
        public byte[] FOTO { get; set; }
        public System.DateTime DTNASCIMEN { get; set; }
        public System.DateTime DTVENCTO { get; set; }
        public string NUCPF { get; set; }
        public int? NUMCARTAO { get; set; }
        public bool CredencialAtiva
        {  
            get { return DTVENCTO.Date >= System.DateTime.Now.Date; }            
        }
        public string Matricula { get; set; }
        public string Categoria { get; set; }
        public int TipoCategoria { get; set; }
        public string Cdbarras { get; set; }

    }
}