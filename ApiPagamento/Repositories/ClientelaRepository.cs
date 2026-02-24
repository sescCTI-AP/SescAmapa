using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Site;
using PagamentoApi.Models.Termo;
using PagamentoApi.Services;
using PagamentoApi.SignalR;
using SiteSesc.Models;

namespace PagamentoApi.Repositories
{
    public class ClientelaRepository
    {
        private readonly CobrancaRepository cobrancaRepository;
        private readonly IConfiguration configuration;
        public ClientelaRepository(IConfiguration configuration, CobrancaRepository cobrancaRepository)
        {
            this.cobrancaRepository = cobrancaRepository;
            this.configuration = configuration;
        }

        public async Task<bool> EstaAtivo(string cpf)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();

                var dataAtual = DateTime.Now;   
                var sql = @"SELECT count(DTVENCTO) FROM CLIENTELA WHERE NUCPF=@cpf and DTVENCTO >= @dataAtual";
                var cliente = await connection.QuerySingleAsync<bool>( sql, new { cpf = cpf, dataAtual = dataAtual } );

                return cliente;
            }
        }

        public async Task<CLIENTELA> getClientePorCpf(string cpf)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT CDUOP, CDUOTITUL, SQTITULMAT, SQMATRIC, CDCATEGORI, DTVENCTO , TRIM(NMCLIENTE) NMCLIENTE, TRIM(NMSOCIAL) NMSOCIAL, " +
                        " DTNASCIMEN, STMATRIC, NUCPF, FOTO, NUMCARTAO, NUDV, NUCGCCEI, NMPAI, CDSEXO, NMMAE, TRIM(DSNATURAL) DSNATURAL, TRIM(DSNACIONAL) DSNACIONAL, " +
                        " NUCTPS, TRIM(NUREGGERAL) NUREGGERAL, VLRENDA, NUPISPASEP, TRIM(DSCARGO) DSCARGO, DTEMIRG " +
                        " FROM CLIENTELA " +
                        " WHERE NUCPF = @cpf AND STMATRIC = 0";
                var cliente = await connection.QueryFirstOrDefaultAsync<CLIENTELA>(
                        sql,
                        new
                        {
                            cpf = cpf
                        });
                if (cliente != null)
                {
                    cliente.COBRANCA = await cobrancaRepository.obterCobrancasEmAberto(cliente);
                    cliente.ENDERECOS = await this.ObterEnderecos(cliente.CDUOP, cliente.SQMATRIC);
                }
                return cliente;
            }
        }

        public async Task<int?> getTipoCategoria(int id)
        {
            using(var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT TPCATEGORI FROM CATEGORIA WHERE CDCATEGORI = @id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add(new DB2Parameter("@id", id));

                    var result = await command.ExecuteScalarAsync();

                   return result != null && int.TryParse(result.ToString(), out var tipoCategoria)
                   ? tipoCategoria
                   : (int?)null;
                }
            }

        }

        public async Task<string> getCategoria(int id)
        {
            using(var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT DSCATEGORI FROM CATEGORIA WHERE CDCATEGORI = @id";

                using(var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add(new DB2Parameter("@id", id));

                    var result = await command.ExecuteScalarAsync();

                    return result != null ? result.ToString() : null;
                }
            }
        }

        public async Task<List<MunicipioSelect>> ObterMunicipios()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                
                var sql = @"SELECT CDMUNICIP, DSMUNICIP, SIESTADO FROM MUNICIPIO m ";
                List<MunicipioSelect> municipios = (await connection.QueryAsync<MunicipioSelect>(
                       sql
                        )).ToList();
                return municipios;
            }
        }

        private async Task<List<ENDERECOS>> ObterEnderecos(int cduop, int sqmatric)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT E.IDCLASSE, E.CDELEMENT, E.SQENDEREC, E.DSLOGRADOU, E.SIESTADO, E.DSCOMPLEM, M.DSMUNICIP, E.CDMUNICIP, E.NUIMOVEL, E.DSBAIRRO, " +
                " E.NUCEP, E.STPRINCIP  FROM ENDERECOS E JOIN MUNICIPIO  M ON E.CDMUNICIP = M.CDMUNICIP WHERE E.CDUOP = @CDUOP AND E.SQMATRIC = @SQMATRIC ORDER BY E.SQENDEREC";
                List<ENDERECOS> enderecos = (await connection.QueryAsync<ENDERECOS>(
                       sql,
                        new
                        {
                            cduop,
                            sqmatric
                        })).ToList();
                return enderecos;
            }
        }
        public async Task<List<ClientelaApp>> ObterClientePorCpfSemCobrancas(string cpf)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                // var sql = @"SELECT C.CDUOP, C.SQMATRIC, C.CDCATEGORI, DTVENCTO, NMCLIENTE, DTNASCIMEN, STMATRIC, NUCPF, FOTO, NUMCARTAO, MATFORMAT, DSCATEGORI FROM CLIENTELA AS C " +
                //         " JOIN CLIFORMAT AS F ON C.CDUOP = F.CDUOP AND C.SQMATRIC = F.SQMATRIC " +
                //         " JOIN CATEGORIA AS CAT ON C.CDCATEGORI = CAT.CDCATEGORI WHERE NUCPF = @cpf";

                var sql = @"(SELECT C.CDUOP, C.NUREGGERAL, C.IDORGEMIRG, C.SQMATRIC, C.CDCATEGORI, DTVENCTO, TRIM(NMCLIENTE) AS NMCLIENTE, TRIM(C.NMSOCIAL) AS NMSOCIAL, DTNASCIMEN, STMATRIC, NUCPF, FOTO, C.NUMCARTAO, TRIM(MATFORMAT) AS Matricula, TRIM(DSCATEGORI) AS Categoria, CAT.TPCATEGORI AS TipoCategoria, SQTITULMAT, CART.CDBARRA AS CdBarras FROM CLIENTELA AS C " +
                        " JOIN CLIFORMAT AS F ON C.CDUOP = F.CDUOP AND C.SQMATRIC = F.SQMATRIC " +
                        " LEFT JOIN CARTAO AS CART ON C.NUMCARTAO = CART.NUMCARTAO " +
                        " JOIN CATEGORIA AS CAT ON C.CDCATEGORI = CAT.CDCATEGORI WHERE NUCPF = @cpf AND STMATRIC = 0 UNION ALL " +
                        @"SELECT C.CDUOP, c.NUREGGERAL, C.IDORGEMIRG, C.SQMATRIC, C.CDCATEGORI, DTVENCTO, TRIM(NMCLIENTE) AS NMCLIENTE,  TRIM(C.NMSOCIAL) AS NMSOCIAL, DTNASCIMEN, STMATRIC, NUCPF, FOTO, C.NUMCARTAO, TRIM(MATFORMAT) As Matricula, TRIM(DSCATEGORI) AS Categoria, CAT.TPCATEGORI AS TipoCategoria, SQTITULMAT, CART.CDBARRA AS CdBarras FROM CLIENTELA AS C " +
                        " JOIN CLIFORMAT AS F ON C.CDUOP = F.CDUOP AND C.SQMATRIC = F.SQMATRIC " +
                        " LEFT JOIN CARTAO AS CART ON C.NUMCARTAO = CART.NUMCARTAO " +
                        " JOIN CATEGORIA AS CAT ON C.CDCATEGORI = CAT.CDCATEGORI WHERE C.STMATRIC = 0 AND C.SQTITULMAT = (SELECT SQMATRIC FROM CLIENTELA AS C3 WHERE NUCPF = @cpf AND STMATRIC = 0) AND " +
                        " C.CDUOTITUL = (SELECT CDUOP FROM CLIENTELA AS C3 WHERE NUCPF = @cpf AND STMATRIC = 0)) ORDER BY SQTITULMAT DESC";
                List<ClientelaApp> cliente = (await connection.QueryAsync<ClientelaApp>(
                       sql,
                        new
                        {
                            cpf = cpf
                        })).ToList();
                return cliente;
            }
        }

        public async Task<CLIENTELA> ObterClientePorCpfSemFoto(string cpf)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var clientelaDictionary = new Dictionary<string, CLIENTELA>();
                var cliente = await connection.QueryFirstOrDefaultAsync<CLIENTELA>(
                    @"SELECT CDUOP, SQMATRIC, CDCATEGORI, DTVENCTO, NMCLIENTE, DTNASCIMEN, STMATRIC, NUCPF, NUMCARTAO FROM CLIENTELA WHERE NUCPF = @CPF AND STMATRIC = 0",
                    new
                    {
                        cpf,
                    });
                return cliente;
            }
        }
        public async Task<CLIENTELA> ObterClientePorMatricula(int CDUOP, int SQMATRIC)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var clientelaDictionary = new Dictionary<string, CLIENTELA>();
                var cliente = await connection.QueryFirstOrDefaultAsync<CLIENTELA>(
                    @"SELECT CDUOP, SQMATRIC, CDCATEGORI, DTVENCTO, NMCLIENTE, DTNASCIMEN, STMATRIC, NUCPF FROM CLIENTELA WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC",
                    new
                    {
                        CDUOP,
                        SQMATRIC
                    });
                return cliente;
            }
        }

        public async Task<byte[]> ObterFotoClientePorMatricula(int CDUOP, int SQMATRIC)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var foto = await connection.QueryFirstOrDefaultAsync<byte[]>(
                    @"SELECT FOTO FROM CLIENTELA WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC",
                    new
                    {
                        CDUOP,
                        SQMATRIC
                    });
                return foto;
            }
        }

        public async Task<List<CLIENTELA>> ObterDependentesPorMatricula(int CDUOP, int SQMATRIC)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var clientelaDictionary = new Dictionary<string, CLIENTELA>();
                var cliente = await connection.QueryFirstOrDefaultAsync<List<CLIENTELA>>(
                    @"SELECT CDUOP, SQMATRIC, CDCATEGORI, DTVENCTO, NMCLIENTE, DTNASCIMEN, STMATRIC, NUCPF FROM CLIENTELA WHERE CDUOTITUL = @CDUOP AND SQTITULMAT = @SQMATRIC",
                    new
                    {
                        CDUOP,
                        SQMATRIC
                    });
                return cliente;
            }
        }

        public async Task<CLIENTELA> ObterClientePorMatriculaComFoto(int CDUOP, int SQMATRIC)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var clientelaDictionary = new Dictionary<string, CLIENTELA>();
                var cliente = await connection.QueryFirstOrDefaultAsync<CLIENTELA>(
                    @"SELECT * FROM CLIENTELA WHERE CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC",
                    new
                    {
                        CDUOP,
                        SQMATRIC
                    });

                if (cliente != null)
                {
                    cliente.ENDERECOS = await this.ObterEnderecos(cliente.CDUOP, cliente.SQMATRIC);
                }
                return cliente;
            }
        }


        public async Task<List<ClientelaSummary>> ObterClientePorUnidade(int? CDUOP, int SKIP, int TAKE)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var clientelaDictionary = new Dictionary<string, CLIENTELA>();
                var sql = "SELECT CL.NMCLIENTE, CL.NMSOCIAL, CL.NUCPF, CL.CDUOP, CL.SQMATRIC, CL.NUDV, " +
                    "CL.DTVENCTO, CL.DTNASCIMEN, CL.CDCATEGORI, C.DSCATEGORI AS CATEGORIA, CL.NMPAI, CL.NMMAE   " +
                    " FROM CLIENTELA CL JOIN CATEGORIA C ON CL.CDCATEGORI = C.CDCATEGORI  ";

                if (CDUOP != null && CDUOP > 0)
                {
                    sql += " WHERE CDUOP = @CDUOP";
                }

                sql += " LIMIT @SKIP, @TAKE";


                List<ClientelaSummary> cliente = (await connection.QueryAsync<ClientelaSummary>(
                    sql,
                    new
                    {
                        CDUOP = CDUOP,
                        SKIP= SKIP,
                        TAKE = TAKE
                    }
                    )).ToList();
                return cliente;
            }
        }


        public async Task<ClientelaSummary> ObterClientePorCobranca(string CDELEMENT, int SQCOBRANCA)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var clientelaDictionary = new Dictionary<string, CLIENTELA>();
                var sql = "SELECT C.NMCLIENTE, C.NMSOCIAL, C.NUCPF, C.CDUOP, C.SQMATRIC, C.NUDV, C.DTVENCTO, C.DTNASCIMEN, " +
                    "C.CDCATEGORI, C.NMPAI, C.NMMAE FROM COBRANCA CB JOIN CLIENTELA C ON CB.CDUOP = C.CDUOP " +
                    "AND CB.SQMATRIC = C.SQMATRIC  WHERE CB.CDELEMENT = @CDELEMENT AND CB.SQCOBRANCA = @SQCOBRANCA";

                ClientelaSummary cliente = await connection.QueryFirstOrDefaultAsync<ClientelaSummary>(
                    sql,
                    new
                    {
                        CDELEMENT = CDELEMENT,
                        SQCOBRANCA = SQCOBRANCA
                    });
                return cliente;
            }
        }


        public async Task<SescTO_UsuarioClientela> ObterUsuarioPorCpf(string cpf)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var usuario = await connection.QueryFirstOrDefaultAsync<SescTO_UsuarioClientela>(@"SELECT * FROM SESCTOBOLETO_USUARIO_CLIENTELA WHERE NUCPF = @CPF", new { CPF = cpf });
                return usuario;
            }
        }



        public async Task<TermoSignature> ObterTermo(string cdelement, string cpf)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("TERMOBD")))
            {
                try
                {
                    var sql = "SELECT * FROM TermoSignature WHERE Cdelement = @cdelement and Cpf = @cpf";
                    var cardapio = await connection.QueryFirstOrDefaultAsync<TermoSignature>(sql, new { cdelement = cdelement, cpf = cpf });
                    return cardapio;
                }
                catch(Exception e) { 
                    Console.WriteLine(e);
                    return null;
                }
            }
        }



        public async Task<List<CardapioUnidade>> ObterCardapio()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                var sql = "SELECT c.DataInicio, c.DataFinal, cc.DiaDaSemana, i.Nome, u.Nome as NomeUnidade, c.IdUnidadeOperacional, i.IdGrupoItemCardapio FROM ComposicaoCardapio cc JOIN Cardapio c ON c.Id = cc.IdCardapio JOIN UnidadeOperacional u\r\nON c.IdUnidadeOperacional = u.Id  JOIN ItemCardapio i ON i.Id = cc.IdItemCardapio WHERE c.IsAtivo = 1";
                var cardapio = await connection.QueryAsync<Cardapio>(sql);

                if (cardapio != null)
                {
                    var validaPeriodo = cardapio.First();
                    if (DateTime.Now.Date >= validaPeriodo.DataInicio && DateTime.Now.Date <= validaPeriodo.DataFinal)
                    {
                        var c = new Cardapio();
                        var hoje = DateTime.Today.DayOfWeek.ToString().ToLower();
                        var dia = 0;

                        switch (hoje)
                        {
                            case "monday":
                                dia = 1;
                                break;
                            case "tuesday":
                                dia = 2;
                                break;
                            case "wednesday":
                                dia = 3;
                                break;
                            case "thursday":
                                dia = 4;
                                break;
                            case "friday":
                                dia = 5;
                                break;
                            default:
                                break;
                        }

                        cardapio = cardapio.Where(c => c.DiaDaSemana == dia).ToList();
                        var cardapioAgrupado = cardapio.GroupBy(a => a.IdUnidadeOperacional);
                        var chavesDistintas = cardapioAgrupado.Select(g => g.Key).Distinct();

                        var listaCardapios = new List<CardapioUnidade>();
                        foreach (var chave in chavesDistintas)
                        {
                            var itensCardapio = new List<ItemCardapio>();
                            var itens = cardapio.Where(c => c.IdUnidadeOperacional == chave);
                            if (itens.Count() > 0)
                            {
                                foreach (var it in itens)
                                {
                                    itensCardapio.Add(new ItemCardapio { nome = it.Nome.ToUpper(), IdGrupoItemCardapio = it.IdGrupoItemCardapio });
                                }
                                var cardapioUnidade = new CardapioUnidade
                                {
                                    NomeUnidade = itens.First().NomeUnidade,
                                    Itens = itensCardapio
                                };
                                listaCardapios.Add(cardapioUnidade);
                            }
                        }
                        return listaCardapios;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        public async Task<bool> HasCliente(string NUCPF)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT * FROM CLIENTELA WHERE NUCPF = @NUCPF";
                var hasCliente = await connection.QueryAsync(sql, new { NUCPF });
                if (hasCliente.Count() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public static byte[] ConvertBase64ToByteArray(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                throw new ArgumentException("A string Base64 � nula ou est� vazia");
            }

            return Convert.FromBase64String(base64String);
        }

        public async Task<int?> ObterUtilmoSqmatric(int? CDUOP)
        {
            if (CDUOP != null)
            {
                using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT SQMATRIC FROM CODMATUOP WHERE CDUOP = @CDUOP";
                    var sqmatric = await connection.QuerySingleOrDefaultAsync<int>(sql, new { CDUOP });
                    if (sqmatric > 0)
                    {
                        return sqmatric;
                    }
                    return 0;
                }
            }
            return null;

        }

        public async Task<dynamic> AddCliente(ClienteDb2 cliente)
        {
            var cduopBase = 276;

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();

                //Unidade padrão para cadastro de cliente web - SEDE

                //Verifica se o cliente já possui cadastro
                var hasCliente = await HasCliente(cliente.NUCPF);
                if (!hasCliente)
                {

                    //var atualizaSqmatric = await AtualizaSqmatric(cduopBase);
                    var ultimoSqmatric = await ObterUtilmoSqmatric(cduopBase);

                    var sqmatric = (int)ultimoSqmatric + 1;
                    var lgatu = "web";

                    cliente.CDUOP = cduopBase;
                    cliente.SQMATRIC = sqmatric;
                    cliente.NUDV = 1;
                    cliente.LGATU = lgatu.ToString();

                    var CDELEMENT = $"{cliente.CDUOP.ToString().PadLeft(4, '0')}-{cliente.SQMATRIC.ToString().PadLeft(6, '0')}-1";
                    cliente.ENDERECO.CDELEMENT = CDELEMENT;
                    cliente.ENDERECO.LGATU = lgatu.ToString();
                    cliente.ENDERECO.CDUOP = cduopBase;
                    cliente.ENDERECO.SQMATRIC = sqmatric;
                    if (!string.IsNullOrEmpty(cliente.NUCTPS))
                        cliente.NUCTPS = cliente.NUCTPS.Replace(".", "").Replace("-", "");
                    if (!string.IsNullOrEmpty(cliente.NUPISPASEP))
                        cliente.NUPISPASEP = cliente.NUPISPASEP.Replace(".", "").Replace("-", "");

                    if (!string.IsNullOrEmpty(cliente.FOTO64))
                    {
                        var fotobyte = ConvertBase64ToByteArray(cliente.FOTO64);
                        var fotoRedimensionada = Util.RedimensionarImagem(fotobyte, 32000);
                        cliente.FOTO = fotoRedimensionada;
                    }
                    Console.WriteLine($"Tamanho da foto: {cliente.FOTO.Length} bytes.");

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            var sqlSqmatric = "UPDATE CODMATUOP SET SQMATRIC = SQMATRIC + 1 WHERE CDUOP = @cduopBase";
                            await connection.ExecuteAsync(sqlSqmatric,
                                new { cduopBase = cduopBase },
                                transaction
                            );

                            var sqlClass = "SELECT CDCLASSIF FROM CSE";
                            var cdclass = await connection.QueryFirstOrDefaultAsync<string>(sqlClass, transaction: transaction);
                            cliente.CDCLASSIF = cdclass;

                            var sqlCartao = "SELECT MAX(NUMCARTAO) FROM CARTAO";

                            var NUMCARTAO = await connection.ExecuteScalarAsync<int?>(sqlCartao, transaction: transaction);
                            if (NUMCARTAO.HasValue && NUMCARTAO.Value > 0)
                            {
                                NUMCARTAO = NUMCARTAO.Value + 1;
                                var CDBARRA = $"000000001{cliente.CDUOP.ToString().PadLeft(4, '0')}{cliente.SQMATRIC.ToString().PadLeft(6, '0')}{cliente.NUDV}";
                                var sqlInsert = "INSERT INTO CARTAO (NUMCARTAO, TPCONTRL, PSWCART, CDBARRA, TPCARTAO, DTCADASTRO, DTATU, HRATU, LGATU) " +
                                                "VALUES (@NUMCARTAO, 1, '70D6001E222FC191', @CDBARRA, 1, CURRENT DATE, CURRENT DATE, CURRENT TIME, @LGATU)";
                                await connection.ExecuteAsync(sqlInsert, new { NUMCARTAO = NUMCARTAO, CDBARRA = CDBARRA, LGATU = cliente.LGATU }, transaction);
                                cliente.NUMCARTAO = NUMCARTAO.Value;
                            }

                            //Verificar empresa
                            if (!string.IsNullOrEmpty(cliente.NUCGCCEI))
                            {
                                var sqlEmpresa = "SELECT * FROM EMPRESA WHERE NUCGCCEI = @NUCGCCEI";
                                var emp = await connection.QueryFirstOrDefaultAsync<string>(sqlEmpresa, new { NUCGCCEI = cliente.NUCGCCEI }, transaction: transaction);
                                if (string.IsNullOrEmpty(emp))
                                {
                                    var empresaWs = await Util.GetDadosEmpresa(cliente.NUCGCCEI);

                                    var nome = empresaWs.Nome;
                                    var fantasia = empresaWs.Fantasia;
                                    var atividadePrincipal = empresaWs.Atividade_Principal.First();

                                    empresaWs.Nome = nome.Length > 50 ? nome.Substring(0, 50) : nome;
                                    empresaWs.Fantasia = fantasia.Length > 50 ? fantasia.Substring(0, 50) : fantasia;
                                    if (empresaWs != null)
                                    {
                                        var cnae = atividadePrincipal.Code.Replace(".", "").Replace("-", "");
                                        sqlEmpresa = "INSERT INTO EMPRESA (NUCGCCEI, IDCGCCEI, NMRAZSOC, VBCONTRIB, VBSIMPLES, NMFANTASIA, DTINSCR, QTMATRIC, HRATU, SMFIELDATU, DTATU, LGATU, CDCNAE, VBCONTRAT, TEOBS, CDEMPPDV, VBFATURA, VBDEBFOL, CDSITUACAO, DTSITUACAO, STEMPRESA, SQCNAE, CDCNAE2, CDFPAS, VBINTEGRATOTVS, CNAESECUNDARIO)" +
                                                        "VALUES " +
                                                        "(@NUCGCCEI, 0, @NMRAZSOC, 0, 0, @NMFANTASIA, CURRENT DATE, 0, CURRENT TIME, 0, CURRENT DATE, 'WEB', '', 0, '', 0, 0, 0, 1, CURRENT DATE, 1, '00', @CDCNAE2, '515', 0, 0);";
                                        var affectedRowsEmpresa = await connection.ExecuteAsync(sqlEmpresa, new { NUCGCCEI = cliente.NUCGCCEI, NMRAZSOC = nome, NMFANTASIA = fantasia, CDCNAE2 = cnae }, transaction);
                                        if (affectedRowsEmpresa <= 0)
                                        {
                                            //return "Falha ao cadastrar empresa.";
                                            return new { success = false, error = "Falha ao Cadastrar o Empresa" };
                                        }
                                    }
                                }
                            }

                            var sql = "INSERT INTO CLIENTELA  (CDUOP, SQMATRIC, CDCLASSIF, NUDV, NUCGCCEI, CDCATEGORI, CDNIVEL, SQTITULMAT, CDUOTITUL, STMATRIC, DTINSCRI, CDMATRIANT, DTVENCTO," +
                                "NMCLIENTE, NMSOCIAL, DTNASCIMEN, NMPAI, CDSEXO, NMMAE, CDESTCIVIL, VBESTUDANT, NUULTSERIE, DSNATURAL, DSNACIONAL, NUCTPS, DTADMISSAO, " +
                                "DTDEMISSAO, NUREGGERAL, VLRENDA, NUCPF, NUPISPASEP, VLRENDAFAM, DSCARGO, DTEMIRG, IDORGEMIRG, DTATU, STEMICART, HRATU, LGATU, SMFIELDATU, TEOBS, " +
                                "NRVIACART, SITUPROF, TIPOIDENTIDADE, COMPIDENTIDADE, VBPCG, VBEMANCIPADO, VBPCD, VBNOMEAFETIVO, IDNACIONAL, NUMCARTAO, FOTO) " +

                                "VALUES " +

                                "(@CDUOP, @SQMATRIC, @CDCLASSIF, 1, @NUCGCCEI, @CDCATEGORI, @CDNIVEL, @SQTITULMAT, @CDUOTITUL, 0,   CURRENT DATE,  NULL, ADD_YEARS(CURRENT DATE, 2), @NMCLIENTE , @NMSOCIAL , " + 
                                " @DTNASCIMEN, @NMPAI, @CDSEXO, @NMMAE, @CDESTCIVIL, @VBESTUDANT, @NUULTSERIE, @DSNATURAL, " +
                                " @DSNACIONAL, @NUCTPS, @DTADMISSAO, @DTDEMISSAO, @NUREGGERAL, @VLRENDA, @NUCPF, @NUPISPASEP, @VLRENDAFAM, @DSCARGO, @DTEMIRG, @IDORGEMIRG, CURRENT DATE, 1, " + 
                                " CURRENT TIME, @LGATU, 0, NULL, 1, " +
                                "@SITUPROF, 0, NULL, 0, 0, @VBPCD, 0, '2be47058-9969-4b32-b0b8-350442c38a7c', @NUMCARTAO, @FOTO) ";

                            //erro de chave estrangeira de CSE
                            var affectedRows = await connection.ExecuteAsync(sql,cliente,transaction);
                            if (affectedRows <= 0)
                            {
                                //return "Falha ao cadastrar cliente.";
                                return new { seccess = false, error = "Falha ao cadastrar cliente" };
                            }
                            var categoria = cliente.CDCATEGORI == 1 ? "Trabalhador do comércio" : "Público em geral";
                            var OBS = "Inclusão de cliente no tipo de categoria " + categoria;
                            sql = "INSERT INTO OCORENCER(CDOCORRENC, SQOCORRENC, CDUOP, SQMATRIC, CDUOPOCORR, DTOCORRENC, HROCORRENC, TEOBS, DTATU, HRATU, LGATU) " +
                                    "VALUES" +
                                    "(15, 1, @CDUOP, @SQMATRIC, @CDUOP, CURRENT_DATE, CURRENT_TIME, @OBS, CURRENT DATE, CURRENT TIME, @LGATU)";

                            await connection.ExecuteAsync(sql, new { CDUOP = cduopBase, SQMATRIC = sqmatric, LGATU = lgatu, OBS = OBS }, transaction);

                            #region Endereco

                            var sqlEndereco = "INSERT INTO ENDERECOS  (IDCLASSE, CDELEMENT, SQENDEREC, SIESTADO, CDMUNICIP, DSLOGRADOU, NUIMOVEL, DSCOMPLEM, DSBAIRRO, NUCEP, STPRINCIP, SMFIELDATU, CDUOP, SQMATRIC, DTATU, HRATU, LGATU) " +
                                "VALUES  ('CLI01', @CDELEMENT, 1, @SIESTADO, @CDMUNICIP, @DSLOGRADOU, @NUIMOVEL, @DSCOMPLEM, @DSBAIRRO, @NUCEP, 1, -1, @CDUOP, @SQMATRIC, CURRENT DATE, CURRENT TIME, @LGATU) ";

                            var affectedRowsEndereco = await connection.ExecuteAsync(sqlEndereco, cliente.ENDERECO, transaction);
                            
                            #endregion

                            Contato contato = new Contato
                            {
                                CDELEMENT = CDELEMENT,
                                TPCONTATO = 1,
                                STPRINCIPAL = 1,
                                NUDDD = cliente.TELEFONE.Substring(1,2),
                                DSCONTATO = cliente.TELEFONE.Substring(5, 10).Replace("-",""),
                                NMPESSOA = cliente.NMCLIENTE.Length > 40 ? cliente.NMCLIENTE.ToUpper().Substring(0,40) : cliente.NMCLIENTE.ToUpper(),
                                LGATU = "web",
                                CDUOP = cliente.CDUOP,
                                SQMATRIC = cliente.SQMATRIC,
                                STRECEBEINFO = 1
                            };

                            var sqlContato = "INSERT INTO CONTATOS (IDCLASSE, SQCONTATO, CDELEMENT, TPCONTATO, STPRINCIPAL, NUDDD, DSCONTATO, NMPESSOA, SMFIELDATU, LGATU, CDUOP, SQMATRIC, STRECEBEINFO, DTATU, HRATU) " +
                                             "VALUES (@IDCLASSE, 1, @CDELEMENT, @TPCONTATO, @STPRINCIPAL, @NUDDD, @DSCONTATO, @NMPESSOA, @SMFIELDATU, @LGATU, @CDUOP, @SQMATRIC, @STRECEBEINFO, @DTATU, @HRATU) ";

                            var affectedRowsContato = await connection.ExecuteAsync(sqlContato, contato, transaction);

                            if(cliente.EMAIL != null)
                            {
                                Contato contato2 = new Contato
                                {
                                    CDELEMENT = CDELEMENT,
                                    TPCONTATO = 1,
                                    STPRINCIPAL = 0,
                                    NUDDD = null,
                                    DSCONTATO = cliente.EMAIL.ToLower(),
                                    NMPESSOA = cliente.NMCLIENTE.ToUpper(),
                                    LGATU = "web",
                                    CDUOP = cliente.CDUOP,
                                    SQMATRIC = (short)cliente.SQMATRIC,
                                    STRECEBEINFO = 1
                                };

                                var sqlContato2 = "INSERT INTO CONTATOS (IDCLASSE, SQCONTATO, CDELEMENT, TPCONTATO, STPRINCIPAL, NUDDD, DSCONTATO, NMPESSOA, HRATU, SMFIELDATU, DTATU, LGATU, CDUOP, SQMATRIC, STRECEBEINFO) " +
                                                 "VALUES ('CLI01', 2, @CDELEMENT, @TPCONTATO, @STPRINCIPAL, @NUDDD, @DSCONTATO, @NMPESSOA, CURRENT TIME, @SMFIELDATU, CURRENT DATE, 'web',@CDUOP, @SQMATRIC, @STRECEBEINFO);";

                                var affectedRowsContato2 = await connection.ExecuteAsync(sqlContato2, contato2, transaction);
                            }

                            await transaction.CommitAsync();
                            return cliente;
                        }
                        catch (Exception e)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }

                }
                return new { success = false, error = "Cliente ja adicionado" };
            }
        }

        public async Task<RESPCLIENTELACONTRATO?> ObterClientelaResponsavel(int cdUop = 0, int sqMatric = 0)
        {
            if (cdUop != 0 && sqMatric != 0)
            {
                using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
                {
                    await connection.OpenAsync();

                    var sql =
                            "SELECT " +
                            "    c.NUCPF, " +
                            "    c.NUREGGERAL, " +
                            "    c.IDORGEMIRG, " +
                            "    r2.NMRESPONSA, " +
                            "    (SELECT " +
                            "       e2.DSLOGRADOU || ' ' || " +
                            "       e2.NUIMOVEL || ' ' || " +
                            "       TRIM(e2.DSBAIRRO) || ' ' || " +
                            "       (SELECT TRIM(m.DSMUNICIP) || '/' || m.SIESTADO FROM MUNICIPIO m WHERE m.CDMUNICIP = e2.CDMUNICIP) " +
                            "   FROM ENDERECOS e2 WHERE e2.CDELEMENT = '0' || 328 || '-0' || 51507 || '-1') AS DSLOGRADOU " +
                            "FROM RESPCLI r " +
                            "INNER JOIN RESPONSAVEIS r2 ON r.NUCPF = r2.NUCPF " +
                            "INNER JOIN CLIENTELA c ON r2.NUCPF = c.NUCPF " +
                            "WHERE r.CDUOP = @CDUOP AND r.SQMATRIC = @SQMATRIC; ";
                    var responsavel = await connection.QuerySingleOrDefaultAsync<RESPCLIENTELACONTRATO>(sql, new { CDUOP = cdUop, SQMATRIC = sqMatric });
                    if (responsavel is RESPCLIENTELACONTRATO)
                    {
                        return responsavel;
                    }
                }
            }
            return null;
        }

        public async Task<bool> GetDoisFatoresHabilitado(string cpf)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                await connection.OpenAsync();

                var sql =
                        "SELECT DoisFatoresHabilitado FROM Usuario u WHERE u.Cpf = @CPF ";
                var responsavel = await connection.QuerySingleOrDefaultAsync<bool>(sql, new { CPF = cpf });
                
                return responsavel;
                
            }
        }

        public async Task<bool> AtualizaDoisFatores(string cpf, bool valorDoisFatores)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                await connection.OpenAsync();

                var sql =
                        "UPDATE Usuario SET DoisFatoresHabilitado = @VALORDOISFATORES WHERE Cpf = @CPF";
                var responsavel = await connection.ExecuteAsync(sql, new { VALORDOISFATORES = valorDoisFatores, CPF = cpf });

                return responsavel == 1 ? true : false;

            }
        }
    }
}