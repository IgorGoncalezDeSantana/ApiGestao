using APIGestao.Context;
using APIGestao.Contracts;
using APIGestao.Enums;
using APIGestao.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace APIGestao.Models
{
    public class PedidoItem : IValidate
    {
        public long Id { get; set; }
        public int Quantidade { get; set; }
        public float ValorUnitario { get; set; }
        public long IDProduto { get; set; }
        public long IDEmpresa { get; set; }
        public long IDPedido { get; set; }


        public PedidoItem()
        {
        }

        public PedidoItem(int quantidade, float valorUnitario, long iDProduto, long iDEmpresa, long iDPedido)
        {
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            IDProduto = iDProduto;
            IDEmpresa = iDEmpresa;
            IDPedido = iDPedido;
        }

        public void Validate(EValidateType type)
        {
            if (!(type == EValidateType.vtDelete) && Quantidade <= 0)
                throw new ValidateException("O campo quantidade deve ter um valor positivo!");
           
            if (!(type == EValidateType.vtDelete) && ValorUnitario <= 0)
                throw new ValidateException("O campo valor unitário deve ter um valor positivo!");

            if (IDEmpresa <= 0)
                throw new ValidateException("O campo ID Empresa é obrigatório");

            if (!(type == EValidateType.vtDelete) && IDProduto <= 0)
                throw new ValidateException("Um produto deve ser selecionado!");

            if (IDPedido <= 0)
                throw new ValidateException("O campo ID Pedido é obrigatório");
        }
    }
}