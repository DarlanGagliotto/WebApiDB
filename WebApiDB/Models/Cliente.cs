using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiDB.Models
{
    public class Cliente
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public bool IsPremium { get; set; }
        public string Cidade { get; set; }
    }
}