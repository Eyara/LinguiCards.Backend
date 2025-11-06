namespace LinguiCards.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }

    public int Level { get; set; }
    public double XP { get; set; }

    public int? UserSettingId { get; set; }
    public UserSetting UserSetting { get; set; }
    public ICollection<TranslationEvaluationHistory> TranslationEvaluationHistories { get; set; } 
    public ICollection<Language> Languages { get; set; }
    public ICollection<DailyGoal> DailyGoals { get; set; }
    public ICollection<GrammarTaskHistory> GrammarTaskHistories { get; set; }
}