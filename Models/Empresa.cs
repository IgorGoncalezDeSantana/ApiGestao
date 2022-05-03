using APIGestao.Contracts;
using APIGestao.Enums;
using APIGestao.Exceptions;
using APIGestao.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGestao.Models
{
    public class Empresa: IValidate
    {
        public long Id { get; set; }
        public string RazaoSocial { get; set; }
        public string Fantasia { get; set; }
        public string CNPJ { get; set; }
        public Guid AccessToken { get; set; }

        public Empresa()
        {
        }

        public Empresa(string razaoSocial, string fantasia, string cNPJ)
        {
            RazaoSocial = razaoSocial;
            Fantasia = fantasia;
            CNPJ = cNPJ;
        }

        public void Validate(EValidateType type)
        {
            if(string.IsNullOrEmpty(RazaoSocial))
                throw new ValidateException("O campo Razão Social é obrigatório!");

            if (string.IsNullOrEmpty(Fantasia))
                throw new ValidateException("O campo Fantasia é obrigatório!");

            if (string.IsNullOrEmpty(CNPJ))
                throw new ValidateException("O campo CNPJ é obrigatório!");

            if (!CNPJUtils.IsCnpj(CNPJ))
                throw new ValidateException("O campo CNPJ foi preenchido incorretamente!");
        }
    }
}
