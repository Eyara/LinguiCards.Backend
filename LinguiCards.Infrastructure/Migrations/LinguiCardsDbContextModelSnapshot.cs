﻿// <auto-generated />
using System;
using LinguiCards.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    [DbContext(typeof(LinguiCardsDbContext))]
    partial class LinguiCardsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LinguiCards.Domain.Entities.CribDescription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CribId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Header")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("CribDescriptions");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.DefaultCrib", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("LanguageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.ToTable("DefaultCribs");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("LanguageDictionaryId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LanguageDictionaryId");

                    b.HasIndex("UserId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.LanguageDictionary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("LanguageDictionaries");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.Word", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("LanguageId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("LearnedPercent")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TranslatedName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.WordChangeHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ChangedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsCorrectAnswer")
                        .HasColumnType("boolean");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordId");

                    b.ToTable("WordChangeHistories");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.DefaultCrib", b =>
                {
                    b.HasOne("LinguiCards.Domain.Entities.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.Language", b =>
                {
                    b.HasOne("LinguiCards.Domain.Entities.LanguageDictionary", "LanguageDictionary")
                        .WithMany("Languages")
                        .HasForeignKey("LanguageDictionaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LinguiCards.Domain.Entities.User", "User")
                        .WithMany("Languages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LanguageDictionary");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.Word", b =>
                {
                    b.HasOne("LinguiCards.Domain.Entities.Language", "Language")
                        .WithMany("Words")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.WordChangeHistory", b =>
                {
                    b.HasOne("LinguiCards.Domain.Entities.Word", "Word")
                        .WithMany("Histories")
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Word");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.Language", b =>
                {
                    b.Navigation("Words");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.LanguageDictionary", b =>
                {
                    b.Navigation("Languages");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.User", b =>
                {
                    b.Navigation("Languages");
                });

            modelBuilder.Entity("LinguiCards.Domain.Entities.Word", b =>
                {
                    b.Navigation("Histories");
                });
#pragma warning restore 612, 618
        }
    }
}
