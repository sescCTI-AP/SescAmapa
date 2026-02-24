using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagamentoApi.Models;
using PagamentoApi.Models.Cielo;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Site;
using PagamentoApi.Models.Termo;
using PagamentoApi.Repositories;
using PagamentoApi.Services;
using SiteSesc.Models;

namespace PagamentoApi.Controllers
{
    


    [ApiController]
    [Route("v1/[controller]")]
    public class SolicitacaoReembolsoController : ControllerBase
    {
        SolicitaaoReembolsoRepository _solicitaaoReembolsoRepository;
        public SolicitacaoReembolsoController(SolicitaaoReembolsoRepository solicitaaoReembolsoRepository)
        {
            _solicitaaoReembolsoRepository = solicitaaoReembolsoRepository;
        }

        [Authorize]
        [HttpGet("{cpf}")]
        public async Task<dynamic> Get(string cpf)
        {
            List<SolicitacaoReembolso> solicitacao = await _solicitaaoReembolsoRepository.GetSolicitacaoReembolso(cpf);
            return solicitacao;
        }

        [Authorize]
        [HttpGet("{cpf}/{cdelement}")]
        public async Task<dynamic> TermoReembolsoAssinado(string cpf, string cdelement)
        {
            TermoReembolsoAssinado solicitacao = await _solicitaaoReembolsoRepository.TermoReembolsoAssinado(cpf, cdelement);
            return solicitacao;
        }
    }
}