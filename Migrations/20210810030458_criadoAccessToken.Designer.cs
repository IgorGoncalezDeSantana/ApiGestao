﻿// <auto-generated />
using System;
using APIGestao.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace APIGestao.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20210810030458_criadoAccessToken")]
    partial class criadoAccessToken
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.8");

            modelBuilder.Entity("APIGestao.Models.Cliente", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("CPF")
                        .HasColumnType("longtext");

                    b.Property<string>("Cep")
                        .HasColumnType("longtext");

                    b.Property<string>("Cidade")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Estado")
                        .HasColumnType("longtext");

                    b.Property<int>("IDEmpresa")
                        .HasColumnType("int");

                    b.Property<string>("Logradouro")
                        .HasColumnType("longtext");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext");

                    b.Property<string>("Numero")
                        .HasColumnType("longtext");

                    b.Property<string>("Pais")
                        .HasColumnType("longtext");

                    b.Property<string>("Sexo")
                        .HasColumnType("longtext");

                    b.Property<string>("Sobrenome")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("APIGestao.Models.Empresa", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<Guid>("AccessToken")
                        .HasColumnType("char(36)");

                    b.Property<string>("CNPJ")
                        .HasColumnType("longtext");

                    b.Property<string>("Fantasia")
                        .HasColumnType("longtext");

                    b.Property<string>("RazaoSocial")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Empresas");
                });

            modelBuilder.Entity("APIGestao.Models.Pedido", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("IDCliente")
                        .HasColumnType("int");

                    b.Property<int>("IDEmpresa")
                        .HasColumnType("int");

                    b.Property<string>("MetodoPagamento")
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Pedidos");
                });

            modelBuilder.Entity("APIGestao.Models.PedidoItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("IDEmpresa")
                        .HasColumnType("int");

                    b.Property<int>("IDPedido")
                        .HasColumnType("int");

                    b.Property<int>("IDProduto")
                        .HasColumnType("int");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.Property<float>("ValorUnitario")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("PedidosItems");
                });

            modelBuilder.Entity("APIGestao.Models.Produto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<float>("Altura")
                        .HasColumnType("float");

                    b.Property<float>("Comprimento")
                        .HasColumnType("float");

                    b.Property<string>("Descricao")
                        .HasColumnType("longtext");

                    b.Property<int>("IDEmpresa")
                        .HasColumnType("int");

                    b.Property<float>("Largura")
                        .HasColumnType("float");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext");

                    b.Property<float>("Peso")
                        .HasColumnType("float");

                    b.Property<float>("PrecoCusto")
                        .HasColumnType("float");

                    b.Property<float>("PrecoVenda")
                        .HasColumnType("float");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Produtos");
                });

            modelBuilder.Entity("APIGestao.Models.Usuario", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("CPF")
                        .HasColumnType("longtext");

                    b.Property<string>("Cep")
                        .HasColumnType("longtext");

                    b.Property<string>("Cidade")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Estado")
                        .HasColumnType("longtext");

                    b.Property<int>("IDEmpresa")
                        .HasColumnType("int");

                    b.Property<string>("Login")
                        .HasColumnType("longtext");

                    b.Property<string>("Logradouro")
                        .HasColumnType("longtext");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext");

                    b.Property<string>("Numero")
                        .HasColumnType("longtext");

                    b.Property<string>("Pais")
                        .HasColumnType("longtext");

                    b.Property<string>("Senha")
                        .HasColumnType("longtext");

                    b.Property<string>("Sexo")
                        .HasColumnType("longtext");

                    b.Property<string>("Sobrenome")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
