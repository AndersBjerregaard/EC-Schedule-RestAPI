﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApi.Data;

namespace WebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220317111539_JsonWebTokens")]
    partial class JsonWebTokens
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("WebApi.Domain.RefreshToken", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("varchar(767)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("Invalidated")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("JwtId")
                        .HasColumnType("text");

                    b.Property<bool>("Used")
                        .HasColumnType("tinyint(1)");

                    b.Property<byte[]>("UserId")
                        .IsRequired()
                        .HasColumnType("varbinary(16)");

                    b.HasKey("Token");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("WebApi.Domain.UserDomainClass", b =>
                {
                    b.Property<byte[]>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varbinary(16)");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApi.Domain.RefreshToken", b =>
                {
                    b.HasOne("WebApi.Domain.UserDomainClass", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}