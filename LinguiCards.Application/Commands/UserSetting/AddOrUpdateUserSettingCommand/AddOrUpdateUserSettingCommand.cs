using MediatR;

namespace LinguiCards.Application.Commands.UserSetting.AddOrUpdateUserSettingCommand;

public record AddOrUpdateUserSettingCommand
    (string Username, int ActiveTrainingSize, int PassiveTrainingSize, int? DailyGoalXp, int? DailyGoalByTranslation, int? DailyGoalByGrammar) : IRequest;