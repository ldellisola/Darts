namespace darts.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (index, item));
    }
}
