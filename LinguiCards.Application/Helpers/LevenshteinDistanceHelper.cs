namespace LinguiCards.Application.Helpers;

public class LevenshteinDistanceHelper
{
    public static int CalculateDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
            return string.IsNullOrEmpty(target) ? 0 : target.Length;

        if (string.IsNullOrEmpty(target))
            return source.Length;

        var distance = new int[source.Length + 1, target.Length + 1];

        for (var i = 0; i <= source.Length; i++)
            distance[i, 0] = i;

        for (var j = 0; j <= target.Length; j++)
            distance[0, j] = j;

        for (var i = 1; i <= source.Length; i++)
        for (var j = 1; j <= target.Length; j++)
        {
            var cost = source[i - 1] == target[j - 1] ? 0 : 1;

            distance[i, j] = Math.Min(
                Math.Min(distance[i - 1, j] + 1,
                    distance[i, j - 1] + 1),
                distance[i - 1, j - 1] + cost);
        }

        return distance[source.Length, target.Length];
    }
}