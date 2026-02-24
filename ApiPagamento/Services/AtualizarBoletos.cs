using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagamentoApi.Clients;
using PagamentoApi.Models.BB;
using PagamentoApi.Repositories;

namespace PagamentoApi.Services
{
    public class AtualizarBoletos : CronJobService
    {
        private readonly ILogger<AtualizarBoletos> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AtualizarBoletos(IScheduleConfig<AtualizarBoletos> config, ILogger<AtualizarBoletos> logger, IServiceProvider serviceProvider) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {

            
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Atualizar Boletos Rodando.");
            var dataAtual = DateTime.Now;
            using(var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                BBClient bbClient = scope.ServiceProvider.GetRequiredService<BBClient>();
                BoletoRepository boletoRepository = scope.ServiceProvider.GetRequiredService<BoletoRepository>();
                //Boletos baixados
                
                var boletosBaixados = await bbClient.ListarBoletos('B', 7, dataAtual, dataAtual);
                if (boletosBaixados is BoletoResponse)
                {
                   var retornoBaixados = await boletoRepository.BaixarBoletos(boletosBaixados.boletos, bbClient);
                    if (retornoBaixados)
                        _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Boletos Baixados.");
                }
                //Boletos pagos
                var retorno = await boletoRepository.LiquidarBoletos(bbClient);
                if (retorno is bool)
                {
                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Boletos Atualizados.");
                }
                else
                {
                    _logger.LogError($"Erro ao atualizar boletos: {retorno}");
                }
            }
            
        }

    }
}