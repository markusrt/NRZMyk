﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NRZMyk.Services.Data;

namespace NRZMyk.Services.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200919204843_AntimicrobialSensitivityTest_Initial")]
    partial class AntimicrobialSensitivityTest_Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NRZMyk.Services.Data.Entities.AntimicrobialSensitivityTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AntifungalAgent")
                        .HasColumnType("int");

                    b.Property<int>("ClinicalBreakpointId")
                        .HasColumnType("int");

                    b.Property<float>("MinimumInhibitoryConcentration")
                        .HasColumnType("real");

                    b.Property<int>("Resistance")
                        .HasColumnType("int");

                    b.Property<int?>("SentinelEntryId")
                        .HasColumnType("int");

                    b.Property<int>("TestingMethod")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClinicalBreakpointId");

                    b.HasIndex("SentinelEntryId");

                    b.ToTable("AntimicrobialSensitivityTest");
                });

            modelBuilder.Entity("NRZMyk.Services.Data.Entities.ClinicalBreakpoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AntifungalAgent")
                        .HasColumnType("int");

                    b.Property<string>("AntifungalAgentDetails")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<float?>("MicBreakpointResistent")
                        .HasColumnType("real");

                    b.Property<float?>("MicBreakpointSusceptible")
                        .HasColumnType("real");

                    b.Property<int>("Species")
                        .HasColumnType("int");

                    b.Property<int>("Standard")
                        .HasColumnType("int");

                    b.Property<float?>("TechnicalUncertainty")
                        .HasColumnType("real");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("datetime2");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Standard", "Version", "Species", "AntifungalAgentDetails")
                        .IsUnique();

                    b.ToTable("ClinicalBreakpoints");
                });

            modelBuilder.Entity("NRZMyk.Services.Data.Entities.SentinelEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AgeGroup")
                        .HasColumnType("int");

                    b.Property<int>("HospitalDepartment")
                        .HasColumnType("int");

                    b.Property<int>("HospitalDepartmentType")
                        .HasColumnType("int");

                    b.Property<int>("IdentifiedSpecies")
                        .HasColumnType("int");

                    b.Property<int>("Material")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SamplingDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SenderLaboratoryNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SpeciesIdentificationMethod")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SentinelEntries");
                });

            modelBuilder.Entity("NRZMyk.Services.Data.Entities.AntimicrobialSensitivityTest", b =>
                {
                    b.HasOne("NRZMyk.Services.Data.Entities.ClinicalBreakpoint", "ClinicalBreakpoint")
                        .WithMany("AntimicrobialSensitivityTests")
                        .HasForeignKey("ClinicalBreakpointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NRZMyk.Services.Data.Entities.SentinelEntry", "SentinelEntry")
                        .WithMany("AntimicrobialSensitivityTests")
                        .HasForeignKey("SentinelEntryId");
                });
#pragma warning restore 612, 618
        }
    }
}
