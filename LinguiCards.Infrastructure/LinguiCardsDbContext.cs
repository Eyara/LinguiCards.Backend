﻿using LinguiCards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguiCards.Infrastructure;

public class LinguiCardsDbContext : DbContext
{
    public LinguiCardsDbContext(DbContextOptions<LinguiCardsDbContext> options) : base(options)
    {
    }

    public DbSet<User?> Users { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Word> Words { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Language>()
            .HasKey(l => l.Id);

        modelBuilder.Entity<Language>()
            .Property(l => l.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Language>()
            .HasOne(l => l.User)
            .WithMany(u => u.Languages)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Word>()
            .HasKey(w => w.Id);

        modelBuilder.Entity<Word>()
            .Property(w => w.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Word>()
            .HasOne(w => w.Language)
            .WithMany(l => l.Words)
            .HasForeignKey(w => w.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}