using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.EventoExterno
{
    public class EventoAvulso
    {
        public int Id { get; set; }

        [Display(Name = "NOME EVENTO")]
        public string NomeEvento { get; set; }

        [Display(Name = "DATA DE INÍCIO DA INSCRIÇÃO")]
        public DateTime DataInicio { get; set; }

        [Display(Name = "DATA DE TÉRMINO DA INSCRIÇÃO")]
        public DateTime DataFim { get; set; }

        [Display(Name = "VALOR")]
        public decimal Valor { get; set; }

        [Display(Name = "TERMO DE ADESÃO")]
        public string TermoAdesao { get; set; }

        [Display(Name = "QUANTIDADE DE VAGAS")]
        public int? QtdVagas { get; set; }

        [Display(Name = "IDADE MÍNIMA")]
        public int? IdadeMin { get; set; }

        [Display(Name = "IDADE MÁXIMA")]
        public int? IdadeMax { get; set; } 
        public bool IsGratuito { get; set; }
        [ForeignKey("IsAtivo")]
        public bool IsAtivo { get; set; } = true;

        [Display(Name = "DATA DO EVENTO")]
        public DateTime DataEvento { get; set; }

        [Display(Name = "DOAÇÃO")]
        public string Doacao { get; set; }
        
        [Display(Name = "UNIDADE")]
        public int? Unidade { get; set; }

        [ForeignKey("Unidade")]

        public virtual UnidadeOperacional IdUnidade { get; set; }

        [Display(Name = "LOCAL DO EVENTO")]
        public string LocalEvento { get; set; }

        [Display(Name = "INSTRUCOES ADICIONAIS")]
        public string InstrucaoAdicional { get; set; }


        [ForeignKey("Arquivo")]
        public int? IdArquivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]

        public virtual Arquivo Arquivo { get; set; }

        public virtual ICollection<InscricaoEvento> InscricaoEvento { get; set; }



    }
}
