namespace LinguiCards.Application.Helpers;

public record SrsResult(double EaseFactor, int IntervalDays, int RepetitionCount);

/// <summary>
/// SM-2 spaced repetition algorithm.
/// Quality scale: 0 (total blank) to 5 (perfect recall with no hesitation).
/// </summary>
public static class SrsCalculator
{
    public const double DefaultEaseFactor = 2.5;
    public const double MinEaseFactor = 1.3;
    public const int MaxQuality = 5;
    public const int PassThreshold = 3;

    public static SrsResult Calculate(double easeFactor, int intervalDays, int repetitionCount, int quality)
    {
        quality = Math.Clamp(quality, 0, MaxQuality);

        if (quality >= PassThreshold)
        {
            var newInterval = repetitionCount switch
            {
                0 => 1,
                1 => 6,
                _ => (int)Math.Ceiling(intervalDays * easeFactor)
            };

            var newEf = easeFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02));
            newEf = Math.Max(MinEaseFactor, newEf);

            return new SrsResult(newEf, newInterval, repetitionCount + 1);
        }

        return new SrsResult(Math.Max(MinEaseFactor, easeFactor - 0.2), 1, 0);
    }

    /// <summary>
    /// Maps a training answer to SM-2 quality (0-5).
    /// </summary>
    public static int MapQuality(bool wasSuccessful, int hintCount, int answerLength)
    {
        if (!wasSuccessful)
            return 1;

        if (hintCount == 0)
            return 5;

        var hintRatio = answerLength > 0 ? (double)hintCount / answerLength : 1.0;
        return hintRatio switch
        {
            <= 0.25 => 4,
            <= 0.5 => 3,
            _ => 3
        };
    }

    /// <summary>
    /// Derives a 0-100 mastery percent from SRS state for display purposes.
    /// </summary>
    public static double DeriveLearnedPercent(int repetitionCount, int intervalDays)
    {
        var percent = repetitionCount * 15.0 + intervalDays * 2.0;
        return Math.Min(100.0, Math.Max(0.0, Math.Round(percent, 2)));
    }
}
