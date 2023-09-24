namespace Darts.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<(int Index, T Value)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (index, item));
    }

    public static IEnumerable<T?> WithoutNullTrail<T>(this IEnumerable<T?> source)
    {
        var arr = source.ToArray();
        int lastIndex = arr.Length - 1;

        // Find the last non-null index
        while (lastIndex >= 0 && arr[lastIndex] is null)
        {
            lastIndex--;
        }

        // Create a new array up to the last non-null index
        T[] newArr = new T[lastIndex + 1];
        Array.Copy(arr, newArr, lastIndex + 1);

        return newArr;
    }
}
