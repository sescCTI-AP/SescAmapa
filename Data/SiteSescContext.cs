using Microsoft.EntityFrameworkCore;
using SiteSesc.Models;
using SiteSesc.Models.Admin;
using SiteSesc.Models.Atividade;
using SiteSesc.Models.Licitacao;
using SiteSesc.Models.ProcessoSeletivo;
using SiteSesc.Models.Cardapio;
using SiteSesc.Models.Edital;
using SiteSesc.Models.Avaliacao;
using SiteSesc.Models.EventoExterno;
using System.Collections.Generic;
using SiteSesc.Models.MatriculaEscolar;
using SiteSesc.Models.Rematricula;

namespace SiteSesc.Data
{
    public class SiteSescContext : DbContext
    {
        public SiteSescContext(DbContextOptions<SiteSescContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        public DbSet<Noticia> Noticia { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Arquivo> Arquivo { get; set; }
        public DbSet<Banner> Banner { get; set; }
        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<SolicitacaoCadastroCliente> SolicitacaoCadastroCliente { get; set; }
        public DbSet<Parentesco> Parentesco { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<EstadoCivil> EstadoCivil { get; set; }
        public DbSet<Escolaridade> Escolaridade { get; set; }
        public DbSet<Sexo> Sexo { get; set; }
        public DbSet<SituacaoProfissional> SituacaoProfissional { get; set; }
        public DbSet<TipoDocumentoIdentificacao> TipoDocumentoIdentificacao { get; set; }
        public DbSet<ArquivoCadastroCliente> ArquivoCadastroCliente { get; set; }
        public DbSet<MensagemRapida> MensagemRapida { get; set; }
        public DbSet<BotaoSideMenu> BotaoSideMenu { get; set; }
        public DbSet<BotaoPerfil> BotaoPerfil { get; set; }
        public DbSet<PerfilUsuario> PerfilUsuario { get; set; }
        public DbSet<BotaoDropDown> BotaoDropDown { get; set; }
        public DbSet<HstSolicitacao> HstSolicitacao { get; set; }

        public DbSet<UnidadeOperacional> UnidadeOperacional { get; set; }
        public DbSet<AtividadeOnLine> AtividadeOnLine { get; set; }
        public DbSet<SubArea> SubArea { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Endereco> Endereco { get; set; }
        public DbSet<Telefone> Telefone { get; set; }
        public DbSet<EventoAvulso> EventoAvulso { get; set; }
        
        public DbSet<InscricaoEvento> InscricaoEvento { get; set; }

        public DbSet<mat_escolar> mat_escolar_inscritos { get; set; }
        public DbSet<mat_escolar> mat_escolar { get; set; }

        #region processo seletivo
        public DbSet<psl_areasProcessoSeletivo> psl_areasProcessoSeletivo { get; set; }
        public DbSet<psl_arquivosProcessoSeletivo> psl_arquivosProcessoSeletivo { get; set; }
        public DbSet<psl_cargoProcessoSeletivo> psl_cargoProcessoSeletivo { get; set; }
        public DbSet<psl_cidadesProcessoSeletivo> psl_cidadesProcessoSeletivo { get; set; }
        public DbSet<psl_observadoresProcessoSeletivo> psl_observadoresProcessoSeletivo { get; set; }
        public DbSet<psl_processoSeletivo> psl_processoSeletivo { get; set; }
        public DbSet<psl_statusProcessoSeletivo> psl_statusProcessoSeletivo { get; set; }
        public DbSet<psl_tipoDocumentoProcessoSeletivo> psl_tipoDocumentoProcessoSeletivo { get; set; }
        public DbSet<psl_documentoProcessoSeletivo> psl_documentoProcessoSeletivo { get; set; }
        public DbSet<psl_tipoProcessoSeletivo> psl_tipoProcessoSeletivo { get; set; }
        public DbSet<edt_curriculosProcessoSeletivo> psl_curriculosProcessoSeletivo { get; set; }

        #endregion

        #region Licitação

        public DbSet<lct_licitacao> lct_licitacao { get; set; }
        public DbSet<lct_arquivosLicitacao> lct_arquivosLicitacao { get; set; }
        public DbSet<lct_statusLicitacao> lct_statusLicitacao { get; set; }
        public DbSet<lct_tipoLicitacao> lct_tipoLicitacao { get; set; }
        public DbSet<lct_modalidade> lct_modalidade { get; set; }

        #endregion

        #region Edital
        public DbSet<edt_areasEdital> edt_areasEdital { get; set; }
        public DbSet<edt_arquivosEdital> edt_arquivosEdital { get; set; }
        public DbSet<edt_cargoEdital> edt_cargoEdital { get; set; }
        public DbSet<edt_cidadesEdital> edt_cidadesEdital { get; set; }
        public DbSet<edt_curriculosEdital> edt_curriculosEdital{ get; set; }
        public DbSet<edt_edital> edt_edital { get; set; }
        public DbSet<edt_modalidade> edt_modalidade { get; set; }
        public DbSet<edt_statusEdital> edt_statusEdital { get; set; }
        public DbSet<edt_tipoEdital> edt_tipoEdital { get; set; }
        public DbSet<edt_documentoEdital> edt_documentoEdital { get; set; }
        public DbSet<edt_observadoresEdital> edt_observadoresEdital{ get; set; }
        public DbSet<edt_statusCurriculoEdital> edt_statusCurriculoEdital { get; set; }
        public DbSet<edt_parecerEdital> edt_parecerEdital { get; set; }
        public DbSet<edt_tipoPessoaEdital> edt_tipoPessoaEdital { get; set; }

        public DbSet<edt_tipoDocumentoEdital> edt_tipoDocumentoEdital { get; set; }


        //   public DbSet<RequisicaoDto> RequisicaoDto{ get; set; }


        #endregion

        #region admin
        public DbSet<adm_acaoSistema> adm_acaoSistema { get; set; }
        public DbSet<adm_acoesUsuarioModuloSistema> adm_acoesUsuarioModuloSistema { get; set; }
        public DbSet<adm_moduloSistema> adm_moduloSistema { get; set; }
        public DbSet<adm_usuarioModuloSistema> adm_usuarioModuloSistema { get; set; }

        #endregion

        #region Cardapio
        public DbSet<cdp_Cardapio> cdp_Cardapio { get; set; }
        public DbSet<cdp_ComposicaoCardapio> cdp_ComposicaoCardapio { get; set; }
        public DbSet<cdp_GrupoItemCardapio> cdp_GrupoItemCardapio { get; set; }
        public DbSet<cdp_ItemCardapio> cdp_ItemCardapio { get; set; }

        #endregion


        #region Agendamento
        public DbSet<agd_tipoAgendamento> agd_tipoAgendamento { get; set; }
        public DbSet<agd_agendaAtividade> agd_agendaAtividade { get; set; }
        public DbSet<agd_agendamento> agd_agendamento { get; set; }
        public DbSet<agd_horariosAgenda> agd_horariosAgenda { get; set; }
        public DbSet<agd_agendaCliente> agd_agendaCliente { get; set; }
        public DbSet<agd_atividades> agd_atividades { get; set; }
        #endregion



        public DbSet<DashboardUsuario> DashboardUsuario { get; set; }
        public DbSet<TelaInicialAdmin> TelaInicialAdmin { get; set; }
        public DbSet<AvaliacaoAtividadeCliente> AvaliacaoAtividadeCliente { get; set; }
        public DbSet<Log> Log { get; set; }
    }
}
