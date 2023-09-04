namespace Darts.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<(int Index, T Value)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (index, item));
    }
}
