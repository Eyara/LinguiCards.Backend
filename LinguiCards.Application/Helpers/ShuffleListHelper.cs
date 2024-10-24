namespace LinguiCards.Application.Helpers;

public static class ShuffleListHelper
{
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        var rng = new Random();
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }
}