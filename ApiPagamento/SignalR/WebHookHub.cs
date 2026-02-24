using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace PagamentoApi.SignalR
{
    public class WebHookHub : Hub
    {
        private readonly ClienteConectado _clienteConectado;

        public WebHookHub(ClienteConectado clienteConectados)
        {
            _clienteConectado = clienteConectados;
        }

        public async Task ConexaoCliente(string txId)
        {
            // await Clients.Client(Context.ConnectionId).SendAsync("initMessage");
            _clienteConectado.Clientes.Add(new Cliente { ConnectionId = Context.ConnectionId, TxId = txId });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _clienteConectado.Clientes.Remove(_clienteConectado.Clientes.Find(x => x.ConnectionId == Context.ConnectionId));
            await base.OnDisconnectedAsync(exception);
        }
    }
}
