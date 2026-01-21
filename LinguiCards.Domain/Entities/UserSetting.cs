namespace LinguiCards.Domain.Entities;

public class UserSetting
{
    public int Id { get; set; }

    public int ActiveTrainingSize { get; set; }
    public int PassiveTrainingSize { get; set; }
    public int? DailyGoalXp { get; set; }
    public int? DailyGoalByTranslation { get; set; }
    public int? DailyGoalByGrammar { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}