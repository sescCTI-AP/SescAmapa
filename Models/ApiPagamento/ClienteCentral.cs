namespace SiteSesc.Models.ApiPagamento
{
    using SiteSesc.Helpers;
    using SiteSesc.Models.DB2;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.InteropServices;

    public class ClienteCentral
    {
        public int Id { get; set; }
        public int Sqmatric { get; set; }
        public int Cduop { get; set; }
        public int? Cdclassif { get; set; }
        public int Nudv { get; set; }
        public string Nucgccei { get; set; }

        [TemplateVariable("Categoria")]
        public string Categoria { get; set; }
        public int Cdcategori { get; set; }
        public int? Cdnivel { get; set; }
        public int? Sqtitulmat { get; set; }
        public int? Cduotitul { get; set; }
        public int Stmatric { get; set; }
        public DateTime Dtinscri { get; set; }
        public int? Cdmatriant { get; set; }
        public DateTime Dtvencto { get; set; }

        [TemplateVariable("NomeCliente")]
        public string Nmcliente { get; set; }

        [TemplateVariable("DataNascimento")]
        public DateTime Dtnascimen { get; set; }
        public string Nmpai { get; set; }
        public string Cdsexo { get; set; }
        public string Nmmae { get; set; }
        public int Cdestcivil { get; set; }
        public decimal Vbestudant { get; set; }
        public int? Nuultserie { get; set; }
        public string Dsnatural { get; set; }
        public string Dsnacional { get; set; }
        public int? Nudepend { get; set; }
        public string Nuctps { get; set; }
        public DateTime? Dtadmissao { get; set; }
        public DateTime? Dtdemissao { get; set; }
        public string Nureggeral { get; set; }
        public decimal Vlrenda { get; set; }

        public string Nucpf { get; set; }
        public string Nupispasep { get; set; }
        public string Dscargo { get; set; }
        public DateTime Dtemirg { get; set; }
        public int? Idorgemirg { get; set; }
        public string Dsparentsc { get; set; }
        public Byte[] Foto { get; set; }
        public DateTime Dtatu { get; set; }
        public bool? Stemicart { get; set; }
        public string Lgatu { get; set; }
        public int Smfieldatu { get; set; }
        public string Teobs { get; set; }
        public int Nrviacart { get; set; }
        public string Pswcli { get; set; }
        public int Numcartao { get; set; }
        public string Pswcrip { get; set; }
        public decimal? Vlrendafam { get; set; }
        public string Nmsocial { get; set; }
        public int Situprof { get; set; }
        public int Tipoidentidade { get; set; }
        public string Compidentidade { get; set; }
        public bool Vbpcg { get; set; }
        public bool Vbemancipado { get; set; }
        public bool Vbpcd { get; set; }
        public string Idnacional { get; set; }
        public string Inscricao { get; set; }
        public List<Cobranca>? Cobranca { get; set; }
        public List<Endereco>? Enderecos { get; set; }
        public object Respcli { get; set; }
        public object Uop { get; set; }

        [NotMapped]
        [TemplateVariable("Matricula")]
        public string Credencial => $"{Cduop.ToString().PadLeft(4, '0')}-{Sqmatric.ToString().PadLeft(6, '0')}-{Nudv}" ;

        [NotMapped]
        [TemplateVariable("NomeResponsavel")]
        public string NomeResponsavel
        {
            get => Nmcliente;
            set => Nmcliente = value;
        }

        [NotMapped]
        [TemplateVariable("RgResponsavel")]
        public string RgResponsavel
        {
            get => Nureggeral;
            set => Nureggeral = value;
        }

        [NotMapped]
        [TemplateVariable("CpfResponsavel")]
        public string CpfResponsavel
        {
            get => Nucpf;
            set => Nucpf = value;
        }


        [NotMapped]
        public string FotoCliente
        {
            get
            {
                if (Foto != null)
                {
                    var imgBase64Dados = Convert.ToBase64String(Foto);
                    var imagemCliente = $"data:image/png;base64,{imgBase64Dados}";
                    return imagemCliente;
                }
                return null;
            }
        }

        public static ClienteCentral SetResponsavel(ClienteCentral cliente, Responsavel responsavel)
        {
            cliente.CpfResponsavel = responsavel.CPF;
            cliente.NomeResponsavel = responsavel.NOME;
            cliente.RgResponsavel = responsavel.RG;
            return cliente;
        }
    }



    public class Cobranca
    {
        public int Id { get; set; }
        public string Idclasse { get; set; }
        public string Cdelement { get; set; }
        public int Sqcobranca { get; set; }
        public int Cduopcob { get; set; }
        public int Cduop { get; set; }
        public int Sqmatric { get; set; }
        public string Dscobranca { get; set; }
        public int Rfcobranca { get; set; }
        public decimal Vlcobrado { get; set; }
        public DateTime Dtvencto { get; set; }
        public DateTime Dtemissao { get; set; }
        public int Strecebido { get; set; }
        public int Tpcobranca { get; set; }
        public decimal Pcjuros { get; set; }
        public DateTime Dtatu { get; set; }
        public int Smfieldatu { get; set; }
        public string Lgatu { get; set; }
        public decimal? VlcaractE1 { get; set; }
        public decimal? VlcaractE2 { get; set; }
        public int Ddcobjuros { get; set; }
        public int Ddinijuros { get; set; }
        public decimal Pcmulta { get; set; }
        public string Dscancelam { get; set; }
        public int Cduoprec { get; set; }
        public string Nmestacao { get; set; }
        public string Cdcancela { get; set; }
        public int Tpmora { get; set; }
        public string Lgcancel { get; set; }
        public int? Idcobranca { get; set; }
        public List<object> Pagamentos { get; set; }
        public object Clientela { get; set; }
        public object Uop { get; set; }
    }

    public class Endereco
    {
        public int Id { get; set; }
        public string Idclasse { get; set; }
        public string Cdelement { get; set; }
        public int Sqenderec { get; set; }

        public string Dslogradou { get; set; }
        public string Siestado { get; set; }
        public string Dscomplem { get; set; }
        public int Cdmunicip { get; set; }
        public int? Nuimovel { get; set; }
        public string Dsbairro { get; set; }
        public string Nucep { get; set; }
        public int Stprincip { get; set; }
        public DateTime? Dtinicio { get; set; }
        public int Smfieldatu { get; set; }
        public DateTime Dtatu { get; set; }
        public string Lgatu { get; set; }
        public int? Cduop { get; set; }
        public int? Sqmatric { get; set; }
        public int? Idendereco { get; set; }
        public string Dsmunicip { get; set; }

        [TemplateVariable("EnderecoResponsavel")]
        public string EnderecoCompleto => $"{Dslogradou}, Nº {Nuimovel}, {Dsmunicip}-{Siestado} Bairro: {Dsbairro} CEP: {Nucep}";
    }
}