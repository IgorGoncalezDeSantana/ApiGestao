using APIGestao.Context;
using APIGestao.Contracts;
using APIGestao.Enums;
using APIGestao.Exceptions;
using APIGestao.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APIGestao.Models
{
    public class Usuario : IValidate
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Sexo { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public long IDEmpresa { get; set; }

        public Usuario()
        {
        }

        public Usuario(string nome, string sobrenome, string cPF, string email, string login, string senha, string sexo, DateTime dataNascimento, string logradouro, string numero, string cep, string cidade, string estado, string pais, long idEmpresa)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            CPF = cPF;
            Email = email;
            Login = login;
            Senha = senha;
            Sexo = sexo;
            DataNascimento = dataNascimento;
            Logradouro = logradouro;
            Numero = numero;
            Cep = cep;
            Cidade = cidade;
            Estado = estado;
            Pais = pais;
            IDEmpresa = idEmpresa;
        }

        public void Validate(EValidateType type)
        {
            if (IDEmpresa <= 0)
                throw new ValidateException("O campo ID Empresa é obrigatório");

            if (!(type == EValidateType.vtDelete) && String.IsNullOrEmpty(Nome))
                throw new ValidateException("O campo nome é obrigatório!");
            
            if (!(type == EValidateType.vtDelete) && String.IsNullOrEmpty(Login))
                throw new ValidateException("O campo login é obrigatório!");
           
            if (type == EValidateType.vtInsert && String.IsNullOrEmpty(Senha))           
                throw new ValidateException("O campo senha é obrigatório!");
           
            if (!(type == EValidateType.vtDelete) && (DataNascimento == DateTime.MinValue))
                throw new ValidateException("O campo data de nascimento é obrigatório!");
           
            if (!(type == EValidateType.vtDelete) && !CPFUtils.IsCpf(CPF))
                throw new ValidateException("Digite um CPF válido!");

            if (!(type == EValidateType.vtDelete) && Senha.Length < 8)
                throw new ValidateException("A senha precisa ter no mínimo 8 dígitios!");

            if (!(type == EValidateType.vtDelete) && !EmailUtils.IsValidEmail(Email))
                throw new ValidateException("Digite um email válido!");

            if (!(type == EValidateType.vtDelete) && !CEPUtils.ValidaCep(Cep))
                throw new ValidateException("Digite um CEP válido!");
        }
    }
}
