    using System.Collections.Generic;
    using System;

    namespace PagamentoApi.Models {
        public partial class SescTO_Usuario {
            public int ID { get; set; }
            public string NOME { get; set; }
            public int CDPESSOA { get; set; }
            public string USUARIOCAN { get; set; }
            public string USUARIODOMINIO { get; set; }
            public DateTime DATACADASTRO { get; set; }
            public int TIPO { get; set; }
        }
    }