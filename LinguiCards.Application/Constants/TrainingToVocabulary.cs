using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Constants;

public static class TrainingToVocabulary
{
    private static TrainingType[] _passiveTrainingTypes =
        new[] { TrainingType.FromLearnLanguage, TrainingType.FromNativeLanguage };
    
    private static TrainingType[] _activeTrainingTypes =
        new[] { TrainingType.WritingFromLearnLanguage, TrainingType.WritingFromNativeLanguage };

    public static VocabularyType GetVocabularyType(TrainingType type)
    {
        return _activeTrainingTypes.Contains(type) ? VocabularyType.Active  : VocabularyType.Passive;
    }
}