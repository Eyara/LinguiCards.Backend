using LinguiCards.Application.Common.Models;

namespace LinguiCards.Application.Constants;

public static class TrainingTypesHelper
{
    private static readonly TrainingType[] _learnTrainingTypes =
        { TrainingType.FromLearnLanguage, TrainingType.WritingFromLearnLanguage };

    private static TrainingType[] _nativeTrainingTypes =
        { TrainingType.FromNativeLanguage, TrainingType.WritingFromNativeLanguage };

    public static bool IsFromLearnTraining(TrainingType type)
    {
        return _learnTrainingTypes.Contains(type);
    }
}