using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Enums;
using PagamentoApi.Models;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Site;
using PagamentoApi.Settings;

namespace PagamentoApi.Repositories
{
    public class AtividadesRepository
    {
        public readonly IConfiguration configuration;
        private readonly CaixaSettings caixaConfiguration;
        private readonly ClientelaRepository clientelaRepository;

        public AtividadesRepository(IConfiguration configuration, ClientelaRepository clientelaRepository)
        {
            this.clientelaRepository = clientelaRepository;
            this.configuration = configuration;
            caixaConfiguration = configuration.GetSection("CaixaSettings").Get<CaixaSettings>();
        }

        /// <summary>
        /// Obtem atividades que o cliente está inscrito
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public async Task<List<INSCRICAO>> ObterAtividadesPorCliente(CLIENTELA cliente)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var caixaDictionary = new Dictionary<string, INSCRICAO>();
                var sql = "SELECT * from INSCRICAO I inner join PROGOCORR P ON P.CDPROGRAMA = I.CDPROGRAMA AND P.CDCONFIG = I.CDCONFIG AND P.SQOCORRENC = I.SQOCORRENC " +
                    " WHERE I.CDUOP = @CDUOP and I.SQMATRIC = @SQMATRIC AND I.STINSCRI != 3 AND P.DTFIMOCORR > current date ORDER BY DTINSCRI DESC";
                var inscricoes = connection.Query<INSCRICAO, PROGOCORR, INSCRICAO>(
                        sql,
                        (i, p) =>
                        {
                            i.PROGOCORR = p;
                            return i;
                        },
                        cliente, splitOn: "CDPROGRAMA");
                return inscricoes.AsList();
            }
        }

        /// <summary>
        /// Obtem atividades que o cliente está inscrito
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public async Task<List<INSCRICAO>> ObterAtividadesInativasPorCliente(CLIENTELA cliente)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var caixaDictionary = new Dictionary<string, INSCRICAO>();
                var sql = "SELECT * from INSCRICAO I inner join PROGOCORR P ON P.CDPROGRAMA = I.CDPROGRAMA AND P.CDCONFIG = I.CDCONFIG AND P.SQOCORRENC = I.SQOCORRENC " +
                    " WHERE I.CDUOP = @CDUOP and I.SQMATRIC = @SQMATRIC AND I.STINSCRI = 3 ORDER BY DTINSCRI DESC";
                var inscricoes = connection.Query<INSCRICAO, PROGOCORR, INSCRICAO>(
                        sql,
                        (i, p) =>
                        {
                            i.PROGOCORR = p;
                            return i;
                        },
                        cliente, splitOn: "CDPROGRAMA");
                return inscricoes.AsList();
            }
        }

        public async Task<int> ObterCdMapa(int cdprograma)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT CDMAPA FROM PROGSUBMOD WHERE CDPROGRAMA = @cdprograma";
                var map = await connection.QueryFirstAsync<int>(sql, new { cdprograma });
                return map;
            }
        }

        public async Task<string> ObterTermoDeAdesao(int cdprograma)
        {
            var cdmapa = await ObterCdMapa(cdprograma);
            var ano = DateTime.Now.Year;
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                var sql = @$"SELECT Conteudo FROM TemplateTermoDeAdesao where CdMapa = {cdmapa} and Ano = {ano} and IsAtivo = 1";
                var termo = await connection.QueryFirstOrDefaultAsync<string>(sql);
                return termo;
            }
        }

        public async Task<string> ObterTermoDeAdesaoPorAno(int cdprograma, int ano)
        {
            var cdmapa = await ObterCdMapa(cdprograma);
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                var sql = @$"SELECT Conteudo FROM TemplateTermoDeAdesao where CdMapa = {cdmapa} and Ano = {ano} and IsAtivo = 1";
                var termo = await connection.QueryFirstOrDefaultAsync<string>(sql);
                return termo;
            }
        }

        /// <summary>
        /// Obtem lista com todas as atividades atuais do regional
        /// </summary>
        /// <param name="uop"></param>
        /// <returns></returns>
        public async Task<List<PROGRAMAS>> ObterAtividades(int uop)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT PR.* FROM PROGRAMAS PR " +
                    "INNER JOIN PROGSUBMOD PS ON PR.CDPROGRAMA = PS.CDPROGRAMA " +
                    "INNER JOIN MAPA MP ON MP.CDMAPA = PS.CDMAPA " +
                    "WHERE PR.STATUS = 1 AND PR.CDUOP = @uop AND " +
                    "PS.ANOPROG = (SELECT MAX(ANOPROG) FROM PROGSUBMOD PS1) " +
                    "ORDER BY PR.CDPROGSUP, PR.NMPROGRAMA, PR.CDPROGRAMA ";
                var atividadesBD = (await connection.QueryAsync<PROGRAMAS>(
                    sql,
                    new
                    {
                        uop
                    })).AsList();
                var atividades = atividadesBD.FindAll(a => a.CDPROGSUP == null);
                foreach (var atividade in atividades)
                {
                    atividade.SUBPROGRAMAS = atividadesBD.FindAll(a => a.CDPROGSUP == atividade.CDPROGRAMA);
                    foreach (var subAtividade in atividade.SUBPROGRAMAS)
                    {
                        subAtividade.SUBPROGRAMAS = atividadesBD.FindAll(a => a.CDPROGSUP == subAtividade.CDPROGRAMA);
                        foreach (var subSubAtividade in atividade.SUBPROGRAMAS)
                        {
                            subSubAtividade.SUBPROGRAMAS = atividadesBD.FindAll(a => a.CDPROGSUP == subSubAtividade.CDPROGRAMA);
                        }
                    }
                }

                return atividades;
            }
        }

        public async Task<List<PROGOCORR>> ObterTurmasPorUnidade(int uop)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = " SELECT P.* from PROGOCORR AS P " +
                    " INNER JOIN PROGRAMAS AS PR ON P.CDPROGRAMA = PR.CDPROGRAMA " +
                    "WHERE DTFIMOCORR > current date AND CDUOP = @UOP ORDER BY DSUSUARIO";
                var turmas = (await connection.QueryAsync<PROGOCORR>(
                    sql,
                    new
                    {
                        uop
                    })).AsList();

                foreach (var turma in turmas)
                {
                    turma.FORMASPGTO = await this.ObterFormatoPgto(turma.CDPROGRAMA, turma.CDCONFIG);
                    turma.Horarios = await this.ObterHorarios(turma);
                }

                return turmas;
            }
        }

        public async Task<List<ModalidadeCategoria>> ObterTurmasPorUnidadeEData(int uop, int? ano = null, int? mes = null)
        {

            if (ano == null)
            {
                ano = DateTime.Now.Year;
            }

            if (mes == null)
            {
                mes = DateTime.Now.Month;
            }

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();

                string sql = "";

                sql += "SELECT ";
                sql += "    LISUB.CDELEMENT, ";
                sql += "    LISUB.CATEGORIACLIENTE, ";
                sql += "    LISUB.ECOMERCIARIO, ";
                sql += "    LISUB.QTDPORCATEGORIA, ";
                sql += "    PO.DSUSUARIO, ";
                sql += "    PO.NUVAGAS, ";
                sql += "    PO.NUVAGASOCP, ";
                sql += "    PO.NUMINVOCUP, ";
                sql += "    PO.CDUOPCAD, ";
                sql += "    PO.AAMODA ";
                sql += "FROM ";
                sql += "    ( ";
                sql += "    SELECT ";
                sql += "        LI.CDELEMENT, ";
                sql += "        CASE ";
                sql += "            WHEN LI.ECOMERCIARIO = 1 THEN 'TRABALHADOR DO COMÉRCIO' ";
                sql += "            ELSE 'PÚBLICO GERAL' ";
                sql += "        END AS CATEGORIACLIENTE, ";
                sql += "        LI.ECOMERCIARIO, ";
                sql += "        COUNT(*) AS QTDPORCATEGORIA ";
                sql += "    FROM ";
                sql += "        ( ";
                sql += "        SELECT ";
                sql += "            LPAD(LI.CDPROGRAMA, 8, '0') || LPAD(LI.CDCONFIG, 8, '0') || LPAD(LI.SQOCORRENC, 8, '0') AS CDELEMENT, ";
                sql += "            LI.ECOMERCIARIO ";
                sql += "        FROM ";
                sql += "            ( ";
                sql += "            SELECT ";
                sql += "                LI.CDUOP, ";
                sql += "                LI.SQMATRIC, ";
                sql += "                LI.CDPROGRAMA, ";
                sql += "                LI.CDCONFIG, ";
                sql += "                LI.SQOCORRENC, ";
                sql += "                LI.STINSCRI, ";
                sql += "                LI.CDCATEGORI, ";
                sql += "                LI.DTATU, ";
                sql += "                LI.HRATU, ";
                sql += "                CASE ";
                sql += "                    WHEN C.TPCATEGORI = 0 OR C.TPCATEGORI = 1 THEN 1 ";
                sql += "                    ELSE 0 ";
                sql += "                END AS ECOMERCIARIO, ";
                sql += "                ROW_NUMBER() OVER (PARTITION BY LI.CDUOP, LI.SQMATRIC, LI.CDPROGRAMA, LI.CDCONFIG, LI.SQOCORRENC ORDER BY LI.DTATU DESC, LI.HRATU DESC) AS rn ";
                sql += "            FROM ";
                sql += "                LOG_INSCRICAO LI ";
                sql += "            JOIN CATEGORIA C ON ";
                sql += "                LI.CDCATEGORI = C.CDCATEGORI ";
                sql += "            ) AS LI ";
                sql += "        WHERE ";
                sql += "            LI.rn = 1 ";
                sql += "            AND LI.STINSCRI = 0 ";
                sql += "            AND ( ";
                sql += "                YEAR(LI.DTATU) < @ANO ";
                sql += "                OR ";
                sql += "                (YEAR(LI.DTATU) = @ANO AND MONTH(LI.DTATU) <= @MES) ";
                sql += "            ) ";
                sql += "        ) AS LI ";
                sql += "    GROUP BY ";
                sql += "        LI.CDELEMENT, ";
                sql += "        LI.ECOMERCIARIO ";
                sql += "    ) AS LISUB ";
                sql += "JOIN ";
                sql += "    ( ";
                sql += "    SELECT ";
                sql += "        LPAD(PO.CDPROGRAMA, 8, '0') || LPAD(PO.CDCONFIG, 8, '0') || LPAD(PO.SQOCORRENC, 8, '0') AS CDELEMENT, ";
                sql += "        PO.DSUSUARIO, ";
                sql += "        PO.NUVAGAS, ";
                sql += "        PO.NUVAGASOCP, ";
                sql += "        PO.NUMINVOCUP, ";
                sql += "        PO.CDUOPCAD, ";
                sql += "        PO.AAMODA ";
                sql += "    FROM ";
                sql += "        PROGOCORR PO ";
                sql += "    ) PO ON ";
                sql += "    PO.CDELEMENT = LISUB.CDELEMENT ";
                sql += "WHERE ";
                sql += "    PO.AAMODA = @ANO ";
                sql += "    AND PO.CDUOPCAD = @UOP;";

                var modalidadesCategorias = (await connection.QueryAsync<dynamic>(sql, new
                {
                    UOP = uop,
                    ANO = ano,
                    MES = mes
                })).AsList();

                return modalidadesCategorias.Select(mc => new ModalidadeCategoria
                {
                    CdElement = mc.CDELEMENT,
                    CategoriaCliente = mc.CATEGORIACLIENTE,
                    EComerciario = mc.ECOMERCIARIO == 1,
                    QtdPorCategoria = mc.QTDPORCATEGORIA,
                    DsUsuario = mc.DSUSUARIO,
                    NuVagas = mc.NUVAGAS,
                    NuVagasOcp = mc.NUVAGASOCP,
                    NuMinOcup = mc.NUMINVOCUP,
                    CdUopCad = mc.CDUOPCAD,
                    AaModa = mc.AAMODA
                }).ToList();

            }

        }

        public async Task<List<ModalidadeCategoria>> ObterTurmasPorUnidadeEData(int uop, DateTime data)
        {

            //if (ano == null)
            //{
            //    ano = DateTime.Now.Year;
            //}

            //if (mes == null)
            //{
            //    mes = DateTime.Now.Month;
            //}

            //se data for valor default então recebe a data atual
            if (data == default)
            {
                data = DateTime.Now;
            }


            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();

                string sql = "";

                sql += "SELECT ";
                sql += "    LISUB.CDELEMENT, ";
                sql += "    LISUB.CATEGORIACLIENTE, ";
                sql += "    LISUB.ECOMERCIARIO, ";
                sql += "    LISUB.QTDPORCATEGORIA, ";
                sql += "    PO.DSUSUARIO, ";
                sql += "    PO.NUVAGAS, ";
                sql += "    PO.NUVAGASOCP, ";
                sql += "    PO.NUMINVOCUP, ";
                sql += "    PO.CDUOPCAD, ";
                sql += "    PO.AAMODA ";
                sql += "FROM ";
                sql += "    ( ";
                sql += "    SELECT ";
                sql += "        LI.CDELEMENT, ";
                sql += "        CASE ";
                sql += "            WHEN LI.ECOMERCIARIO = 1 THEN 'TRABALHADOR DO COMÉRCIO' ";
                sql += "            ELSE 'PÚBLICO GERAL' ";
                sql += "        END AS CATEGORIACLIENTE, ";
                sql += "        LI.ECOMERCIARIO, ";
                sql += "        COUNT(*) AS QTDPORCATEGORIA ";
                sql += "    FROM ";
                sql += "        ( ";
                sql += "        SELECT ";
                sql += "            LPAD(LI.CDPROGRAMA, 8, '0') || LPAD(LI.CDCONFIG, 8, '0') || LPAD(LI.SQOCORRENC, 8, '0') AS CDELEMENT, ";
                sql += "            LI.ECOMERCIARIO ";
                sql += "        FROM ";
                sql += "            ( ";
                sql += "            SELECT ";
                sql += "                LI.CDUOP, ";
                sql += "                LI.SQMATRIC, ";
                sql += "                LI.CDPROGRAMA, ";
                sql += "                LI.CDCONFIG, ";
                sql += "                LI.SQOCORRENC, ";
                sql += "                LI.STINSCRI, ";
                sql += "                LI.CDCATEGORI, ";
                sql += "                LI.DTATU, ";
                sql += "                LI.HRATU, ";
                sql += "                CASE ";
                sql += "                    WHEN C.TPCATEGORI = 0 OR C.TPCATEGORI = 1 THEN 1 ";
                sql += "                    ELSE 0 ";
                sql += "                END AS ECOMERCIARIO, ";
                sql += "                ROW_NUMBER() OVER (PARTITION BY LI.CDUOP, LI.SQMATRIC, LI.CDPROGRAMA, LI.CDCONFIG, LI.SQOCORRENC ORDER BY LI.DTATU DESC, LI.HRATU DESC) AS rn ";
                sql += "            FROM ";
                sql += "                LOG_INSCRICAO LI ";
                sql += "            JOIN CATEGORIA C ON ";
                sql += "                LI.CDCATEGORI = C.CDCATEGORI ";
                sql += "            ) AS LI ";
                sql += "        WHERE ";
                sql += "            LI.rn = 1 ";
                sql += "            AND LI.STINSCRI = 0 ";
                //sql += "            AND ( ";
                //sql += "                YEAR(LI.DTATU) < @ANO ";
                //sql += "                OR ";
                //sql += "                (YEAR(LI.DTATU) = @ANO AND MONTH(LI.DTATU) <= @MES) ";
                //sql += "            ) ";
                sql += "            AND DATE(LI.DTATU) <= @Data  ";
                sql += "        ) AS LI ";
                sql += "    GROUP BY ";
                sql += "        LI.CDELEMENT, ";
                sql += "        LI.ECOMERCIARIO ";
                sql += "    ) AS LISUB ";
                sql += "JOIN ";
                sql += "    ( ";
                sql += "    SELECT ";
                sql += "        LPAD(PO.CDPROGRAMA, 8, '0') || LPAD(PO.CDCONFIG, 8, '0') || LPAD(PO.SQOCORRENC, 8, '0') AS CDELEMENT, ";
                sql += "        PO.DSUSUARIO, ";
                sql += "        PO.NUVAGAS, ";
                sql += "        PO.NUVAGASOCP, ";
                sql += "        PO.NUMINVOCUP, ";
                sql += "        PO.CDUOPCAD, ";
                sql += "        PO.AAMODA ";
                sql += "    FROM ";
                sql += "        PROGOCORR PO ";
                sql += "    ) PO ON ";
                sql += "    PO.CDELEMENT = LISUB.CDELEMENT ";
                sql += "WHERE ";
                sql += "    PO.AAMODA = @ANO ";
                sql += "    AND PO.CDUOPCAD = @UOP;";

                var modalidadesCategorias = (await connection.QueryAsync<dynamic>(sql, new
                {
                    UOP = uop,
                    Data = data.Date,
                    //A DATA DEVE SER NO FORMATO YYYY-MM-DD
                    ANO = data.Year,
                    //MES = mes
                })).AsList();

               return modalidadesCategorias.Select(mc => new ModalidadeCategoria
               {
                   CdElement = mc.CDELEMENT,
                   CategoriaCliente = mc.CATEGORIACLIENTE,
                   EComerciario = mc.ECOMERCIARIO == 1,
                   QtdPorCategoria = mc.QTDPORCATEGORIA,
                   DsUsuario = mc.DSUSUARIO,
                   NuVagas = mc.NUVAGAS,
                   NuVagasOcp = mc.NUVAGASOCP,
                   NuMinOcup = mc.NUMINVOCUP,
                   CdUopCad = mc.CDUOPCAD,
                   AaModa = mc.AAMODA
               }).ToList();

            }

        }

        public async Task<List<PROGOCORR>> ObterTurmasComInscritosPorUnidade(int uop, int? ano = null)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = " SELECT P.*, " +
                " NUCPF, TRIM(CLI.NMCLIENTE) NMCLIENTE, CLI.DTVENCTO, CLI.DTNASCIMEN, CLI.CDUOP, CLI.SQMATRIC, INS.CDPROGRAMA, INS.CDCONFIG, INS.SQOCORRENC, INS.CDFORMATO, " +
                " INS.CDFONTEINF, INS.CDPERFIL, INS.STINSCRI, INS.DTINSCRI, INS.LGINSCRI, INS.NUCOBRANCA, INS.CDUOPINSC, INS.DTSTATUS, INS.HRSTATUS, " +
                " INS.LGSTATUS, INS.CDUOPSTAT, TRIM(CAT.DSCATEGORI) DSCATEGORI, VALUE(COBATRINS.NUCOBRANCA, 0) NUCOBEXC, " +
                " VALUE(COBGERINS.NUCOBRANCA, 0) NUCOBREST " +
                " FROM PROGOCORR AS P " +
                " INNER JOIN PROGRAMAS AS PR ON P.CDPROGRAMA = PR.CDPROGRAMA " +
                " INNER JOIN INSCRICAO INS ON INS.CDPROGRAMA = P.CDPROGRAMA AND INS.CDCONFIG = P.CDCONFIG AND INS.SQOCORRENC = P.SQOCORRENC " +
                " INNER JOIN CLIENTELA CLI ON CLI.CDUOP = INS.CDUOP AND CLI.SQMATRIC = INS.SQMATRIC " +
                " INNER JOIN CATEGORIA CAT ON CAT.CDCATEGORI = CLI.CDCATEGORI " +
                " LEFT OUTER JOIN COBATRINS ON COBATRINS.CDELEMENT = CONCAT(CONCAT(LPAD(P.CDPROGRAMA, 8, 0),LPAD(P.CDCONFIG, 8, 0)),LPAD(P.SQOCORRENC, 8, 0)) " +
                " AND COBATRINS.SQMATRIC = INS.SQMATRIC " +
                " LEFT OUTER JOIN COBGERINS ON COBGERINS.CDELEMENT = CONCAT(CONCAT(LPAD(P.CDPROGRAMA, 8, 0),LPAD(P.CDCONFIG, 8, 0)),LPAD(P.SQOCORRENC, 8, 0))  " +
                " AND COBGERINS.SQMATRIC = INS.SQMATRIC ";
                sql += " WHERE DTFIMOCORR > current date AND PR.CDUOP = @UOP ORDER BY DSUSUARIO, CLI.NMCLIENTE";
                //if (ano != null)
                //{
                //    sql += " (YEAR(DTFIMOCORR) - 1) >=  (YEAR(CURRENT date) -1) AND PR.CDUOP = @UOP ORDER BY DSUSUARIO, CLI.NMCLIENTE";
                //}
                //else
                //{
                //    sql += " WHERE DTFIMOCORR > current date AND PR.CDUOP = @UOP ORDER BY DSUSUARIO, CLI.NMCLIENTE";
                //}

                var turmasDictionary = new Dictionary<string, PROGOCORR>();
                var turmas = (await connection.QueryAsync<PROGOCORR, UsuariosAtividade, PROGOCORR>(
                    sql,
                    (turma, usuario) =>
                    {
                        PROGOCORR turmaEntry;
                        string id = Convert.ToString(turma.CDPROGRAMA) + Convert.ToString(turma.CDCONFIG) + Convert.ToString(turma.SQOCORRENC);
                        if (!turmasDictionary.TryGetValue(id, out turmaEntry))
                        {
                            turmaEntry = turma;
                            turma.Inscritos = new List<UsuariosAtividade>();
                            turmasDictionary.Add(id, turmaEntry);
                        }
                        turmaEntry.Inscritos.Add(usuario);
                        return turmaEntry;
                    },
                    new
                    {
                        uop
                    }, splitOn: "NUCPF")).Distinct().AsList();
                var total = turmas.Count;
                return turmas;
            }
        }

        private async Task<List<HRPROGOCOR>> ObterHorarios(PROGOCORR turma)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT * from HRPROGOCOR " +
                    "WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC ORDER BY DDSEMANA, HRINICIO";
                var turmas = (await connection.QueryAsync<HRPROGOCOR>(
                    sql,
                    turma)).AsList();
                return turmas;
            }
        }

        /// <summary>
        /// Obtem lista com todas as atividades atuais do regional
        /// </summary>
        /// <param name="uop"></param>
        /// <returns></returns>
        public async Task<List<PROGRAMAS>> ObterAtividadesPorUnidadeEModalidade(int uop)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT PR.* FROM PROGRAMAS PR " +
                    "INNER JOIN PROGSUBMOD PS ON PR.CDPROGRAMA = PS.CDPROGRAMA " +
                    "INNER JOIN MAPA MP ON MP.CDMAPA = PS.CDMAPA " +
                    "WHERE PR.STATUS = 1 AND PR.CDUOP = @uop AND " +
                    "PS.ANOPROG = (SELECT MAX(ANOPROG) FROM PROGSUBMOD PS1) " +
                    "ORDER BY PR.CDPROGSUP, PR.NMPROGRAMA, PR.CDPROGRAMA ";
                var atividadesBD = (await connection.QueryAsync<PROGRAMAS>(
                    sql,
                    new
                    {
                        uop
                    })).AsList();
                var atividades = atividadesBD.FindAll(a => a.CDPROGSUP == null);
                foreach (var atividade in atividades)
                {
                    atividade.SUBPROGRAMAS = atividadesBD.FindAll(a => a.CDPROGSUP == atividade.CDPROGRAMA);
                    foreach (var subAtividade in atividade.SUBPROGRAMAS)
                    {
                        subAtividade.SUBPROGRAMAS = atividadesBD.FindAll(a => a.CDPROGSUP == subAtividade.CDPROGRAMA);
                        foreach (var subSubAtividade in atividade.SUBPROGRAMAS)
                        {
                            subSubAtividade.SUBPROGRAMAS = atividadesBD.FindAll(a => a.CDPROGSUP == subSubAtividade.CDPROGRAMA);
                        }
                    }
                }

                return atividades;
            }
        }

        /// <summary>
        /// Obtem lista de modalidades e submodalidades
        /// </summary>
        /// <returns></returns>
        public async Task<List<MODALIDADE>> ObterModalidades()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT MOD.CDADMIN, MOD.CDMAPA, MOD.CDMODALIDA,  MOD.CDREALIZAC, TRIM(MOD.DSMODALIDA) DSMODALIDA, SUB.CDSUBMODAL, " +
                    " TRIM(SUB.DSSUBMODAL) DSSUBMODAL, YEAR(M.DTINICIO) ANOPROG  FROM MAPA M, MODALIDADE MOD, SUBMODALID SUB " +
                    " WHERE MOD.CDADMIN = (SELECT VLINTEIRO FROM PARAMETROS WHERE IDCLASSE = 'CACLS' AND CDPARAM = 2) " +
                    " AND M.CDSDEPROG <> 1   AND MOD.CDMAPA = M.CDMAPA  AND SUB.CDADMIN = MOD.CDADMIN AND SUB.CDMAPA = MOD.CDMAPA AND SUB.CDREALIZAC = MOD.CDREALIZAC " +
                    " AND SUB.CDMODALIDA = MOD.CDMODALIDA AND(YEAR(M.DTINICIO) <= 2017 AND M.DTVALIDADE IS NULL)" +
                    " ORDER BY MOD.CDADMIN, MOD.CDMAPA, MOD.CDREALIZAC, MOD.CDMODALIDA, SUB.CDSUBMODAL ";
                var modalidadesDB = (await connection.QueryAsync<MODALIDADE>(
                    sql)).AsList();
                var modalidades = modalidadesDB.FindAll(m => m.CDREALIZAC == 0 && m.CDMODALIDA == 0);
                foreach (var modalidade in modalidades)
                {
                    var submodalidades = modalidadesDB.FindAll(m => m.CDMAPA == modalidade.CDMAPA && m.CDREALIZAC != 0 && m.CDMODALIDA == 0 && m.CDSUBMODAL == 0)
                        .OrderBy(m => m.CDREALIZAC).ToList();
                    foreach (var subModalidade in submodalidades)
                    {
                        subModalidade.SUBMODALIDADES = modalidadesDB.FindAll(m => m.CDMAPA == modalidade.CDMAPA && m.CDREALIZAC != 0 && m.CDMODALIDA != 0 && m.CDSUBMODAL == 0)
                            .OrderBy(m => m.CDREALIZAC).ToList();
                        foreach (var subSubModalidade in subModalidade.SUBMODALIDADES)
                        {
                            subSubModalidade.SUBMODALIDADES = modalidadesDB.FindAll(m => m.CDMAPA == modalidade.CDMAPA && m.CDREALIZAC != 0 &&
                                    m.CDSUBMODAL == subSubModalidade.CDREALIZAC && m.CDMODALIDA != 0 && m.CDSUBMODAL != 0)
                                .OrderBy(m => m.CDREALIZAC).ToList();
                        }
                    }
                    modalidade.SUBMODALIDADES = submodalidades;
                }

                return modalidades;
            }
        }

        /// <summary>
        /// Obtem numero de vagas ocupadas
        /// </summary>
        /// <param name="CDPROGRAMA"></param>
        /// <param name="CDCONFIG"></param>
        /// <param name="SQOCORRENC"></param>
        /// <returns></returns>
        public async Task<int> ObterNumeroVagas(int CDPROGRAMA, int CDCONFIG, int SQOCORRENC)
        {
            var sql = "select NUVAGASOCP from PROGOCORR WHERE CDPROGRAMA = 3280730 AND CDCONFIG = 3710004 AND SQOCORRENC = 3710004";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var numeroVagas = await connection.QueryFirstOrDefaultAsync<int>(
                    sql,
                    new
                    {
                        CDPROGRAMA,
                        CDCONFIG,
                        SQOCORRENC
                    }
                );
                return numeroVagas;
            }
        }

        /// <summary>
        /// Obtem os dados da turma
        /// </summary>
        /// <param name="CDPROGRAMA"></param>
        /// <param name="CDCONFIG"></param>
        /// <param name="SQOCORRENC"></param>
        /// <returns></returns>
        public async Task<PROGOCORR> ObterDadosTurma(int CDPROGRAMA, int CDCONFIG, int SQOCORRENC)
        {
            var sql = "SELECT * FROM PROGOCORR WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var turma = await connection.QueryFirstAsync<PROGOCORR>(
                    sql,
                    new
                    {
                        CDPROGRAMA,
                        CDCONFIG,
                        SQOCORRENC
                    }
                );
                return turma;
            }

        }

        public async Task<INSCRICAO> VerificaInscricoesAtivasAluno(CLIENTELA cliente)
        {
            var sql = "SELECT I.*,PO.DSUSUARIO, PO.VBCANCELA , U.NMUSUARIO FROM INSCRICAO AS I, PROGOCORR AS PO, VWUSUARIOS AS U " +
                " WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC AND STINSCRI = 0 AND ((DTFIMOCORR >= (current date + 7 days)) OR (DTFIMOCORR IS NULL)) " +
                " AND PO.CDPROGRAMA = I.CDPROGRAMA AND PO.CDCONFIG = I.CDCONFIG  AND PO.SQOCORRENC = I.SQOCORRENC  AND I.LGSTATUS = U.IDUSUARIO";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var numeroVagas = await connection.QueryFirstOrDefaultAsync<INSCRICAO>(
                    sql,
                    cliente
                );
                return numeroVagas;
            }
        }

        /// <summary>
        /// Obtem o valor das parcelas de uma inscricao em atividade
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        public async Task<VALORPARC> ObtemValorDasParcelas(INSCRICAO inscricao)
        {

            var sql = "SELECT CDPROGRAMA, CDCONFIG, SQOCORRENC, CDFORMATO, CDPARCELA, CDPERFIL, VLPARCELA, DTVENCTO, TPPARCELA, DTATU, HRATU, LGATU FROM VALORPARC " +
                "WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC AND CDFORMATO = @CDFORMATO AND CDPERFIL = @CDPERFIL AND TPPARCELA = 0 " +
                "UNION SELECT CDPROGRAMA, CDCONFIG, SQOCORRENC, CDFORMATO, CDPARCELA, CDPERFIL, VLPARCELA, DTVENCTO, TPPARCELA, DTATU, HRATU, LGATU FROM VALORPARC " +
                "WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC AND CDFORMATO = @CDFORMATO AND CDPERFIL = @CDPERFIL AND TPPARCELA = 1 ";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var valorParc = await connection.QueryFirstAsync<VALORPARC>(
                    sql,
                    inscricao
                );
                return valorParc;
            }
        }

        /// <summary>
        /// Obtem o valor das parcelas de uma inscricao em atividade
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        public async Task<List<VALORPARC>> ObtemValoresTurma(PROGOCORR turma)
        {
            var sql = "SELECT V.CDPROGRAMA, V.CDCONFIG, V.SQOCORRENC, V.CDPERFIL, P.CDCATEGORI, V.VLPARCELA, V.DTVENCTO, V.CDFORMATO, V.CDPARCELA, V.CDPERFIL, V.TPPARCELA FROM VALORPARC V " +
                "INNER JOIN PERFILCAT P ON P.CDPERFIL = V.CDPERFIL WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC ";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var valorParc = await connection.QueryAsync<VALORPARC>(
                    sql,
                    turma
                );
                return valorParc.ToList();
            }
        }

        /// <summary>
        /// Obtem o dados de uma atividade
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        public async Task<PROGOCORR> ObtemDadosTurma(PROGOCORR turma)
        {
            var sql = "SELECT PR.*, PS.CDMAPA FROM PROGOCORR PR INNER JOIN PROGSUBMOD PS ON PR.CDPROGRAMA = PS.CDPROGRAMA"
            + " WHERE PR.CDPROGRAMA = @CDPROGRAMA AND PR.CDCONFIG = @CDCONFIG AND PR.SQOCORRENC = @SQOCORRENC";

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var turmaDb = await connection.QueryFirstAsync<PROGOCORR>(
                    sql,
                    turma
                );
                turmaDb.FORMASPGTO = await this.ObterFormatoPgto(turma.CDPROGRAMA, turma.CDCONFIG);
                turmaDb.Horarios = await this.ObterHorarios(turma);
                return turmaDb;
            }
        }

        /// <summary>
        /// Obtem os valores de desconto e acrescimo de uma cobrança a partir da inscrição
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        public async Task<DESCACRES> ObtemDescAscres(INSCRICAO inscricao)
        {
            var sql = "SELECT CDPROGRAMA, CDCONFIG, SQOCORRENC, CDFORMATO, SQLANCAMEN, CDPERFIL, DDLIMITE, TPLANCAMEN, TPVALOR, VLDESCACRE, RFDIASLMT, DTATU, HRATU, LGATU," +
                "case RFDIASLMT when - 1 then 'Antes' when 1 then 'Depois' else '' end AS REFERENCIA," +
                " case TPLANCAMEN when 0 then 'Desconto' when 1 then 'Acréscimo' else '' end AS LANCAMENTO, case TPVALOR when 0 then 'Espécie' when 1 then 'Percentual' " +
                " else '' " +
                " end AS TIPOVALOR FROM DESCACRES WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG " +
                " AND SQOCORRENC = @SQOCORRENC AND CDPERFIL = @CDPERFIL AND CDFORMATO = @CDFORMATO";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var descAcres = await connection.QueryFirstOrDefaultAsync<DESCACRES>(
                    sql,
                    inscricao
                );
                return descAcres;
            }
        }

        /// <summary>
        /// Inscreve um usuário em uma atividade
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns
        public async Task<string> InscreveUsuario(INSCRICAO inscricao)
        {
            inscricao.CDUOPINSC = (short)caixaConfiguration.CdUop;
            inscricao.CDUOPSTAT = (short)caixaConfiguration.CdUop;
            inscricao.LGSTATUS = caixaConfiguration.CdPessoa.ToString();
            inscricao.LGINSCRI = caixaConfiguration.CdPessoa.ToString();
            inscricao.CDFONTEINF = 6; // 6 - Fonte da informação - Internet
            var turma = await this.ObterDadosTurma(inscricao.CDPROGRAMA, inscricao.CDCONFIG, inscricao.SQOCORRENC);
            inscricao.DTFIMOCORR = turma.DTFIMOCORR != null ? turma.DTFIMOCORR.Value : DateTime.Now;
            inscricao.PROGOCORR = turma;
            var nuVagasOcp = turma.NUVAGASOCP;

            inscricao.CDPERFIL = await this.ObterPerfilCliente(inscricao);

            //Se não informado formato de pagamento obtem o padrao para atividade
            if (inscricao.CDFORMATO == null)
                inscricao.CDFORMATO = await this.ObterFormatoPgtoPadrao(inscricao);

            if (nuVagasOcp >= turma.NUVAGAS) //Verifica se existem vagas disponiveis na turma
            {
                return "Não existem mais vagas disponiveis na turma";
            }
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        //Obter dados da turma
                        var cobrancas = await this.GerarCobranca(inscricao, turma.CDUOPCAD);
                        inscricao.NUCOBRANCA = (short)cobrancas.Count;

                        //Verifica se cliente já foi inscrito na atividade
                        var sql = "SELECT STINSCRI, CDUOPINSC FROM INSCRICAO WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC AND CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";
                        var inscOriginal = connection.QueryFirstOrDefault<INSCRICAO>(
                            sql,
                            inscricao,
                            transaction
                        );

                        sql = "INSERT INTO INSCRICAO (CDUOP, SQMATRIC, CDPROGRAMA, CDCONFIG, SQOCORRENC, CDDESCONTO, CDFONTEINF, CDFORMATO, CDPERFIL, STINSCRI, DTPREINSCR, DTINSCRI, " +
                            "LGINSCRI, DTPRIVENCT, NUCOBRANCA, CDUOPINSC, DTSTATUS, HRSTATUS, LGSTATUS, DSSTATUS, CDUOPSTAT) " +
                            " VALUES(@CDUOP, @SQMATRIC, @CDPROGRAMA, @CDCONFIG, @SQOCORRENC, @CDDESCONTO, @CDFONTEINF, @CDFORMATO, @CDPERFIL, @STINSCRI, @DTPREINSCR, CURRENT DATE," +
                            "@LGINSCRI, @DTPRIVENCT, @NUCOBRANCA, @CDUOPINSC, CURRENT DATE, CURRENT TIME , @LGSTATUS, @DSSTATUS, @CDUOPSTAT)";

                        if (inscOriginal is INSCRICAO) //Se inscrição para atividade ja existe ou ja existiu esse ano
                        {
                            if (inscOriginal.STINSCRI == 0) return "Cliente já inscrito nessa atividade";
                            sql = "SELECT SQHISTORIC FROM HSTINSCRIC WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC AND CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC" +
                                " ORDER BY SQHISTORIC DESC";
                            var sqhistoric = await connection.QueryFirstOrDefaultAsync<int>(
                                sql,
                                inscricao,
                                transaction
                            );

                            var hstinscricao = new HSTINSCRIC
                            {
                                CDUOP = inscricao.CDUOP,
                                SQMATRIC = inscricao.SQMATRIC,
                                CDPROGRAMA = inscricao.CDPROGRAMA,
                                CDCONFIG = inscricao.CDCONFIG,
                                SQOCORRENC = inscricao.SQOCORRENC,
                                SQHISTORIC = sqhistoric > 0 ? sqhistoric + 1 : 1,
                                STREGISTRO = inscricao.STINSCRI,
                                LGRESPONSA = inscricao.LGINSCRI,
                                CDUOPREG = inscOriginal.CDUOPINSC
                            };

                            sql = "INSERT INTO HSTINSCRIC(CDUOP, SQMATRIC, CDPROGRAMA, CDCONFIG, SQOCORRENC, SQHISTORIC, STREGISTRO, DSACONTEC, DTREGISTRO, HRREGISTRO, LGRESPONSA, CDUOPREG)" +
                                " VALUES (@CDUOP, @SQMATRIC, @CDPROGRAMA, @CDCONFIG, @SQOCORRENC, @SQHISTORIC, @STREGISTRO, '', current date, current time, @LGRESPONSA, @CDUOPREG) ";
                            await connection.ExecuteAsync(
                                sql,
                                hstinscricao,
                                transaction
                            );

                            sql = "UPDATE INSCRICAO SET STINSCRI = @STINSCRI, DTSTATUS = current date, HRSTATUS = current time, LGSTATUS = @LGSTATUS, " +
                                " CDUOPSTAT = @CDUOPSTAT, CDUOPINSC = @CDUOPINSC, STCANCELAD = NULL, DSSTATUS = @DSSTATUS " +
                                " WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC AND CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";

                        }

                        await connection.ExecuteAsync(
                            sql,
                            inscricao,
                            transaction
                        );

                        foreach (var cobranca in cobrancas)
                        {
                            //cobranca.CDUOPREC = turma.CDUOPCAD;
                            sql = "INSERT INTO COBRANCA(IDCLASSE, CDELEMENT, SQCOBRANCA, CDUOPCOB, CDUOP, SQMATRIC, RFCOBRANCA, DSCOBRANCA, VLCOBRADO, DTEMISSAO, DTVENCTO, STRECEBIDO," +
                                " TPCOBRANCA, PCJUROS, SMFIELDATU, DTATU, HRATU, LGATU, VLCARACTE1, VLCARACTE2, DDCOBJUROS, DDINIJUROS, PCMULTA, DSCANCELAM, TPMORA, CDUOPREC )" +
                                " VALUES( @IDCLASSE, @CDELEMENT, @SQCOBRANCA, @CDUOPCOB, @CDUOP, @SQMATRIC, @RFCOBRANCA, @DSCOBRANCA, @VLCOBRADO, CURRENT DATE, @DTVENCTO, @STRECEBIDO, " +
                                " @TPCOBRANCA, @PCJUROS, @SMFIELDATU, CURRENT DATE, CURRENT TIME, @LGATU, @VLCARACTE1, @VLCARACTE2, @DDCOBJUROS, @DDINIJUROS, @PCMULTA, @DSCANCELAM, @TPMORA, @CDUOPREC )";

                            await connection.ExecuteAsync(
                                sql,
                                cobranca,
                                transaction
                            );
                        }

                        sql = "UPDATE PROGOCORR SET NUVAGASOCP = @NUVAGASOCP WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";
                        await connection.ExecuteAsync(
                            sql, new
                            {
                                inscricao.CDPROGRAMA,
                                inscricao.CDCONFIG,
                                inscricao.SQOCORRENC,
                                nuVagasOcp = nuVagasOcp + 1
                            },
                            transaction
                        );

                        await transaction.CommitAsync();
                        return "";
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        Console.Write(e);
                        return $"Erro ao inscrever o cliente: {e.Message}";
                    }
                }
            }
        }

        public async Task<AtividadeOnLine> ObterAtividadeSite(int CDPROGRAMA, int CDCONFIG, int SQOCORRENC)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                var sql = "SELECT DescontoInscricao, DataInicioPromocao, DataFimPromocao, Ano FROM AtividadeOnLine WHERE CDPROGRAMA = @CDPROGRAMA " +
                    " AND SQOCORRENC = @SQOCORRENC AND CDCONFIG = @CDCONFIG";
                var atividade = await connection.QueryFirstOrDefaultAsync<AtividadeOnLine>(sql, new
                {
                    CDPROGRAMA,
                    CDCONFIG,
                    SQOCORRENC
                });
                return atividade;
            }
        }
        public async Task<int> ObterAtividadeSiteSubArea(int CDPROGRAMA, int CDCONFIG, int SQOCORRENC)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                var sql = "select sa.Id as idSubArea from AtividadeOnLine ao JOIN SubArea sa on sa.Id  = ao.IdSubArea JOIN AreaCoordenacao ac on ac.Id = sa.IdAreaCoordenacao " +
                    "where ao.CdPrograma = @CDPROGRAMA and ao.CdConfig = @CDCONFIG and ao.SqOcorrenc = @SQOCORRENC ";
                var subarea = await connection.QueryFirstAsync<int>(sql, new
                {
                    CDPROGRAMA,
                    CDCONFIG,
                    SQOCORRENC
                });
                return subarea;
            }
        }

        /// <summary>
        /// Gera as cobrancas para a atividade do cliente selecionado
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        public async Task<List<COBRANCA>> GerarCobranca(INSCRICAO inscricao, int uopcad)
         {
            
            // SELECT VALUE(MAX(SQCOBRANCA), 0) + 1 FROM COBRANCA WHERE IDCLASSE = 'OCRID'
            // AND CDELEMENT = '032807300371000403710004'
            var cdElement = inscricao.CDPROGRAMA.ToString().PadLeft(8, '0') + inscricao.CDCONFIG.ToString().PadLeft(8, '0') + inscricao.SQOCORRENC.ToString().PadLeft(8, '0');
            //var cdElement = "032807300371000403710004";
            var sql = "SELECT VALUE(MAX(SQCOBRANCA), 0) + 1 FROM COBRANCA WHERE IDCLASSE = 'OCRID' AND CDELEMENT = @CDELEMENT";
            var codigoProxCobranca = 1;
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                codigoProxCobranca = await connection.QueryFirstOrDefaultAsync<int>(
                    sql,
                    new
                    {
                        cdElement
                    }
                );
                //return descAcres;
            }
            var parametrosJuros = await this.ObterParametrosJuros(inscricao);
            var valorParc = await this.ObtemValorDasParcelas(inscricao);
            var formaPgto = await this.ObterFormaPgto(inscricao);
            var cobrancas = new List<COBRANCA>();
            var diaVencimento = formaPgto.DDVENCTO;
            var diaAtual = DateTime.Now.Day;
            var mesAtual = DateTime.Now.Month;
            var anoAtual = DateTime.Now.Year;
            var vencimento = new DateTime(anoAtual, mesAtual, diaAtual);
            var valor = valorParc.VLPARCELA;
            var valorDia = valor / 30;
            var mes = 0;

            //var atividadeOnLine = await ObterAtividadeSite(inscricao.CDPROGRAMA, inscricao.CDCONFIG, inscricao.SQOCORRENC);

            //var atividadeOnLineSubArea = await ObterAtividadeSiteSubArea(inscricao.CDPROGRAMA, inscricao.CDCONFIG, inscricao.SQOCORRENC);

            //if (valorParc.TPPARCELA == 1) // 1 - Parcelas mensais TODO: 1 nao quer dizer mensal ... talvez formaPgto = 2 - avista formapgto = 0 - mensal
            if (valor > 0)
            {
                if (formaPgto.TPPGTO == 2)
                { //TODO: Validar isenção
                    vencimento = DateTime.Today;
                    var cobranca = new COBRANCA
                    {
                        IDCLASSE = "OCRID", //Cobranças de atividades
                        CDELEMENT = cdElement,
                        CDUOP = inscricao.CDUOP,
                        SQMATRIC = inscricao.SQMATRIC,
                        CDUOPCOB = uopcad,
                        CDUOPREC = uopcad,
                        SQCOBRANCA = codigoProxCobranca,
                        DSCOBRANCA = inscricao.PROGOCORR.DSUSUARIO,
                        RFCOBRANCA = 3, // 3 - Atividades / Turmas
                        VLCOBRADO = valor,
                        DTVENCTO = vencimento,
                        DTEMISSAO = DateTime.Now.Date,
                        STRECEBIDO = 0, // 0 - Em aberto
                        TPCOBRANCA = 1, // 0 - Inscrição, 1 - Mensalidade
                        PCJUROS = parametrosJuros.PERCMORA,
                        DTATU = DateTime.Now.Date,
                        SMFIELDATU = 0.0,
                        HRATU = DateTime.Now.TimeOfDay,
                        LGATU = inscricao.LGINSCRI,
                        DDCOBJUROS = parametrosJuros.DCALMORA,
                        DDINIJUROS = (short)parametrosJuros.DINIMORA,
                        PCMULTA = parametrosJuros.PERCMULTA,
                        TPMORA = parametrosJuros.TPMORA,
                    };
                    cobrancas.Add(cobranca);
                    return cobrancas;
                }
                else if (formaPgto.TPPGTO == 0) // Pagamentos Mensais
                {
                    var numCobranca = 0;
                    var inicioAtividade = inscricao.PROGOCORR.DTINIOCORR;
                    
                    // Verificação para garantir que o início da atividade é válido
                    if (inicioAtividade == null || inicioAtividade == DateTime.MinValue)
                    {
                        throw new ArgumentException("A data de início da atividade é inválida.");
                    }

                 
                    while (vencimento < inscricao.DTFIMOCORR || vencimento <= inscricao.DTFIMOCORR )
                    //for (int i = 0; i < inscricao.NUCOBRANCA; i++)
                    {
                        valor = valorParc.VLPARCELA;
                        if (formaPgto.VBPROPORCI == 1)
                        {
                            if (numCobranca == 0)
                            {
                                var diasProporcional = 0;
                                if (inicioAtividade > DateTime.Now)
                                {
                                    diasProporcional = diaVencimento - inicioAtividade.Value.Day;
                                    if (diasProporcional < 0) //SE O VALOR FOR MENOR QUE ZERO, A ATIVIDADE INICIA APÓS O DIA DEFINIDO COMO PADRÃO PARA O VENCIMENTO
                                    {
                                        diasProporcional = -1 * (inicioAtividade.Value.Day - 30) + diaVencimento;
                                    }
                                }
                                else
                                {
                                    mes = (diaAtual > diaVencimento) ? (DateTime.Now.AddMonths(1)).Month : mesAtual;
                                    
                                    if (anoAtual < 1 || anoAtual > 9999 || mes < 1 || mes > 12 || diaVencimento < 1 || diaVencimento > DateTime.DaysInMonth(anoAtual, mes))
                                    {
                                        throw new ArgumentOutOfRangeException("Data de vencimento inválida ao calcular o próximo vencimento.");
                                    }
                                    
                                    var proximoVencimento = new DateTime(anoAtual, mes, diaVencimento);
                                    var primeiroVenc = new DateTime(anoAtual, mesAtual, diaAtual);
                                    diasProporcional = (proximoVencimento - primeiroVenc).Days;
                                }
                                if (diasProporcional > 0)
                                {
                                    valor = Decimal.Truncate(valorDia * diasProporcional * 100) / 100;
                                }
                                else
                                {
                                    mes = 0;
                                }
                            }
                            if (vencimento.AddMonths(1) > inscricao.DTFIMOCORR)
                            { //se for o ultimo mes, cobra proporcional
                                var diasProporcional = (inscricao.DTFIMOCORR - vencimento).Days;
                                valor = Decimal.Truncate(valorDia * diasProporcional * 100) / 100;
                            }
                        }
                        if (inscricao.PROGOCORR.DTINIFER == null || vencimento < inscricao.PROGOCORR.DTINIFER || vencimento > inscricao.PROGOCORR.DTFIMFER)
                        {

                            var cobranca = new COBRANCA
                            {
                                IDCLASSE = "OCRID", //Cobranças de atividades
                                CDELEMENT = cdElement,
                                CDUOP = inscricao.CDUOP,
                                SQMATRIC = inscricao.SQMATRIC,
                                CDUOPCOB = uopcad,
                                CDUOPREC = uopcad,
                                SQCOBRANCA = codigoProxCobranca,
                                DSCOBRANCA = inscricao.PROGOCORR.DSUSUARIO,
                                RFCOBRANCA = 3, // 3 - Atividades / Turmas
                                VLCOBRADO = valor,
                                DTVENCTO = vencimento,
                                DTEMISSAO = DateTime.Now.Date,
                                STRECEBIDO = 0, // 0 - Em aberto
                                TPCOBRANCA = 1, // 0 - Inscrição, 1 - Mensalidade
                                PCJUROS = parametrosJuros.PERCMORA,
                                DTATU = DateTime.Now.Date,
                                SMFIELDATU = 0.0,
                                HRATU = DateTime.Now.TimeOfDay,
                                LGATU = inscricao.LGINSCRI,
                                DDCOBJUROS = parametrosJuros.DCALMORA,
                                DDINIJUROS = (short)parametrosJuros.DINIMORA,
                                PCMULTA = parametrosJuros.PERCMULTA,
                                TPMORA = parametrosJuros.TPMORA
                            };

                            if (inscricao.PROGOCORR.DTINIOCORR > DateTime.Now.Date && vencimento < inscricao.PROGOCORR.DTINIOCORR)
                            {
                                cobranca.DTVENCTO = vencimento;
                                //vencimento = (DateTime)inscricao.PROGOCORR.DTINIOCORR;
                            }
                            if (cobranca.VLCOBRADO > 0)
                                cobrancas.Add(cobranca);

                            codigoProxCobranca++;
                        }
                        
                        // Verificação antes de atualizar o vencimento
                        if (vencimento.Year < 1 || vencimento.Year > 9999 || vencimento.Month < 1 || vencimento.Month > 12 || diaVencimento < 1 || diaVencimento > DateTime.DaysInMonth(vencimento.Year, vencimento.Month))
                        {
                            throw new ArgumentOutOfRangeException("Parâmetros de vencimento inválidos.");
                        }
                        
                        //var inicioAtividade = inscricao.PROGOCORR.DTINIOCORR;
                        if (inicioAtividade > vencimento)
                        {
                            vencimento = new DateTime(inicioAtividade.Value.Year, inicioAtividade.Value.Month, diaVencimento);
                            if (vencimento.Date < inicioAtividade.Value.Date)
                            {
                                vencimento = vencimento.AddMonths(1);
                            }
                        }
                        else
                        {

                            if (mes != mesAtual)
                            {
                                vencimento = vencimento.AddMonths(1);
                            }
                            else
                                mes = 0;
                           
                            vencimento = new DateTime(vencimento.Year, vencimento.Month, diaVencimento);
                           
                        }
                        numCobranca++;
                    }
                    return cobrancas;
                }
            }
            return cobrancas;
        }

        /// <summary>
        /// Obtem os juros para as atividades
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        private async Task<PARAMATIV> ObterParametrosJuros(INSCRICAO inscricao)
        {
            //SELECT CDMAPA, CDREALIZAC, VBCOBMULTA , PERCMULTA ,DINIMULTA , VBCOBMORA , PERCMORA , DINIMORA , DCALCMORA, TPMORA 
            //FROM PARAMATIV WHERE CDMAPA = ? AND CDREALIZAC = ? SELECT CDMAPA, CDREALIZAC  FROM PROGSUBMOD  WHERE CDPROGRAMA = 3280730 AND ANOPROG = (SELECT MAX(ANOPROG) FROM PROGSUBMOD WHERE CDPROGRAMA = 3280730)
            var sql = "SELECT CDMAPA, CDREALIZAC, VBCOBMULTA , PERCMULTA ,DINIMULTA , VBCOBMORA , PERCMORA , DINIMORA , DCALMORA, TPMORA " +
                "FROM PARAMATIV WHERE CDMAPA = (SELECT CDMAPA FROM PROGSUBMOD WHERE CDPROGRAMA = @CDPROGRAMA AND ANOPROG = (SELECT MAX(ANOPROG) FROM PROGSUBMOD WHERE CDPROGRAMA = @CDPROGRAMA)) " +
                "AND CDREALIZAC = (SELECT CDREALIZAC FROM PROGSUBMOD  WHERE CDPROGRAMA = @CDPROGRAMA AND ANOPROG = (SELECT MAX(ANOPROG) FROM PROGSUBMOD WHERE CDPROGRAMA = @CDPROGRAMA))";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var parametrosJuros = await connection.QueryFirstOrDefaultAsync<PARAMATIV>(
                    sql,
                    new
                    {
                        inscricao.CDPROGRAMA
                    }
                );
                if (parametrosJuros.VBCOBMORA == 0)
                {
                    parametrosJuros.PERCMORA = 0;
                    parametrosJuros.DINIMORA = 0;
                }
                if (parametrosJuros.VBCOBMULTA == 0)
                {
                    parametrosJuros.PERCMULTA = 0;
                    parametrosJuros.DINIMULTA = 0;
                }
                if (parametrosJuros.DINIMORA == 1)
                {
                    parametrosJuros.DINIMORA = 101;
                }
                return parametrosJuros;
            }
        }

        /// <summary>
        /// Obtem o perfil do cliente pela categoria e atividade
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        public async Task<int> ObterPerfilCliente(INSCRICAO inscricao)
        {
            var sql = "SELECT CDPERFIL FROM PERFILCAT WHERE CDCATEGORI = @CDCATEGORI AND " +
                "CDPERFIL IN(SELECT CDPERFIL FROM PERFILPROG WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG) ";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var perfil = await connection.QueryFirstAsync<int>(
                    sql,
                    new
                    {
                        inscricao.CDPROGRAMA,
                        inscricao.CDCONFIG,
                        inscricao.CDCATEGORI
                    }
                );
                return perfil;
            }
        }

        /// <summary>
        /// Obtem o codigo formato do pagamento padrão
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        private async Task<int> ObterFormatoPgtoPadrao(INSCRICAO inscricao)
        {
            // Listar formas de pagamento
            //   SELECT CDFORMATO, CDPROGRAMA, DDVENCTO, DTATU, HRATU, Trim(LGATU) LGATU, Trim(NMFORMATO) NMFORMATO, 
            //  NUPARCELAS, TPPGTO, VBCOBGERIN, VBINSCCOBR,   VBPROPORCI, VLPERIODIC
            //  FROM FORMASPGTO WHERE CDFORMATO IN(SELECT CDFORMATO FROM PERFILPGTO
            //  WHERE CDPERFIL = 3280742 AND CDPROGRAMA = 3280667 AND CDCONFIG = 3710003)
            //   FOR READ ONLY WITH UR

            // Pagamento padrão da atividade
            var sql = "SELECT CDFORMATO FROM CONFPROG WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG";
            // var sql = "SELECT CDFORMATO, CDPROGRAMA, DDVENCTO, DTATU, HRATU, Trim(LGATU) LGATU, Trim(NMFORMATO) NMFORMATO," +
            // "NUPARCELAS, TPPGTO, VBCOBGERIN, VBINSCCOBR,   VBPROPORCI, VLPERIODIC " +
            // "FROM FORMASPGTO WHERE CDFORMATO IN (SELECT CDFORMATO FROM PERFILPGTO " +
            // "WHERE CDPERFIL = @CDPERFIL AND CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG) ";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var perfil = await connection.QueryFirstAsync<int>(
                    sql,
                    new
                    {
                        inscricao.CDPERFIL,
                        inscricao.CDPROGRAMA,
                        inscricao.CDCONFIG
                    }
                );
                return perfil;
            }
        }

        /// <summary>
        /// Obtem formatos de pagamento
        /// </summary>
        /// <param name="CDPROGRAMA"></param>
        /// <param name="CDCONFIG"></param>
        /// <returns></returns>
        public async Task<List<FORMASPGTO>> ObterFormatoPgto(int CDPROGRAMA, int CDCONFIG)
        {
            // Listar formas de pagamento
            //   SELECT CDFORMATO, CDPROGRAMA, DDVENCTO, DTATU, HRATU, Trim(LGATU) LGATU, Trim(NMFORMATO) NMFORMATO, 
            //  NUPARCELAS, TPPGTO, VBCOBGERIN, VBINSCCOBR,   VBPROPORCI, VLPERIODIC
            //  FROM FORMASPGTO WHERE CDFORMATO IN(SELECT CDFORMATO FROM PERFILPGTO
            //  WHERE CDPERFIL = 3280742 AND CDPROGRAMA = 3280667 AND CDCONFIG = 3710003)
            //   FOR READ ONLY WITH UR

            // Pagamento padrão da atividade
            //var sql = "SELECT CDFORMATO FROM CONFPROG WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG";
            var sql = "SELECT CDFORMATO, CDPROGRAMA, DDVENCTO, Trim(NMFORMATO) NMFORMATO," +
                "NUPARCELAS, TPPGTO, VBCOBGERIN, VBINSCCOBR,   VBPROPORCI, VLPERIODIC " +
                "FROM FORMASPGTO WHERE CDFORMATO IN (SELECT CDFORMATO FROM PERFILPGTO " +
                "WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG) ";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var perfil = await connection.QueryAsync<FORMASPGTO>(
                    sql,
                    new
                    {
                        CDPROGRAMA,
                        CDCONFIG
                    }
                );
                return perfil.ToList();
            }
        }

        /// <summary>
        /// Obtem a forma de pagamento padrão
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        private async Task<FORMASPGTO> ObterFormaPgto(INSCRICAO inscricao)
        {
            // A query abaixo lista todas a formas de pagamento da atividade incluindo Isenção
            // SELECT * FROM FORMASPGTO  WHERE CDFORMATO IN (SELECT CDFORMATO FROM PERFILPGTO
            //WHERE CDPERFIL = (SELECT CDPERFIL FROM PERFILCAT WHERE CDCATEGORI = 25 AND 
            //CDPERFIL IN(SELECT CDPERFIL FROM PERFILPROG WHERE CDPROGRAMA = 3280714 AND CDCONFIG = 3710004))
            //AND CDPROGRAMA = 3280714 AND CDCONFIG = 3710004)

            //Lista apenas o meio de pagamento padrão
            var sql = "SELECT * FROM FORMASPGTO WHERE CDFORMATO = @CDFORMATO ";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var formasPgto = await connection.QueryFirstAsync<FORMASPGTO>(
                    sql,
                    new
                    {
                        inscricao.CDFORMATO
                    }
                );
                return formasPgto;
            }
        }

        public async Task<string> CancelaInscricao(INSCRICAO inscricao, bool nova = false)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var sql = "SELECT SQCANCELA FROM HSTCANINSC WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC AND CDPROGRAMA = @CDPROGRAMA " +
                            "  AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC" +
                            " ORDER BY SQCANCELA DESC";
                        var SQCANCELA = await connection.QueryFirstOrDefaultAsync<int>(
                            sql,
                            inscricao,
                            transaction
                        );

                        //Insere registro de cancelamento
                        var hstcaninsc = new HSTCANINSC
                        {
                            CDUOP = inscricao.CDUOP,
                            SQMATRIC = inscricao.SQMATRIC,
                            CDPROGRAMA = inscricao.CDPROGRAMA,
                            CDCONFIG = inscricao.CDCONFIG,
                            SQOCORRENC = inscricao.SQOCORRENC,
                            CDCANCELA = 83, //COVID-19
                            SQCANCELA = SQCANCELA > 0 ? SQCANCELA + 1 : 1,
                            LGATU = caixaConfiguration.CdPessoa.ToString()
                        };
                        sql = "INSERT INTO HSTCANINSC(CDCANCELA, CDPROGRAMA, CDCONFIG, SQOCORRENC, CDUOP, SQMATRIC, SQCANCELA, DTATU, HRATU, LGATU)" +
                            " VALUES(@CDCANCELA, @CDPROGRAMA, @CDCONFIG, @SQOCORRENC, @CDUOP, @SQMATRIC, @SQCANCELA, CURRENT DATE, CURRENT TIME, @LGATU)";
                        await connection.ExecuteAsync(
                            sql,
                            hstcaninsc,
                            transaction
                        );

                        var cdElement = inscricao.CDPROGRAMA.ToString().PadLeft(8, '0') + inscricao.CDCONFIG.ToString().PadLeft(8, '0') + inscricao.SQOCORRENC.ToString().PadLeft(8, '0');

                        //apaga cobrancas futuras
                        if (nova)
                        {
                            sql = "DELETE FROM COBRANCA " +
                                "WHERE IDCLASSE = @IDCLASSE AND CDELEMENT = @CDELEMENT AND STRECEBIDO = 0 "; //AND DTVENCTO >= CURRENT DATE
                        }
                        else
                        {
                            sql = "DELETE FROM COBRANCA " +
                                "WHERE IDCLASSE = @IDCLASSE AND CDELEMENT = @CDELEMENT AND STRECEBIDO = 0 AND DTVENCTO >= CURRENT DATE"; //AND DTVENCTO >= CURRENT DATE
                        }

                        await connection.ExecuteAsync(
                            sql,
                            new
                            {
                                IDCLASSE = "OCRID",
                                CDELEMENT = cdElement,
                            },
                            transaction
                        );

                        //cancela cobrancas vencidas
                        if (!nova)
                        {
                            sql = "UPDATE COBRANCA SET STRECEBIDO = 2, DTATU = current date, HRATU = current time, DSCANCELAM = @DSCANCELAM, CDCANCELA = 81, " +
                                " LGATU = @LGATU, LGCANCEL = @LGATU " +
                                "WHERE IDCLASSE = @IDCLASSE AND CDELEMENT = @CDELEMENT AND STRECEBIDO = 0 AND DTVENCTO < CURRENT DATE";

                            await connection.ExecuteAsync(
                                sql,
                                new
                                {
                                    DSCANCELAM = "Cancelamento automático Covid-19",
                                    IDCLASSE = "OCRID",
                                    CDELEMENT = cdElement,
                                    LGATU = caixaConfiguration.CdPessoa
                                },
                                transaction
                            );
                        }

                        //Marca inscricoes como cancelada
                        sql = "UPDATE INSCRICAO SET STINSCRI = 3, DTSTATUS = current date, HRSTATUS = current time, LGSTATUS = @LGSTATUS, " +
                            "  STCANCELAD = 0, DSSTATUS = '' " +
                            " WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";

                        await connection.ExecuteAsync(
                            sql,
                            new
                            {
                                inscricao.CDPROGRAMA,
                                inscricao.CDCONFIG,
                                inscricao.SQOCORRENC,
                                LGSTATUS = caixaConfiguration.CdPessoa
                            },
                            transaction
                        );

                        var nuVagasOcp = await ObterNumeroVagas(inscricao.CDPROGRAMA, inscricao.CDCONFIG, inscricao.SQOCORRENC);
                        nuVagasOcp--;

                        //Atualiza numero de vagas da turma
                        sql = "UPDATE PROGOCORR SET NUVAGASOCP = @NUVAGASOCP WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";
                        await connection.ExecuteAsync(
                            sql, new
                            {
                                nuVagasOcp,
                                inscricao.CDPROGRAMA,
                                inscricao.CDCONFIG,
                                inscricao.SQOCORRENC,
                            },
                            transaction
                        );

                        await transaction.CommitAsync();
                        return "";
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        Console.Write(e);
                        return $"Erro ao cancelar inscrição: {e.Message}";
                    }
                }
            }
        }

        public async Task<string> CancelaInscricaoTurma(List<string> turmas)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var turma in turmas)
                        {
                            var turmaArray = turma.Split('.');
                            var CDPROGRAMA = turmaArray[0];
                            var CDCONFIG = turmaArray[1];
                            var SQOCORRENC = turmaArray[2];

                            //Obtem todas as inscriçoes da turma
                            var sql = "SELECT * FROM INSCRICAO WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";
                            var inscricoes = await connection.QueryAsync<INSCRICAO>(
                                sql,
                                new
                                {
                                    CDPROGRAMA,
                                    CDCONFIG,
                                    SQOCORRENC
                                },
                                transaction
                            );

                            //Percorre as inscrições
                            foreach (var inscricao in inscricoes)
                            {
                                sql = "SELECT SQCANCELA FROM HSTCANINSC WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC AND CDPROGRAMA = @CDPROGRAMA " +
                                    "  AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC" +
                                    " ORDER BY SQCANCELA DESC";
                                var SQCANCELA = await connection.QueryFirstOrDefaultAsync<int>(
                                    sql,
                                    inscricao,
                                    transaction
                                );

                                //Insere registro de cancelamento
                                var hstcaninsc = new HSTCANINSC
                                {
                                    CDUOP = inscricao.CDUOP,
                                    SQMATRIC = inscricao.SQMATRIC,
                                    CDPROGRAMA = inscricao.CDPROGRAMA,
                                    CDCONFIG = inscricao.CDCONFIG,
                                    SQOCORRENC = inscricao.SQOCORRENC,
                                    CDCANCELA = 83, //COVID-19
                                    SQCANCELA = SQCANCELA > 0 ? SQCANCELA + 1 : 1,
                                    LGATU = caixaConfiguration.CdPessoa.ToString()
                                };
                                sql = "INSERT INTO HSTCANINSC(CDCANCELA, CDPROGRAMA, CDCONFIG, SQOCORRENC, CDUOP, SQMATRIC, SQCANCELA, DTATU, HRATU, LGATU)" +
                                    " VALUES(@CDCANCELA, @CDPROGRAMA, @CDCONFIG, @SQOCORRENC, @CDUOP, @SQMATRIC, @SQCANCELA, CURRENT DATE, CURRENT TIME, @LGATU)";
                                await connection.ExecuteAsync(
                                    sql,
                                    hstcaninsc,
                                    transaction
                                );

                            }

                            var cdElement = CDPROGRAMA.ToString().PadLeft(8, '0') + CDCONFIG.ToString().PadLeft(8, '0') + SQOCORRENC.ToString().PadLeft(8, '0');

                            //Cancela cobrancas vencidas
                            sql = "DELETE FROM COBRANCA " +
                                "WHERE IDCLASSE = @IDCLASSE AND CDELEMENT = @CDELEMENT AND STRECEBIDO = 0 AND DTVENCTO >= CURRENT DATE";

                            await connection.ExecuteAsync(
                                sql,
                                new
                                {
                                    IDCLASSE = "OCRID",
                                    CDELEMENT = cdElement,
                                },
                                transaction
                            );

                            //Apaga cobrancas futuras
                            sql = "UPDATE COBRANCA SET STRECEBIDO = 2, DTATU = current date, HRATU = current time, DSCANCELAM = @DSCANCELAM, CDCANCELA = 81, " +
                                " LGATU = @LGATU, LGCANCEL = @LGATU " +
                                "WHERE IDCLASSE = @IDCLASSE AND CDELEMENT = @CDELEMENT AND STRECEBIDO = 0";

                            await connection.ExecuteAsync(
                                sql,
                                new
                                {
                                    DSCANCELAM = "Cancelamento automático Covid-19",
                                    IDCLASSE = "OCRID",
                                    CDELEMENT = cdElement,
                                    LGATU = caixaConfiguration.CdPessoa.ToString()
                                },
                                transaction
                            );

                            //Marca inscricoes como canceladas
                            sql = "UPDATE INSCRICAO SET STINSCRI = 3, DTSTATUS = current date, HRSTATUS = current time, LGSTATUS = @LGSTATUS, " +
                                "  STCANCELAD = 0, DSSTATUS = '' " +
                                " WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";

                            await connection.ExecuteAsync(
                                sql,
                                new
                                {
                                    CDPROGRAMA,
                                    CDCONFIG,
                                    SQOCORRENC,
                                    LGSTATUS = caixaConfiguration.CdPessoa.ToString()
                                },
                                transaction
                            );

                            //Atualiza numero de vagas da turma
                            sql = "UPDATE PROGOCORR SET NUVAGASOCP = 0 WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC";
                            await connection.ExecuteAsync(
                                sql, new
                                {
                                    CDPROGRAMA,
                                    CDCONFIG,
                                    SQOCORRENC,
                                },
                                transaction
                            );

                        }
                        await transaction.CommitAsync();
                        return "";
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        Console.Write(e);
                        return $"Erro ao cancelar a turma: {e.Message}";
                    }
                }
            }
        }

        /// <summary>
        /// Obtem os usuários da atividade
        /// </summary>
        /// <param name="turma"></param>
        /// <returns></returns>
        public async Task<List<UsuariosAtividade>> ObtemUsuariosTurma(Turma turma)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var cdElement = turma.CDPROGRAMA.ToString().PadLeft(8, '0') + turma.CDCONFIG.ToString().PadLeft(8, '0') + turma.SQOCORRENC.ToString().PadLeft(8, '0');
                try
                {
                    var sql = "	 SELECT TRIM(CLI.NMCLIENTE) NMCLIENTE, CLI.DTVENCTO, CLI.DTNASCIMEN, INS.CDUOP, INS.SQMATRIC, INS.CDPROGRAMA, INS.CDCONFIG, INS.SQOCORRENC, INS.CDFORMATO, " +
                        " INS.CDFONTEINF, INS.CDPERFIL, INS.STINSCRI, INS.DTINSCRI, INS.LGINSCRI, INS.NUCOBRANCA, INS.CDUOPINSC, INS.DTSTATUS, INS.HRSTATUS, " +
                        " INS.LGSTATUS, INS.CDUOPSTAT, TRIM(CAT.DSCATEGORI) DSCATEGORI, TRIM(UOP.NMUOP) NMUOP, VALUE(COBATRINS.NUCOBRANCA, 0) NUCOBEXC, VALUE(COBGERINS.NUCOBRANCA, 0) NUCOBREST " +
                        " FROM CLIENTELA AS CLI, CATEGORIA AS CAT, UOP, INSCRICAO AS INS  LEFT OUTER JOIN COBATRINS ON COBATRINS.CDELEMENT = @CDELEMENT " +
                        " AND COBATRINS.CDUOP = INS.CDUOP AND COBATRINS.SQMATRIC = INS.SQMATRIC LEFT OUTER JOIN COBGERINS ON COBGERINS.CDELEMENT = @CDELEMENT  " +
                        " AND COBGERINS.CDUOP = INS.CDUOP AND COBGERINS.SQMATRIC = INS.SQMATRIC WHERE INS.CDPROGRAMA = @CDPROGRAMA AND INS.CDCONFIG = @CDCONFIG AND INS.SQOCORRENC = @SQOCORRENC " +
                        " AND CLI.CDUOP = INS.CDUOP AND CLI.SQMATRIC = INS.SQMATRIC AND CAT.CDCATEGORI = CLI.CDCATEGORI AND INS.CDUOPINSC = UOP.CDUOP ORDER BY CLI.NMCLIENTE";
                    var inscricoes = await connection.QueryAsync<UsuariosAtividade>(
                        sql,
                        new
                        {
                            cdElement,
                            turma.CDPROGRAMA,
                            turma.CDCONFIG,
                            turma.SQOCORRENC
                        }
                    );
                    foreach (var inscricao in inscricoes)
                    {
                        inscricao.CONTATOS = await ObtemContatos(inscricao.CDUOP, inscricao.SQMATRIC);
                    }
                    return inscricoes.ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    return null;
                }
            }
        }
        public async Task<List<CONTATOS>> ObtemContatos(int cduop, int sqmatric)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                try
                {
                    var sql = "	 SELECT CDUOP, SQMATRIC, SQCONTATO, TPCONTATO, STPRINCIPAL, TRIM(NUDDD) NUDDD, TRIM(DSCONTATO) DSCONTATO," +
                        " TRIM(NMPESSOA) NMPESSOA, STRECEBEINFO FROM CONTATOS WHERE IDCLASSE = 'CLI01' AND CDUOP = @cduop AND SQMATRIC = @sqmatric";
                    var contatos = await connection.QueryAsync<CONTATOS>(
                        sql,
                        new
                        {
                            cduop,
                            sqmatric
                        }
                    );
                    return contatos.ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Obtem HISTORICO DE CANCELAMENTOS DA ATIVIDADE
        /// </summary>
        /// <param name="turma"></param>
        /// <returns></returns>
        public async Task<List<CancelamentosAtividade>> ObtemCancelamentosTurma(Turma turma)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                try
                {
                    var sql = "SELECT M.DSCANCELA, I.* from HSTCANINSC I INNER JOIN MOTCANCEL M ON I.CDCANCELA = I.cdcancela " +
                        " WHERE I.CDPROGRAMA = @CDPROGRAMA AND I.CDCONFIG = @CDCONFIG AND I.SQOCORRENC = @SQOCORRENC " +
                        " ORDER BY I.DTATU, I.HRATU";
                    var inscricoes = await connection.QueryAsync<CancelamentosAtividade>(
                        sql,
                        new
                        {
                            turma.CDPROGRAMA,
                            turma.CDCONFIG,
                            turma.SQOCORRENC
                        }
                    );
                    return inscricoes.ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    return null;
                }
            }
        }
        public async Task<List<PROGOCORR>> ObterTurmasSelect(int uop)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = " SELECT P.* from PROGOCORR AS P " +
                    " INNER JOIN PROGRAMAS AS PR ON P.CDPROGRAMA = PR.CDPROGRAMA " +
                    "WHERE DTFIMOCORR > current date AND CDUOP = @UOP ORDER BY DSUSUARIO";
                var turmas = (await connection.QueryAsync<PROGOCORR>(
                    sql,
                    new
                    {
                        uop
                    })).AsList();

                return turmas;
            }
        }
    }
}