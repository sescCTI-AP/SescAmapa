using Microsoft.AspNetCore.Mvc;
using SiteSesc.Models.Atividade;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Models.ApiPagamento.Relatorios;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SiteSesc.Models.Avaliacao
{
    public class AvaliacaoAtividadeCliente
    {
        public int Id { get; set; }

        [FromForm(Name = "cdprograma")]
        public int cdprograma { get; set; }
        public int cdconfig { get; set; }
        public int sqocorrec { get; set; }
        public string comentario { get; set; }
        public int rating { get; set; }
        public DateTime? DataCadastro { get; set; }


        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [Display(Name = "USUÁRIO")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }



        [NotMapped]
        public string cdelement => $"{cdprograma.ToString().PadLeft(8, '0')}{cdconfig.ToString().PadLeft(8, '0')}{sqocorrec.ToString().PadLeft(8, '0')}";

        //[ForeignKey("AtividadeOnLine")]
        //public int? IdAtividadeOnLine { get; set; }

        //[Display(Name = "ATIVIDADE ONLINE")]
        //[ForeignKey("IdAtividadeOnLine")]
        //public virtual AtividadeOnLine AtividadeOnLine { get; set; }

    }
}
