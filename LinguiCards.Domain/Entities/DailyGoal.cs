using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinguiCards.Domain.Entities;

public class DailyGoal
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public int TargetXp { get; set; }

    [Required]
    public int GainedXp { get; set; } = 0;

    [Required]
    public int ByTranslation { get; set; } = 0;

    [Required]
    public int ByGrammar { get; set; } = 0;

    [NotMapped]
    public bool IsCompleted => GainedXp >= TargetXp;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
    
    public User User { get; set; }
}