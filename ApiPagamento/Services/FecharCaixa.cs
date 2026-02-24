using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PagamentoApi.Models;
using PagamentoApi.Repositories;

namespace PagamentoApi.Services
{
    public class FecharCaixa : CronJobService
    {
        private readonly ILogger<FecharCaixa> _logger;
        private readonly IServiceProvider _serviceProvider;

        public FecharCaixa(IScheduleConfig<FecharCaixa> config, ILogger<FecharCaixa> logger, IServiceProvider serviceProvider) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Atualizar Boletos Rodando.");
            using(var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                CaixaRepository caixaRepository = scope.ServiceProvider.GetRequiredService<CaixaRepository>();
                _logger.LogInformation("Inicio fechamento");
                var caixa = await caixaRepository.ObterCaixaAberto();
                if (caixa is CACAIXA)
                {
                    _logger.LogInformation("Fechando caixa");
                    var result = await caixaRepository.EfetuarRetiradaDoCaixa();
                    if (result is CAIXALANCA)
                    {
                        _logger.LogInformation("Saldo retirado do caixa.");
                        var resultFechamento = await caixaRepository.FecharCaixa();
                        if (resultFechamento.Codigo == 0)
                        {
                            _logger.LogInformation("Caixa fechado com sucesso.");
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Não existe caixa aberto");
                }
            }


            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Iniciando fechamento de caixa TEF.");
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                CaixaRepository caixaRepository = scope.ServiceProvider.GetRequiredService<CaixaRepository>();
                _logger.LogInformation("Inicio fechamento caixa tef");
                var caixa = await caixaRepository.ObterCaixaAberto(true);
                if (caixa is CACAIXA)
                {
                    _logger.LogInformation("Fechando caixa tef");
                    var result = await caixaRepository.EfetuarRetiradaDoCaixa(true);
                    if (result is CAIXALANCA)
                    {
                        _logger.LogInformation("Saldo retirado do caixa.");
                        var resultFechamento = await caixaRepository.FecharCaixa(true);
                        if (resultFechamento.Codigo == 0)
                        {
                            _logger.LogInformation("Caixa tef fechado com sucesso.");
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Não existe caixa tef aberto");
                }
            }
        }
    }
}