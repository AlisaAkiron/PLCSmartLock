﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PLCHost.Database;

#nullable disable

namespace PLCHost.Migrations
{
    [DbContext(typeof(PlcHostDbContext))]
    partial class PlcHostDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("PLCHost.Models.Entities.Persistent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("Key")
                        .HasColumnType("TEXT")
                        .HasColumnName("key");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("persistent");
                });
#pragma warning restore 612, 618
        }
    }
}
