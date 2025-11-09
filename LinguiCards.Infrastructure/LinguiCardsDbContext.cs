using LinguiCards.Domain.Entities;
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
    public DbSet<LanguageDictionary> LanguageDictionaries { get; set; }
    public DbSet<WordChangeHistory> WordChangeHistories { get; set; }
    public DbSet<DefaultCrib> DefaultCribs { get; set; }
    public DbSet<CribDescription> CribDescriptions { get; set; }
    public DbSet<UserSetting> UserSettings { get; set; }
    public DbSet<TranslationEvaluationHistory> TranslationEvaluationHistories { get; set; }
    public DbSet<DailyGoal> DailyGoals { get; set; }
    public DbSet<GrammarTaskHistory> GrammarTaskHistories { get; set; }
    public DbSet<GrammarTaskTypeDictionary> GrammarTaskTypeDictionary { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<User>()
            .Property(u => u.Level)
            .HasDefaultValue(1);

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

        modelBuilder.Entity<LanguageDictionary>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<LanguageDictionary>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Language>()
            .HasOne(l => l.LanguageDictionary)
            .WithMany(l => l.Languages)
            .HasForeignKey(l => l.LanguageDictionaryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WordChangeHistory>()
            .HasKey(w => w.Id);

        modelBuilder.Entity<WordChangeHistory>()
            .Property(w => w.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<WordChangeHistory>()
            .HasOne(w => w.Word)
            .WithMany(l => l.Histories)
            .HasForeignKey(w => w.WordId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DefaultCrib>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<DefaultCrib>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<CribDescription>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<CribDescription>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserSetting>()
            .HasKey(us => us.Id);

        modelBuilder.Entity<UserSetting>()
            .Property(us => us.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserSetting>()
            .HasOne(us => us.User)
            .WithOne(u => u.UserSetting)
            .HasForeignKey<UserSetting>(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TranslationEvaluationHistory>()
            .HasKey(us => us.Id);

        modelBuilder.Entity<TranslationEvaluationHistory>()
            .Property(l => l.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<TranslationEvaluationHistory>()
            .HasOne(t => t.User)
            .WithMany(u => u.TranslationEvaluationHistories)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<DailyGoal>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();

            entity.Property(e => e.GainedXp)
                .HasDefaultValue(0);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
            
            modelBuilder.Entity<DailyGoal>()
                .HasOne(e => e.User)
                .WithMany(u => u.DailyGoals)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<GrammarTaskHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<GrammarTaskHistory>()
                .HasOne(e => e.User)
                .WithMany(u => u.GrammarTaskHistories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<GrammarTaskTypeDictionary>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<GrammarTaskTypeDictionary>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
    }
}