using System.Collections.Generic;

namespace PagamentoApi.SignalR
{  
    public class ClienteConectado
    {
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
    public class Cliente
    {
        public string? ConnectionId { get; set; }
        public string? TxId { get; set; }
    }
}
