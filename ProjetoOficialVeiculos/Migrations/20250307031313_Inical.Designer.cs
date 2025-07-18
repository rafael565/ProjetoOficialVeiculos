﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjetoOficialVeiculos.Models;

#nullable disable

namespace ProjetoOficialVeiculos.Migrations
{
    [DbContext(typeof(Contexto))]
    [Migration("20250307031313_Inical")]
    partial class Inical
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProjetoOficialVeiculos.Models.Agendamento", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("DataAgendamento")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataConclusao")
                        .HasColumnType("datetime2");

                    b.Property<int>("MaterialID")
                        .HasColumnType("int");

                    b.Property<int?>("MotoristaID")
                        .HasColumnType("int");

                    b.Property<int>("OrdemFila")
                        .HasColumnType("int");

                    b.Property<string>("PesoCarregado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("caminhaoID")
                        .HasColumnType("int");

                    b.Property<int>("empresaID")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("MaterialID");

                    b.HasIndex("MotoristaID");

                    b.HasIndex("caminhaoID");

                    b.HasIndex("empresaID");

                    b.ToTable("Agendamentos");
                });

            modelBuilder.Entity("ProjetoOficialVeiculos.Models.Caminhao", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("placa")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.HasKey("id");

                    b.ToTable("Caminhoes");
                });

            modelBuilder.Entity("ProjetoOficialVeiculos.Models.Empresa", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("cnpj")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.Property<string>("contato")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.Property<string>("endereco")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.HasKey("id");

                    b.ToTable("Empresas");
                });

            modelBuilder.Entity("ProjetoOficialVeiculos.Models.Material", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.Property<string>("quantidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Materiais");
                });

            modelBuilder.Entity("ProjetoOficialVeiculos.Models.Motorista", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("nvarchar(35)");

                    b.HasKey("id");

                    b.ToTable("Motoristas");
                });

            modelBuilder.Entity("ProjetoOficialVeiculos.Models.Agendamento", b =>
                {
                    b.HasOne("ProjetoOficialVeiculos.Models.Material", "material")
                        .WithMany()
                        .HasForeignKey("MaterialID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProjetoOficialVeiculos.Models.Motorista", "motorista")
                        .WithMany()
                        .HasForeignKey("MotoristaID");

                    b.HasOne("ProjetoOficialVeiculos.Models.Caminhao", "caminhao")
                        .WithMany()
                        .HasForeignKey("caminhaoID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProjetoOficialVeiculos.Models.Empresa", "empresa")
                        .WithMany()
                        .HasForeignKey("empresaID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("caminhao");

                    b.Navigation("empresa");

                    b.Navigation("material");

                    b.Navigation("motorista");
                });
#pragma warning restore 612, 618
        }
    }
}
