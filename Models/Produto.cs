using APIGestao.Context;
using APIGestao.Contracts;
using APIGestao.Enums;
using APIGestao.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace APIGestao.Models
{
    public class Produto : IValidate
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public float PrecoCusto { get; set; }
        public float PrecoVenda { get; set; }
        public float Peso { get; set; }
        public float Altura { get; set; }
        public float Largura { get; set; }
        public float Comprimento { get; set; }
        public int Quantidade { get; set; }
        public long IDEmpresa { get; set; }
        
        public Produto()
        {
        }

        public Produto(string nome, string descricao, float precoCusto, float precoVenda, float peso, float altura, float largura, float comprimento, int quantidade, long idEmpresa)
        {
            Nome = nome;
            Descricao = descricao;
            PrecoCusto = precoCusto;
            PrecoVenda = precoVenda;
            Peso = peso;
            Altura = altura;
            Largura = largura;
            Comprimento = comprimento;
            Quantidade = quantidade;
            IDEmpresa = idEmpresa;
        }

        public void Validate(EValidateType type)
        {
            if (IDEmpresa <= 0)
                throw new ValidateException("O campo ID Empresa é obrigatório");

            if (!(type == EValidateType.vtDelete) && string.IsNullOrEmpty(Nome))
                throw new ValidateException("O campo nome é obrigatório!");
           
            if (!(type == EValidateType.vtDelete) && PrecoVenda <= 0)
                throw new ValidateException("O preço de venda deve ser preenchido com um valor positivo!");
      
            if (!(type == EValidateType.vtDelete) && Quantidade <= 0)
                throw new ValidateException("A quantidade deve ser preenchido com um valor positivo!");
            
            if (!(type == EValidateType.vtDelete) && PrecoCusto < 0)
                throw new ValidateException("O preço de venda deve ser preenchido com um valor positivo!");

        }
    }
}