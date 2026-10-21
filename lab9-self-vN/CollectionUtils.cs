namespace lab9_self_vN;

// Узагальнені утиліти для колекцій; поведінку кожного методу задає переданий делегат
public static class CollectionUtils
{
    // Перетворює кожен елемент за допомогою mapper (аналог LINQ Select)
    public static IEnumerable<TResult> Map<TSource, TResult>(
        IEnumerable<TSource> collection,
        Func<TSource, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(mapper);

        List<TResult> result = new List<TResult>();
        foreach (TSource item in collection)
        {
            result.Add(mapper(item));
        }

        return result;
    }

    // Повертає елемент з найбільшим ключем; при однакових ключах залишається перший
    public static T MaxBy<T, TKey>(
        IEnumerable<T> collection,
        Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(keySelector);

        T? maxItem = default;
        TKey? maxKey = default;
        bool first = true;

        foreach (T item in collection)
        {
            TKey key = keySelector(item);
            if (first || key.CompareTo(maxKey) > 0)
            {
                maxKey = key;
                maxItem = item;
                first = false;
            }
        }

        // У порожній колекції максимуму немає, тому краще повідомити про помилку, ніж повернути default
        if (first)
        {
            throw new InvalidOperationException("Колекція порожня, максимальний елемент не існує");
        }

        return maxItem!;
    }

    // Повертає елементи, для яких predicate повертає true (аналог LINQ Where)
    public static IEnumerable<T> Filter<T>(
        IEnumerable<T> collection,
        Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(predicate);

        List<T> result = new List<T>();
        foreach (T item in collection)
        {
            if (predicate(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    // Виконує action для кожного елемента
    public static void ForEach<T>(
        IEnumerable<T> collection,
        Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(action);

        foreach (T item in collection)
        {
            action(item);
        }
    }
}
