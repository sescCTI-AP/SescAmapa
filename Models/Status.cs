using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class Status
    {
        public int Id { get; set; }

        public string Nome { get; set; }
        public bool IsAtivo { get; set; }

        public virtual ICollection<SolicitacaoCadastroCliente> SolicitacaoCadastroCliente { get; set; }


        public class Filtro
        {
            public static List<Status> GetLista()
            {
                return new List<Status>
                {
                    new Status{Id = 1, Nome = "Pendente"},
                    new Status{Id = 9, Nome = "Renovação"},
                    new Status{Id = 4, Nome = "Enviado para correção"},
                    new Status{Id = 8, Nome = "Finalizado"},
                    new Status{Id = 5, Nome = "Aprovado"}
            };
            }
        }
    }
}