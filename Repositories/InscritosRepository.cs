using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.MatriculaEscolar;
using SiteSesc.Models.SiteViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SiteSesc.Repositories
{
    public class InscritosRepository
    {
        private readonly SiteSescContext _dbContext;
        public readonly IConfiguration configuration;
        private readonly ApiClient _apiClient;


        public InscritosRepository(SiteSescContext dbContext,IConfiguration configuration, ApiClient apiClient)
        {
            this.configuration = configuration;
            _dbContext = dbContext;
            _apiClient = apiClient;
        }

        public async Task<List<mat_escolar>> GetAtivos()
        {
            var turmasAtivas = await _dbContext.mat_escolar
                .Include(e => e.IdUnidade)
                .Where(e => e.IsAtivo && e.DataInicio <= DateTime.Now && e.DataFim >= DateTime.Now)
                .ToListAsync(); // ToListAsync para operações assíncronas
            return turmasAtivas;
        }


        public async Task<mat_escolar> FindTurma(int id)
        {
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
                {
                    var sql = @$"SELECT * FROM mat_escolar WHERE Id = {id} AND IsAtivo = '1'";
                    var turma = await connection.QuerySingleAsync<mat_escolar>(sql);
                  

                    return turma;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro no DAO: " + ex.Message);
                return null;
            }
        }


        public async Task<mat_escolar_inscritos> GetInscrito(int id)
        {
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
                {
                    var sql = @$"SELECT * FROM mat_escolar_inscritos where Id = {id} ";
                    var inscrito = await connection.QuerySingleAsync<mat_escolar_inscritos>(
                        sql
                    );
                    return inscrito;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro no DAO: " + ex.Message);
                return null;
            }
        }

        public async Task<List<mat_escolar_inscritos>> Inscritos(int id)
        {
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
                {
                    var sql = @$"SELECT * FROM mat_escolar_inscritos where idTurma = {id} ";
                    var inscrito = await connection.QueryAsync<mat_escolar_inscritos>(
                        sql
                    );
                    return inscrito.ToList();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro no DAO: " + ex.Message);
                return null;
            }
        }     
        
    }
}
