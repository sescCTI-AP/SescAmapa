using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models.Site;


namespace PagamentoApi.Repositories
{
    public class NoticiaRepository
    {
        private readonly IConfiguration configuration;
        public NoticiaRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<Noticia>> ObterNoticias(int? qtd)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                var sql = "SELECT TOP (@qtd) n.TituloLongo, n.TituloCurto, n.DataUltimaAtualizacao, n.CorpoNoticia, " +
                    "a.CaminhoVirtual FROM Noticia n JOIN Arquivo a ON n.IdArquivo = a.Id ORDER BY DataUltimaAtualizacao DESC";
                var noticias = await connection.QueryAsync<Noticia>(sql,
                        new
                        {
                            qtd = qtd
                        });

                return noticias.ToList();
            }
        }
    }
}