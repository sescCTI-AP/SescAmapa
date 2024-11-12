using Microsoft.Extensions.DependencyInjection;
using SiteSesc.Models.EventoExterno;
using SiteSesc.Repositories;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SiteSesc.Models.EventoExterno
{
    public class InscricaoEvento
    {
        public int Id { get; set; }
        public int CdPrograma { get; set; }
        public int CdConfig { get; set; }
        public int SqOcorrenc { get; set; }
        public string Nome { get; set; }
        public string NomeEvento { get; set; }
        public DateTime DataCadastro { get; set; }
        [Required(ErrorMessage = "Necessário informar este campo")]

        [Display(Name = "Data de nascimento")]
        public DateTime DataNascimento { get; set; }
        public string TxidPix { get; set; }

        [Display(Name = "CPF")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
        public string Email { get; set; }
        public string Sexo { get; set; }
        public string Cidade { get; set; }
        public string UnidadeOperacional { get; set; }
        public Guid Guid { get; set; }

        [ForeignKey("EventoAvulso")]
        public int? IdEvento { get; set; }

        [ForeignKey("IdEvento")]
        public virtual EventoAvulso EventoAvulso { get; set; }

        [ForeignKey("Usuario")]
        public int? IdUsuario { get; set; }

        [Display(Name = "Usuario")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        public bool? Pago { get; set; }

        public string NomeEquipe { get; set; }

        public string statusPagamento { get; set; }

        public InscricaoEvento() { }

        public InscricaoEvento(int cdprograma, int cdconfig, int sqocorrenc, int idevento, string nomeEvento, string nome,
            DateTime dataNascimento, string txidpix, string cpf, string telefone, string email, string sexo, string cidade, string unidade, int? idUsuario, string nomeEquipe)
        {
            CdPrograma = cdprograma;
            CdConfig = cdconfig;
            SqOcorrenc = sqocorrenc;
            IdEvento = idevento;
            NomeEvento = nomeEvento;
            Nome = nome;
            DataCadastro = DateTime.Now;
            DataNascimento = dataNascimento;
            TxidPix = txidpix;
            Cpf = cpf;
            Telefone = telefone;
            Email = email;
            Sexo = sexo;
            Cidade = cidade;
            UnidadeOperacional = unidade;
            Guid = Guid.NewGuid();
            IdUsuario = idUsuario;
            NomeEquipe = nomeEquipe;
        }
    }
}
