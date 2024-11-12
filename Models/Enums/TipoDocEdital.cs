using System.ComponentModel;
using System.Reflection;

namespace SiteSesc.Models.Enums
{
    public enum TipoDocEdital
    {
        //comun a todos
        [Description("Curriculo")]
        Curriculo = 1,

        [Description("Diploma")]
        Diploma = 2,

        [Description("Experiencia")]
        Experiencia = 3,

                    [Description("DeclaraçãoRepresentacao")]
                    DeclaraçãoRepresentacao = 4,

        [Description("PropostaServico")]
        PropostaServico = 5,

        [Description("TermoAutorizacaoUsoImagemVoz")]
        TermoAutorizacaoUsoImagemVoz = 6,

        [Description("TermoResponsabilidadeCompromisso")]
        TermoResponsabilidadeCompromisso = 7,

        [Description("FichaCadastralPessoaFisica")]
        FichaCadastralPessoaFisica = 8,

        [Description("FichaCadastralPessoaJuridica")]
        FichaCadastralPessoaJuridica= 9,

        [Description("DeclaracaoAusenciaVinculo")]
        DeclaracaoAusenciaVinculo = 10,

        [Description("CurriculoArtistico")]
        CurriculoArtistico = 11,

        [Description("LGPD")]
        Lgpd =12,

        

        [Description("Cnpj")]
        Cnpj = 14,

        [Description("Proposta")]
        Proposta = 15,

        [Description("PortfolioArtistico")]
        PortfolioArtistico =16,

        [Description("FichaTecnica")]
        FichaTecnica = 17,


        //Pessoa Física
        [Description("RgCnh")]
        RgCnh = 18,

        [Description("DocumentoCpf")]
        DocumentoCpf= 19,

        [Description("NitPisPasep")]
        NitPisPasep = 20,

        [Description("ComprovanteBancario")]
        ComprovanteBancario = 21,

        [Description("CertificadoNivelEscolaridade")]
        CertificadoNivelEscolaridade = 22,

        [Description("ComprovanteResidencialAtualizado")]
        ComprovanteResidencialAtualizado = 23,



        //Pessoa Juridica
        [Description("FormularioInscricao")]
        FormularioInscricao = 24,

        [Description("Fgts")]
        Fgts = 25,

        [Description("Cseirelli")]
        Cseirelli = 26,

        [Description("Estatuto")]
        Estatuto = 27,

        [Description("Requerimentoei")]
        Requerimentoei = 28,

        [Description("CertificadoNegativaFederal")]
        CertificadoNegativaFederal = 29,

        [Description("CertificadoNegativaEstadual")]
        CertificadoNegativaEstadual = 30,

        [Description("EstatutoSocial")]
        EstatutoSocial = 31,

        [Description("RegularidadeFgts")]
        RegularidadeFgts = 32,


        //Pessoa Juridica MEI
        [Description("Mei")]
        Mei = 33,

        [Description("Ccmei")]
        Ccmei = 13,

        [Description("Cnpjmei")]
        Cnpjmei = 34,

        [Description("Fgtsmei")]
        Fgtsmei = 35,

        [Description("CertificadoNegativaFederalMei")]
        CertificadoNegativaFederalMei = 36,

        [Description("CertificadoNegativaEstadualMei")]
        CertificadoNegativaEstadualMei = 37,

        //Associação
        [Description("Cnpjacessoria")]
        Cnpjacessoria = 38,

        [Description("Fgtsacessoria")]
        Fgtsacessoria = 39,

        [Description("Cseirelliassociacao")]
        Cseirelliassociacao = 40,

        [Description("Estatutoassociacao")]
        Estatutoassociacao = 41,

        [Description("RequerimentoEiAssociacao")]
        RequerimentoEiAssociacao = 42,

        [Description("CertidaoNegativaFederalAssociacao")]
        CertidaoNegativaFederalAssociacao = 43,

        [Description("CertidaoNegativaEstadualAssociacao")]
        CertidaoNegativaEstadualAssociacao = 44,

        [Description("EstatutoSocialAssociacao")]
        EstatutoSocialAssociacao = 45,

        [Description("EstatutoAssociacaoRepresentante")]
        EstatutoAssociacaoRepresentante = 46,

        
    }



}