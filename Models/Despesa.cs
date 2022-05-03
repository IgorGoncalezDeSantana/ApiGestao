using APIGestao.Contracts;
using APIGestao.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGestao.Models
{
    public class Despesa: IValidate
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public float Valor { get; set; }
        public long IDEmpresa { get; set; }

        public void Validate(EValidateType type)
        {
            if (String.IsNullOrEmpty(Descricao))
                throw new Exception("O campo descrição é obrigatório");

            if (Data == DateTime.MinValue)
                throw new Exception("O campo data é obrigatório");

            if (Valor <= 0)
                throw new Exception("O campo valor é obrigatório");

            if (IDEmpresa <= 0)
                throw new Exception("O campo ID Empresa é obrigatório");
        }
    }
}
