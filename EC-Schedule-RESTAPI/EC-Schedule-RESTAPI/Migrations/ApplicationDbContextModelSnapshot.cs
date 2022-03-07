﻿// <auto-generated />
using System;
using EC_Schedule_RESTAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EC_Schedule_RESTAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("EC_Schedule_RESTAPI.Domain.DomainTestObject", b =>
                {
                    b.Property<byte[]>("DomainTestObjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varbinary(16)");

                    b.Property<string>("Info")
                        .HasColumnType("text");

                    b.HasKey("DomainTestObjectId");

                    b.ToTable("TestObjects");
                });
#pragma warning restore 612, 618
        }
    }
}