using Microsoft.AspNetCore.Mvc;
using SiteSesc.Repositories;
using System.Security.Claims;

namespace SiteSesc.Components
{
    public class CardMensalidadesViewComponent : ViewComponent
    {
        private readonly ClienteRepository _clienteRepository;
        public CardMensalidadesViewComponent(ClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;            
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cpf = ((ClaimsIdentity)User.Identity).FindFirst("CPF").Value;
            var matriculas = await _clienteRepository.GetMatriculas(cpf);
            return View(matriculas);
        }
    }
}
