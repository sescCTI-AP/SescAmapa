using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models
{
    public class Log
    {
        public int Id { get; set; }

        public int? IdEvento { get; set; }

        public string? NomeEvento { get; set; }

        [Display(Name = "ACTION")]
        public string? Action { get; set; }

        [Display(Name = "ENTIDADE")]
        public string? Entidade { get; set; }

        [Display(Name = "DATA DE CADASTRO")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "USUÁRIO")]
        public string? Usuario { get; set; }

        [Display(Name = "CPF")]
        public string? Cpf { get; set; }

        [Display(Name = "MATRICULA")]
        public string? Matricula { get; set; }

        [Display(Name = "STATUS")]
        public int Status { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        public string? Descricao { get; set; }

        [Display(Name = "LATITUDE")]
        public string? Latitude { get; set; }

        [Display(Name = "LONGITUDE")]
        public string? Longitude { get; set; }
    }
}
