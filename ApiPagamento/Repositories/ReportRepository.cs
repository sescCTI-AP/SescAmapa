using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Site;

namespace PagamentoApi.Repositories
{
    public class ReportRepository
    {
        private readonly CobrancaRepository cobrancaRepository;
        private readonly IConfiguration configuration;

        public ReportRepository(IConfiguration configuration, CobrancaRepository cobrancaRepository)
        {
            this.cobrancaRepository = cobrancaRepository;
            this.configuration = configuration;
        }


        public async Task<int> QtdClientesPlenoAtivos()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                int cliente = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT COUNT(*) FROM CLIENTELA C INNER JOIN CATEGORIA CT ON C.CDCATEGORI = CT.CDCATEGORI WHERE DATE(C.DTVENCTO) >= DATE(current date) AND CT.TPCATEGORI <= 1 ");
                return cliente;
            }
        }

        public async Task<int> QtdClientesAtividadeAtivos()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                int cliente = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT COUNT(*) FROM CLIENTELA C INNER JOIN CATEGORIA CT ON C.CDCATEGORI = CT.CDCATEGORI WHERE DATE(C.DTVENCTO) >= DATE(current date) AND CT.TPCATEGORI > 1");
                return cliente;
            }
        }

        public async Task<int> QtdClientesInativos()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                int cliente = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT COUNT(*) FROM CLIENTELA WHERE DTVENCTO < current date");
                return cliente;
            }
        }

        public async Task<int> QtdNovosClientes(int mes, int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                int cliente = await connection.QueryFirstOrDefaultAsync<int>(
                    @"SELECT COUNT(*) FROM CLIENTELA WHERE MONTH(DTINSCRI) = @mes AND YEAR(DTINSCRI) = @ano AND DTATU = DTINSCRI",
                    new
                    {
                        mes,
                        ano
                    });
                return cliente;
            }
        }

        public async Task<List<AtendenteRanking>> RankingAtendentes(int mes, int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT
                                P.NMPESSOA AS ATENDENTE,
                                COUNT(*) AS CADASTROS
                            FROM
                                CLIENTELA C
                            LEFT JOIN PESSOA P ON
                                VARCHAR(P.CDPESSOA) = CHAR(C.LGATU)
                            WHERE
                                MONTH(C.DTATU) = @mes
                                AND YEAR(C.DTATU) = @ano
                            GROUP BY
                                P.NMPESSOA
                            ORDER BY
                                2 DESC ";
                var ret = (await connection.QueryAsync<AtendenteRanking>(
                       sql,
                        new
                        {
                            mes,
                            ano
                        })).ToList();

                return ret;
            }
        }

        public async Task<int> QtdClientesRenovados(int mes, int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                int cliente = await connection.QueryFirstOrDefaultAsync<int>(
                    @"SELECT COUNT(*) FROM CLIENTELA WHERE MONTH(DTATU) = @mes AND YEAR(DTATU) = @ano AND DTATU > DTINSCRI AND DTVENCTO >= CURRENT DATE",
                    new
                    {
                        mes,
                        ano
                    });
                return cliente;
            }
        }

        public async Task<int> QtdClienteVencimentoMesAtual()
        {
            var mes = DateTime.Now.Month;
            var ano = DateTime.Now.Year;
            var trimestre = DateTime.Now.Month + 2;
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                int cliente = await connection.QueryFirstOrDefaultAsync<int>(
                    @"SELECT COUNT(*) FROM CLIENTELA WHERE MONTH(DTVENCTO) = @mes AND YEAR(DTVENCTO) = @ano",
                    new
                    {
                        mes,
                        trimestre,
                        ano
                    });
                return cliente;
            }
        }

        public async Task<List<CLIENTELA>> ObterClientesVencimentoTrimestre()
        {
            var mes = DateTime.Now.Month;
            var ano = DateTime.Now.Year;
            var trimestre = DateTime.Now.Month + 2;
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT NMCLIENTE, NUCPF, DTVENCTO, CDUOP, SQMATRIC, NUDV, DTNASCIMEN FROM CLIENTELA WHERE MONTH(DTVENCTO) >= @mes AND MONTH(DTVENCTO) <= @trimestre  AND YEAR(DTVENCTO) = @ano";
                var cliente = (await connection.QueryAsync<CLIENTELA>(
                       sql,
                        new
                        {
                            mes,
                            ano
                        })).ToList();

                return cliente;
            }
        }

        public async Task<int> QtdInscritoPorMes(int mes, int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                int cliente = await connection.QueryFirstOrDefaultAsync<int>(
                    @"select COUNT(*) from INSCRICAO WHERE STINSCRI = 0 AND MONTH(DTINSCRI) = @mes AND YEAR(DTINSCRI) = @ano",
                    new
                    {
                        mes,
                        ano
                    });
                return cliente;
            }
        }

        public async Task<int> QtdEvasoesMes(int mes, int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                int cliente = await connection.QueryFirstOrDefaultAsync<int>(
                    @"select COUNT(*) from INSCRICAO WHERE STINSCRI = 3 AND MONTH(DTINSCRI) = @mes AND YEAR(DTINSCRI) = @ano",
                    new
                    {
                        mes,
                        ano
                    });
                return cliente;
            }
        }

        public async Task<List<InscricoesEvasoesViewModel>> InscricoesEvasoes(int ano)
        {
            var inscricoes = await InscricoesMesaMes(ano);
            var evasoes = await EvasoesMesaMes(ano);
            var listaIE = new List<InscricoesEvasoesViewModel>();
            int[] meses = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            foreach (var mes in meses)
            {
                var ie = new InscricoesEvasoesViewModel();
                ie.SeqMes = mes;
                ie.Mes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes);

                if (inscricoes.Any(i => i.MES == mes))
                {
                    ie.Inscricoes = inscricoes.FirstOrDefault(a => a.MES == mes).CONTAGEM;
                }
                else
                {
                    ie.Inscricoes = 0;
                }

                if (evasoes.Any(i => i.MES == mes))
                {
                    ie.Evasoes = evasoes.FirstOrDefault(a => a.MES == mes).CONTAGEM;
                }
                else
                {
                    ie.Evasoes = 0;
                }

                listaIE.Add(ie);

            }
            return listaIE;
        }

        public async Task<List<RegistroInscricaoEvasao>> InscricoesMesaMes(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"select MONTH(DTINSCRI) AS MES, count(*) AS CONTAGEM from INSCRICAO " +
                    " WHERE STINSCRI = 0 AND YEAR(DTINSCRI) = @ano group by MONTH(DTINSCRI)";
                var registros = (await connection.QueryAsync<RegistroInscricaoEvasao>(
                       sql,
                        new
                        {
                            ano
                        })).ToList();
                return registros;
            }
        }

        public async Task<List<RegistroInscricaoEvasao>> EvasoesMesaMes(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"select MONTH(DTINSCRI) AS MES, count(*) AS CONTAGEM from INSCRICAO " +
                    " WHERE STINSCRI = 3 AND YEAR(DTINSCRI) = @ano group by MONTH(DTINSCRI)";
                var registros = (await connection.QueryAsync<RegistroInscricaoEvasao>(
                       sql,
                        new
                        {
                            ano
                        })).ToList();
                return registros;
            }
        }

        public async Task<List<CLIENTELA>> ClientesSemCpf()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT * FROM CLIENTELA WHERE DTVENCTO > current date and NUCPF IS NULL";
                var cliente = (await connection.QueryAsync<CLIENTELA>(sql)).ToList();

                return cliente;
            }
        }

        public async Task<int> QtdClientesMatriculados()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var cliente = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT Count(DISTINCT(C.NUCPF)) FROM CLIENTELA C " +
                    "INNER JOIN INSCRICAO I ON " +
                    "C.CDUOP = I.CDUOP AND C.SQMATRIC = I.SQMATRIC " +
                    "INNER JOIN PROGOCORR P ON " +
                    "I.CDPROGRAMA = P.CDPROGRAMA AND I.CDCONFIG = P.CDCONFIG " +
                    "WHERE C.DTVENCTO >= current date AND " +
                    "I.STINSCRI = 0 AND " +
                    "P.DTFIMOCORR > current date");
                return cliente;
            }
        }

        public async Task<int> QtdClientesInadimplentes()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var cliente = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT COUNT(DISTINCT(CONCAT(CDUOP, SQMATRIC))) AS CONTAGEM FROM COBRANCA WHERE STRECEBIDO = 0 AND DTVENCTO < current date AND DTVENCTO > current date - 5 YEARS");
                return cliente;
            }
        }

        public async Task<List<PlataformaInscricao>> GetPlataformaInscricao(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT CDFONTEINF AS FONTE, COUNT(*) AS CONTAGEM FROM INSCRICAO WHERE STINSCRI = 0 AND YEAR(DTINSCRI) = @ano GROUP BY CDFONTEINF";
                var registros = (await connection.QueryAsync<PlataformaInscricao>(
                       sql,
                        new
                        {
                            ano
                        })).ToList();
                return registros;
            }
        }

        public async Task<List<ClientePerfilInscricao>> GetInscritosAno(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT C.NMCLIENTE, P.DSUSUARIO, (DAYS(current date) - DAYS(C.DTNASCIMEN))/365 AS IDADE FROM INSCRICAO " +
                    "I LEFT JOIN CLIENTELA C ON I.CDUOP = C.CDUOP AND I.SQMATRIC = C.SQMATRIC " +
                    "LEFT JOIN PROGOCORR P ON I.CDPROGRAMA = P.CDPROGRAMA AND I.CDCONFIG = P.CDCONFIG AND I.SQOCORRENC = P.SQOCORRENC WHERE YEAR(I.DTINSCRI) = @ano AND I.STINSCRI = 0 ORDER BY C.DTNASCIMEN";
                var registros = (await connection.QueryAsync<ClientePerfilInscricao>(
                       sql,
                        new
                        {
                            ano
                        })).ToList();
                return registros;
            }
        }


        public async Task<VagasProdutividade> VagasProdutividade(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT SUM(NUVAGAS) as VAGASTOTAIS, SUM(NUVAGASOCP) AS VAGASOCUPADAS FROM PROGOCORR WHERE " +
                    "YEAR(DTINIOCORR) = 2021 AND NUVAGAS < 500 AND DSUSUARIO NOT LIKE '%TAXA%' OR (YEAR(DTINIOCORR) = @ano AND NUVAGAS < 500 AND " +
                    "DSUSUARIO NOT LIKE '%TAXA%' AND DSUSUARIO LIKE 'MUSCULA%') AND VBCANCELA = 0";
                var registros = (await connection.QueryFirstOrDefaultAsync<VagasProdutividade>(
                       sql,
                        new
                        {
                            ano
                        }));
                return registros;
            }
        }

        public async Task<List<int>> GetHorariosAtendCanc(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT HOUR(HRATU) FROM HSTCANINSC WHERE YEAR(DTATU) > @ano";
                var registros = (await connection.QueryAsync<int>(
                       sql,
                        new
                        {
                            ano
                        })).ToList();
                return registros;
            }
        }

        public async Task<List<int>> GetHorariosAtendInscricao(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT HOUR(HRSTATUS) FROM INSCRICAO WHERE CDFONTEINF = 4 AND YEAR(DTSTATUS) > @ano";
                var registros = (await connection.QueryAsync<int>(
                       sql,
                        new
                        {
                            ano
                        })).ToList();
                return registros;
            }
        }

        public async Task<List<int>> GetHorariosAtendCredencial(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT HOUR(HRATU) FROM CLIENTELA WHERE YEAR(DTATU) > @ano";
                var registros = (await connection.QueryAsync<int>(
                       sql,
                        new
                        {
                            ano
                        })).ToList();
                return registros;
            }
        }

        public async Task<List<int>> GetHorariosAtendCobranca(int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @"SELECT HOUR(HRATU) FROM COBRANCA WHERE STRECEBIDO = 1 AND YEAR(DTATU) > @ano AND LGATU != '23482'";
                var registros = (await connection.QueryAsync<int>(
                       sql,
                        new
                        {
                            ano
                        })).ToList();
                return registros;
            }
        }

        public async Task<List<HorarioAtendimento>> GetHorariosAtendimento(int ano)
        {
            var anosHistorico = ano - 2;
            var horariosCobranca = await GetHorariosAtendCobranca(anosHistorico);

            var horariosCancelamento = await GetHorariosAtendCanc(anosHistorico);
            var horariosInscricao = await GetHorariosAtendInscricao(anosHistorico);
            var horariosCredencial = await GetHorariosAtendCredencial(anosHistorico);
            var listaHorarios = new List<int>();
            listaHorarios.AddRange(horariosCancelamento);
            listaHorarios.AddRange(horariosInscricao);
            listaHorarios.AddRange(horariosCredencial);
            listaHorarios.AddRange(horariosCobranca);
            var listaAgrupada = listaHorarios.GroupBy(h => h).Select(h => new HorarioAtendimento{ Hora = h.Key, Count = h.Count()}).ToList();
            return listaAgrupada.OrderBy(a => a.Hora).ToList();
        }


        public async Task<List<InscritosPorFaixaEtaria>> GetFaixaEtariaInscricoes(int ano)
        {
            var lista = new List<InscritosPorFaixaEtaria>();
            var inscritos = await GetInscritosAno(ano);
            var inscritosDistinct = inscritos.Select(a => new { nome = a.NMCLIENTE, idade = a.IDADE }).Distinct().ToList();
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "0-10",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 0 && a.idade <= 10)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "11-14",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 10 && a.idade <= 14)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "15-18",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 14 && a.idade <= 18)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "19-22",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 18 && a.idade <= 22)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "23-30",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 22 && a.idade <= 30)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "31-40",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 30 && a.idade <= 40)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "41-50",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 40 && a.idade <= 50)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "51-60",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 50 && a.idade <= 60)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "61-70",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 60 && a.idade <= 70)
            });
            lista.Add(new InscritosPorFaixaEtaria
            {
                CLASSE = "+71",
                CONTAGEM = inscritosDistinct.Count(a => a.idade > 70)
            });

            return lista;
        }

        public async Task<RelatorioIndicadores> ProcessaIndicadores(int? mesSubmit, int? anoSubmit)
        {
            #region getMesAno
            var mes = 0;
            var ano = 0;
            if (mesSubmit == null)
            {
                mes = DateTime.Now.Month;
            }
            else
            {
                mes = (int)mesSubmit;
            }
            if (anoSubmit == null)
            {
                ano = DateTime.Now.Year;
            }
            else
            {
                ano = (int)anoSubmit;
            }
            #endregion

            var clientesPleno = await QtdClientesPlenoAtivos();
            var clientesAtividade = await QtdClientesAtividadeAtivos();
            var clientesVenvidos = await QtdClientesInativos();
            var novosClientes = await QtdNovosClientes(mes, ano);
            var clientesRenovados = await QtdClientesRenovados(mes, ano);
            var clienteVencimentoMes = await QtdClienteVencimentoMesAtual();
            var inscritosMes = await QtdInscritoPorMes(mes, ano);
            var evasoesMes = await QtdEvasoesMes(mes, ano);
            var clientesSemCpf = await ClientesSemCpf();
            var clientesMatriculados = await QtdClientesMatriculados();
            var clientesInadimplentes = await QtdClientesInadimplentes();
            var rankingAtendentes = await RankingAtendentes(mes, ano);
            var inscricoesEvasoes = await InscricoesEvasoes(ano);
            var plataformaInscricao = await GetPlataformaInscricao(ano);
            var clientesPerfil = await GetFaixaEtariaInscricoes(ano);
            var vagasProdutividade = await VagasProdutividade(ano);
            var horariosAtendimento = await GetHorariosAtendimento(ano);

            return new RelatorioIndicadores
            {
                ClientesPleno = clientesPleno,
                ClientesAtividade = clientesAtividade,
                ClientesVencidos = clientesVenvidos,
                NovosCliente = novosClientes,
                ClientesRenovados = clientesRenovados,
                ClienteVencimentoNoMes = clienteVencimentoMes,
                InscritosMes = inscritosMes,
                EvasoesMes = evasoesMes,
                ClientesSemCpf = clientesSemCpf,
                ClientesMatriculados = clientesMatriculados,
                ClientesInadimplentes = clientesInadimplentes,
                AtendentesRaking = rankingAtendentes,
                InscricoesEvasoes = inscricoesEvasoes,
                PlataformaInscricao = plataformaInscricao,
                InscritosPorFaixaEtaria = clientesPerfil,
                VagasProdutividade = vagasProdutividade,
                HorariosAtendimento = horariosAtendimento
            };
        }
    }
}
