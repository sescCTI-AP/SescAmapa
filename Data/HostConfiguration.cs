namespace SiteSesc.Data
{
    public class HostConfiguration
    {
        public string Url { get; set; }
        public string HostServidorArquivos { get; set; }
        public string HostFisicoServidorArquivos { get; set; }
        public string HostFisico { get; set; }
        public string PastaArquivosSite { get; set; }
        public string PastaFisicaArquivosSite { get; set; }
        public string EnderecoFisico { get; set; }
        public string EnderecoVirtual { get; set; }
        public string NomeEmailRemetente { get; set; }
        public string EmailRemetente { get; set; }
        public string SenhaEmailRemetente { get; set; }
        public string SmtpEmail { get; set; }
        public string PortaSmtpEmail { get; set; }
    }
}
