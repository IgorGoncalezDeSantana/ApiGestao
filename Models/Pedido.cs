using APIGestao.Context;
using APIGestao.Contracts;
using APIGestao.Enums;
using APIGestao.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGestao.Models
{
    public class Pedido : IValidate
    {
        public long Id { get; set; }
        public long IDCliente { get; set; }
        public DateTime Date { get; set; }
        public string MetodoPagamento { get; set; }
        public string Status { get; set; }
        public long IDEmpresa { get; set; }

        public Pedido()
        {
        }

        public Pedido(long idCliente, DateTime date, string metodoPagamento, string status, long idEmpresa)
        {
            IDCliente = idCliente;
            Date = date;
            MetodoPagamento = metodoPagamento;
            Status = status;
            IDEmpresa = idEmpresa;
        }

        public void Validate(EValidateType type)
        {
            if (IDEmpresa <= 0)
                throw new ValidateException("O campo ID Empresa é obrigatório");

            if (!(type == EValidateType.vtCancel) && !(type == EValidateType.vtDelete) && IDCliente <= 0)
                throw new ValidateException("Um cliente deve ser selecionado!");
            
            if (!(type == EValidateType.vtCancel) && !(type == EValidateType.vtDelete) && String.IsNullOrEmpty(MetodoPagamento))
                throw new ValidateException("Um método de pagamento deve ser selecionado!");
            
            if (!(type == EValidateType.vtCancel) && (type == EValidateType.vtUpdate) && Status != "Pendente")
                throw new ValidateException("Não é possível editar um pedido com o status diferente de pendente!");
            
            if (type == EValidateType.vtRealize)
            {
                if (Status != "Pendente")
                    throw new ValidateException("Não é possível realizar um pedido com status diferente de pendente!");     
            }

            if (type == EValidateType.vtCancel)
            {
                if (Status != "Pendente")
                    throw new ValidateException("Não é possível cancelar um pedido com status diferente de pendente!");
            }

            if (type == EValidateType.vtDelete && Status != "Pendente")
                throw new ValidateException("Não é possível excluir um pedido com o status diferente de pendente!");

        }
    }
}
