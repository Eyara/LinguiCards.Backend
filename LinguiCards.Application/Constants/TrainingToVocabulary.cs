using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Constants;

public static class TrainingToVocabulary
{
    private static TrainingType[] _passiveTrainingTypes =
        { TrainingType.FromLearnLanguage, TrainingType.FromNativeLanguage };

    private static readonly TrainingType[] _activeTrainingTypes =
        { TrainingType.WritingFromLearnLanguage, TrainingType.WritingFromNativeLanguage };

    public static VocabularyType GetVocabularyType(TrainingType type)
    {
        return _activeTrainingTypes.Contains(type) ? VocabularyType.Active : VocabularyType.Passive;
    }
}