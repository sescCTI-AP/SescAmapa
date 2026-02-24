using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagamentoApi.Repositories
{
    public class VendasRepository
    {
        public readonly IConfiguration _configuration;

        public VendasRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtem as vendas por item
        /// </summary>
        /// <param name="vendas"></param>
        /// <returns></returns>
        public async Task<List<ItemVenda>> ObterVendasPorItens(DateTime inicio, DateTime fim, int? cduop = null)
        {
            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();

                string sql = "";

                sql += "SELECT ";
                sql += "    P.DSPRODUTO AS PRODUTO, ";
                sql += "    G.NMGRUPO AS GRUPO, ";
                sql += "    S.NMSUBGRPDV AS SUBGRUPO, ";
                sql += "    C.UNIDADEVENDA, ";
                sql += "    C.CDUOP, ";
                sql += "    C.LOCALVENDA, ";
                sql += "    I.QTPRODUTO AS QUANTIDADE, ";
                sql += "    I.VLITEM AS VALORUNITARIO, ";
                sql += "    I.VLRECEBIDO AS VALORRECEBIDO, ";
                sql += "    I.VLDESCITEM, ";
                sql += "    CASE ";
                sql += "        WHEN V.STVENDA = 0 THEN 'CONCLUÍDA' ";
                sql += "        WHEN V.STVENDA = 1 THEN 'CANCELADA' ";
                sql += "        ELSE NULL ";
                sql += "    END AS STATUSVENDA, ";
                sql += "    V.STVENDA, ";
                sql += "    V.DSCANCELAM, ";
                sql += "    TIMESTAMP(V.DTVENDA, V.HRVENDA) AS DATAVENDA, ";
                sql += "    CASE ";
                sql += "        WHEN V.ECOMERCIARIO = 1 THEN 'TRABALHADOR DO COMÉRCIO' ";
                sql += "        ELSE 'PÚBLICO GERAL' ";
                sql += "    END AS CATEGORIACLIENTE, ";
                sql += "    V.ECOMERCIARIO ";
                sql += "FROM ";
                sql += "    ITEMVENDA I ";
                sql += "INNER JOIN PRODUTOPDV P ON ";
                sql += "    I.CDPRODUTO = P.CDPRODUTO ";
                sql += "INNER JOIN GRPPRODPDV G ON ";
                sql += "    P.CDGRUPOPDV = G.CDGRUPOPDV ";
                sql += "INNER JOIN SUBGRPRPDV S ON ";
                sql += "    P.CDGRUPOPDV = S.CDGRUPOPDV ";
                sql += "    AND P.CDSUBGRPDV = S.CDSUBGRPDV ";
                sql += "LEFT JOIN (";
                sql += "    SELECT ";
                sql += "        CLI.*, ";
                sql += "        V.*, ";
                sql += "        CASE ";
                sql += "            WHEN CLI.SUBECOMERCIARIO IS NULL THEN 0 ";
                sql += "            ELSE CLI.SUBECOMERCIARIO ";
                sql += "        END AS ECOMERCIARIO ";
                sql += "    FROM ";
                sql += "        VENDA V ";
                sql += "    LEFT JOIN (";
                sql += "        SELECT ";
                sql += "            CLI.SQMATRIC, ";
                sql += "            CLI.CDUOP, ";
                sql += "            CASE ";
                sql += "                WHEN CAT.TPCATEGORI = 0 OR CAT.TPCATEGORI = 1 THEN 1 ";
                sql += "                ELSE 0 ";
                sql += "            END AS SUBECOMERCIARIO ";
                sql += "        FROM ";
                sql += "            CLIENTELA CLI ";
                sql += "        LEFT JOIN CATEGORIA CAT ON ";
                sql += "            CLI.CDCATEGORI = CAT.CDCATEGORI ";
                sql += "    ) CLI ON ";
                sql += "        V.SQMATRIC = CLI.SQMATRIC ";
                sql += "        AND V.CDUOP = CLI.CDUOP ";
                sql += ") V ON ";
                sql += "    V.SQCAIXA = I.SQCAIXA ";
                sql += "    AND V.SQVENDA = I.SQVENDA ";
                sql += "    AND V.CDPESSOA = I.CDPESSOA ";
                sql += "LEFT JOIN (";
                sql += "    SELECT ";
                sql += "        C.*, ";
                sql += "        CASE ";
                sql += "            WHEN LV.DSLOCVENDA IS NULL THEN 'NÃO DEFINIDO' ";
                sql += "            ELSE LV.DSLOCVENDA ";
                sql += "        END AS LOCALVENDA, ";
                sql += "        U.NMUOP AS UNIDADEVENDA ";
                sql += "    FROM ";
                sql += "        CACAIXA C ";
                sql += "    LEFT JOIN LOCALVENDA LV ON ";
                sql += "        LV.CDLOCVENDA = C.CDLOCVENDA ";
                sql += "    LEFT JOIN UOP U ON ";
                sql += "        C.CDUOP = U.CDUOP ";
                sql += "    WHERE ";
                sql += "        U.CDADMIN = 15 ";
                sql += ") C ON ";
                sql += "    C.SQCAIXA = I.SQCAIXA ";
                sql += "    AND C.DTABERTURA = I.DTATU ";
                sql += "    AND C.CDPESSOA = I.CDPESSOA ";
                sql += "WHERE ";
                sql += "    I.DTATU BETWEEN @dataInicio AND @dataFim ";
                if (cduop != null && cduop.Value > 0)
                {
                    sql += "    AND C.CDUOP = @cduop ";
                }
                sql += "ORDER BY ";
                sql += "    DATAVENDA DESC;";

                object parametros = new { dataInicio = inicio, dataFim = fim };

                if (cduop != null)
                {
                    parametros = new { dataInicio = inicio, dataFim = fim, cduop = cduop };
                }

                var itemVenda = await connection.QueryAsync<ItemVenda>(sql, parametros);

                return itemVenda.ToList();
            }
        }
    }
}
