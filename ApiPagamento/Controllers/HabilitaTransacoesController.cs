using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PagamentoApi.Clients;
using PagamentoApi.DTOs;
using PagamentoApi.Models.BB;
using PagamentoApi.Models.Cielo;
using PagamentoApi.Models.HabilitaTransacoes;
using PagamentoApi.Models.Pix;
using PagamentoApi.Repositories;
using PagamentoApi.Settings;

namespace PagamentoApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]

    public class HabilitaTransacoesController : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public async Task<dynamic> HorarioDeGeracao()
        {            
            if (HabilitaTransacao.IsHorarioDeGeracao())
                return Ok(new ResponseGenericoResult(true, "", null));

            return Ok(new ResponseGenericoResult(false, HabilitaTransacao.messagem, null));
        }
    }
}