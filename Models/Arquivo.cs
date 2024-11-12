using SiteSesc.Models.Atividade;
using SiteSesc.Models.EventoExterno;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SiteSesc.Models
{
    public class Arquivo
    {
        public Arquivo()
        {
            this.DataCadastro = DateTime.Now;
        }
        public int Id { get; set; }

        [Display(Name = "NomeArquivo")]
        public String NomeArquivo { get; set; }

        [Display(Name = "Extensao")]
        public String Extensao { get; set; }

        [Display(Name = "CaminhoAbsoluto")]
        public String CaminhoAbsoluto { get; set; }

        [Display(Name = "CaminhoVirtual")]
        public String CaminhoVirtual { get; set; }

        [Display(Name = "DataCadastro")]
        public DateTime DataCadastro { get; set; }

        public virtual ICollection<Noticia> Noticia { get; set; }
        public virtual ICollection<EventoAvulso> EventoAvulso { get; set; }
        public virtual ICollection<AtividadeOnLine> AtividadeOnLine { get; set; }
        public virtual ICollection<UnidadeOperacional> UnidadeOperacional { get; set; }
        public virtual ICollection<Banner> Banner { get; set; }
        public virtual ICollection<Area> Area { get; set; }
        public virtual ICollection<ArquivoCadastroCliente> ArquivoCadastroCliente { get; set; }
        public virtual ICollection<SubArea> SubArea { get; set; }

        public string CaminhoVirtualFormatado()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            string url = config.GetSection("HostConfig")["Url"];
            string pastaArquivos = config.GetSection("HostConfig")["PastaArquivosSite"];
            return $"{pastaArquivos}{CaminhoVirtual}";
        }
    }
}
