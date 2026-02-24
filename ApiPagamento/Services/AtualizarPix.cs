using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagamentoApi.Clients;
using PagamentoApi.Repositories;

namespace PagamentoApi.Services
{
    public class AtualizarPix : CronJobService
    {
        private readonly ILogger<AtualizarPix> _logger;
        private readonly IServiceProvider _serviceProvider;
        public AtualizarPix(IScheduleConfig<AtualizarPix> config, ILogger<AtualizarPix> logger, IServiceProvider serviceProvider)
         : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Atualizar Pix Rodando.");
            var dataAtual = DateTime.Now;
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
               // BBClient bbClient = scope.ServiceProvider.GetRequiredService<BBClient>();
               // ApiBancoBrasilService bbClient = scope.ServiceProvider.GetRequiredService<ApiBancoBrasilService>();
                PixRepository pixRepository = scope.ServiceProvider.GetRequiredService<PixRepository>();


                if ( DateTime.Now.Hour > 5 && !( DateTime.Now.Hour == 23 && DateTime.Now.Minute > 50 ) )
                {
                    // Atualizar pix em aberto
                    var retorno = await pixRepository.AtualizarPixAtivos();
                    /*
                    if (retorno)
                    {
                        _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Pix Atualizados.");
                    }
                    else
                    {
                        _logger.LogError($"Erro ao atualizar pix: {retorno}");
                    }*/
                }
            }
        }
    }
}