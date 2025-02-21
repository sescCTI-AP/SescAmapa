using Microsoft.CodeAnalysis.CSharp.Syntax;
using SiteSesc.Models.ApiPagamento;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models
{
    public class SolicitacaoCadastroCliente
    {

        [Key]
        [Column("Guid")]
        public Guid Id { get; set; } = new Guid(Guid.NewGuid().ToString());

        [Display(Name = "Nome completo")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        public int? IdEmpresaSolicitacao { get; set; }

        [Display(Name = "Nome social")]
        public string? NomeSocial { get; set; }

        [Display(Name = "Nome do pai")]
        public string? NomePai { get; set; }
        public string? Cpf { get; set; }

        [Display(Name = "Nome da mãe")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string NomeMae { get; set; }

        [Display(Name = "Número de documento de identificação")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string NumeroDocumento { get; set; }

        [Display(Name = "Órgao Emissor de documento de identificação")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string OrgaoEmissor { get; set; }

        [Display(Name = "UF de documento de identificação")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(2)]
        public string UF { get; set; }

        [Display(Name = "Data de emissão de documento de identificação")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime DataEmissaoDocumento { get; set; }

        [Display(Name = "Renda individual")]
        public decimal RendaIndividual { get; set; }

        [Display(Name = "Renda familiar")]
        public decimal RendaFamiliar { get; set; }

        [Display(Name = "Data da solicitação")]
        public DateTime DataSolicitacao { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Naturalidade { get; set; }

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nacionalidade { get; set; }

        [Display(Name = "É Estudante")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public bool IsEstudante { get; set; }

        [Display(Name = "É Dependente")]
        public bool IsDependente { get; set; } = false;

        public bool IsRenovacao { get; set; } = false;

        [Display(Name = "Sexo")]
        public int IdSexo { get; set; }

        [Display(Name = "Sexo")]
        [ForeignKey("IdSexo")]
        public virtual Sexo Sexo { get; set; }

        [Display(Name = "Categoria")]
        public string TipoCategoria { get; set; }

        [Display(Name = "Parentesco")]
        public int? IdParentesco { get; set; }

        [Display(Name = "Parentesco")]
        [ForeignKey("IdParentesco")]
        public virtual Parentesco Parentesco { get; set; }

        [Display(Name = "Possui alguma deficiência")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public bool IsDeficiente { get; set; }

        [Display(Name = "Última série")]
        public string? UltimaSerie { get; set; }

        [Display(Name = "Tipo de documento")]
        public int IdTipoDocumentoIdentificacao { get; set; }

        [Display(Name = "Tipo de documento")]
        [ForeignKey("IdTipoDocumentoIdentificacao")]
        public virtual TipoDocumentoIdentificacao TipoDocumentoIdentificacao { get; set; }

        [Display(Name = "Data de nascimento")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime DataNascimento { get; set; }

        [Display(Name = "Escolaridade")]
        public int IdEscolaridade { get; set; }

        [Display(Name = "Escolaridade")]
        [ForeignKey("IdEscolaridade")]
        public virtual Escolaridade Escolaridade { get; set; }

        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Logradouro { get; set; }

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Cep { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Cidade { get; set; }

        [Display(Name = "Número")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string NumeroEndereco { get; set; }

        [Display(Name = "Complemento")]
        public string? Complemento { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Bairro { get; set; }

        [Display(Name = "Status")]
        public int IdStatus { get; set; } = 1;

        [Display(Name = "Status")]
        [ForeignKey("IdStatus")]
        public virtual Status Status { get; set; }

        [Display(Name = "Usuário")]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string TelefonePrimario { get; set; }

        [Display(Name = "Situação Profissional")]
        public int? IdSituacaoProfissional { get; set; }

        [Display(Name = "Situação Profissional")]
        [ForeignKey("IdSituacaoProfissional")]
        public virtual SituacaoProfissional SituacaoProfissional { get; set; }

        [Display(Name = "Cargo")]
        public string? Cargo { get; set; }

        [Display(Name = "Número da Carteira Prossional")]
        public string? NumeroCarteiraProssional { get; set; }

        [Display(Name = "Série Ctps")]
        public string? SerieCtps { get; set; }

        [Display(Name = "Pis/Pasep")]
        public string? PisPasep { get; set; }

        [Display(Name = "Data de admissão na empresa")]
        public DateTime? DataAdmissao { get; set; }

        [Display(Name = "Data de demissão")]
        public DateTime? DataDemissao { get; set; }

        [Display(Name = "Telefone secundário")]
        public string? TelefoneSecundario { get; set; }

        [Display(Name = "Estado Civil")]
        public int IdEstadoCivil { get; set; }

        [Display(Name = "Estado Civil")]
        [ForeignKey("IdEstadoCivil")]
        public virtual EstadoCivil EstadoCivil { get; set; }

        [Display(Name = "Número")]
        public string? CnpjEmpresa { get; set; }

        [Display(Name = "Razão Social")]
        public string? RazaoSocial { get; set; }

        //TODO: Verificar se vale a pena criar um enum
        //0 = Novo cadastro | 1 = Novo dependente | 2 = Renovação | 3 = Enviado para correção
        public int TipoSolicitacao { get; set; } = 0;
        public int? Cduotitul { get; set; }
        public int? Sqtitulmat { get; set; }

        public virtual ICollection<HstSolicitacao> HstSolicitacao { get; set; }

        [NotMapped]
        [Display(Name = "COMPROVANTE DE ENDEREÇO")]
        public IFormFile ComprovanteEndereco { get; set; }

        [NotMapped]
        public IFormFile FotoPerfil { get; set; }

        [NotMapped]
        public IFormFile DocumentoCpf { get; set; }

        [NotMapped]
        public IFormFile RgFrente { get; set; }

        [NotMapped]
        public IFormFile RgVerso { get; set; }

        [NotMapped]
        public IFormFile VinculoPrimeiraPagina { get; set; }

        [NotMapped]
        public IFormFile VinculoSegundaPagina { get; set; }

        [NotMapped]
        public IFormFile ContraCheque { get; set; }

        public static SolicitacaoCadastroCliente ToSolicitacaoCadastro(CLIENTELA cliente)
        {
            return new SolicitacaoCadastroCliente
            {
                Nome = cliente.NMCLIENTE,
                NomeSocial = cliente.NMSOCIAL,
                NomeMae = cliente.NMMAE,
                NomePai = cliente.NMPAI,
                Cpf = cliente.NUCPF,
                DataNascimento = cliente.DTNASCIMEN,
                IdSexo = cliente.CDSEXO == "0" ? 0 : 1,
            };
        }

        public static ClienteAdd ToClienteAdd(SolicitacaoCadastroCliente s, string cpf)
        {
            short cdcategoria = s.TipoCategoria.ToLower() == "pleno" ? (short)1 : (short)6;
            if (s.Cduotitul != null)            
                cdcategoria = s.TipoCategoria.ToLower() == "pleno" ? (short)2 : (short)11;
            
            if (!string.IsNullOrEmpty(cpf))
            {
                var cliente = new ClienteAdd
                {
                    NUCGCCEI = s.CnpjEmpresa,
                    CDCATEGORI = cdcategoria,
                    CDNIVEL = Convert.ToInt16(s.IdEscolaridade),
                    NUDV = 0,
                    SQTITULMAT = s.Sqtitulmat,
                    CDUOTITUL = s.Cduotitul,
                    STMATRIC = 1,
                    CDMATRIANT = "",
                    CDCLASSIF = "",
                    NMCLIENTE = s.Nome,
                    DTNASCIMEN = s.DataNascimento,
                    NMPAI = s.NomePai,
                    NMMAE = s.NomeMae,
                    DTINSCRI = DateTime.Now,
                    DTATU = DateTime.Now,
                    DTVENCTO = DateTime.Now.AddYears(2),
                    CDSEXO = s.IdSexo == 2 ? "1" : "0",
                    CDESTCIVIL = (short)s.IdEstadoCivil,
                    VBESTUDANT = s.IsEstudante ? (short)1 : (short)0,
                    NUULTSERIE = null,
                    DSNATURAL = s.Naturalidade,
                    DSNACIONAL = s.Nacionalidade,
                    NUDEPEND = null,
                    NUCTPS = s.NumeroCarteiraProssional,
                    DTADMISSAO = s.DataAdmissao,
                    DTDEMISSAO = s.DataDemissao,
                    NUREGGERAL = s.NumeroDocumento,
                    VLRENDA = s.RendaIndividual,
                    VLRENDAFAM = s.RendaFamiliar,
                    NUCPF = cpf,
                    NUPISPASEP = s.PisPasep,
                    DSCARGO = s.Cargo,
                    DTEMIRG = s.DataEmissaoDocumento,
                    IDORGEMIRG = s.OrgaoEmissor,
                    DSPARENTSC = null,
                    FOTO = null,
                    STEMICART = 1,
                    TEOBS = "Cadastro submetido via web",
                    NRVIACART = 1,
                    NMSOCIAL = s.NomeSocial,
                    TELEFONE = s.TelefonePrimario,
                    SITUPROF = 0,
                    TIPOIDENTIDADE = 0,
                    COMPIDENTIDADE = "",
                    VBPCD = s.IsDeficiente ? (short)1 : (short)0,
                    VBPCG = 0,
                    VBEMANCIPADO = 0,
                    STONLINE = 0,
                    VBNOMEAFETIVO = 0
                };
                //Mudar a var NUIMOVEL, da classe EnderecoAdd, pra receber string
                var endereco = new EnderecoAdd
                {
                    IDCLASSE = "CLI01",
                    CDELEMENT = "",
                    SQENDEREC = 1,
                    DSLOGRADOU = s.Logradouro,
                    SIESTADO = s.UF,
                    DSCOMPLEM = s.Complemento,
                    CDMUNICIP = (short)Convert.ToInt32(s.Cidade),
                    NUIMOVEL = Convert.ToInt32(s.NumeroEndereco),
                    DSBAIRRO = s.Bairro,
                    NUCEP = s.Cep.Replace("-","").Replace(".",""),
                    STPRINCIP = 1,
                    SMFIELDATU = 0,
                    DSMUNICIP = s.Cidade,
                    LGATU = "XPTO"
                };

                cliente.ENDERECO = endereco;

                return cliente;
            }
            return null;
        }

    }
}