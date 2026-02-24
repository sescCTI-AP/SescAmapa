using System;

namespace PagamentoApi.Models
{
    public partial class SescTO_UsuarioClientela
    {
        public int ID { get; set; }
        public string EMAIL { get; set; }
        public int CDUOP { get; set; }
        public int SQMATRIC { get; set; }
        public short NUDV { get; set; }
        public string SENHA { get; set; }
        public string NUCPF { get; set; }
        public DateTime DATACADASTRO { get; set; }
    }
}