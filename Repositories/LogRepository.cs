using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SiteSesc.Data;
using SiteSesc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Repositories
{
    public class LogRepository
    {
        private SiteSescContext _dbContext;
        private readonly IConfiguration configuration;

        public LogRepository([FromServices] SiteSescContext context, IConfiguration configuration)
        {
            this.configuration = configuration;
            _dbContext = context;
        }

        public async Task<List<Log>> GetAll()
        {
            try
            {
                var logs = _dbContext.Log.ToList();
                return logs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<Log>> LogPorPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                var logs = _dbContext.Log.ToList();
                return logs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void RegistrarLog(string action, string entidade, string usuario, int status, string descricao)
        {
            try
            {
                var log = new Log { 
                    Action = action,
                    DataCadastro = DateTime.Now,
                    Entidade = entidade,
                    Usuario = usuario,
                    Status = status,
                    Descricao = descricao
                };
                _dbContext.Add(log);
                _dbContext.SaveChanges();                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

    }
}
