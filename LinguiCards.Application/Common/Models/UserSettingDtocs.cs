namespace LinguiCards.Application.Common.Models;

public class UserSettingDto
{
    public int Id { get; set; }

    public int ActiveTrainingSize { get; set; }
    public int PassiveTrainingSize { get; set; }

    public int UserId { get; set; }
}