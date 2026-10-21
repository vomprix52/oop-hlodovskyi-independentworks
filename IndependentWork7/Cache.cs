namespace IndependentWork7;

// Узагальнений кеш з обмеженою ємністю і політикою FIFO:
// коли місця немає, видаляється елемент, доданий найраніше
public class Cache<TKey, TValue> where TKey : notnull
{
    // Приватні поля
    private int _capacity;
    private Dictionary<TKey, TValue> _cache;
    private Queue<TKey> _keysOrder;

    // Максимальна кількість елементів
    public int Capacity
    {
        get { return _capacity; }
    }

    // Поточна кількість елементів
    public int Count
    {
        get { return _cache.Count; }
    }

    // Конструктор, ємність має бути більшою за нуль
    public Cache(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Ємність кешу має бути більшою за нуль");
        }

        _capacity = capacity;
        _cache = new Dictionary<TKey, TValue>(capacity);
        _keysOrder = new Queue<TKey>(capacity);
    }

    // Додає елемент; якщо кеш повний, спочатку видаляє найстаріший
    public void Add(TKey key, TValue value)
    {
        // Існуючий ключ лише оновлює значення, його місце в черзі не змінюється
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            return;
        }

        if (_cache.Count >= _capacity)
        {
            TKey oldestKey = _keysOrder.Dequeue();
            _cache.Remove(oldestKey);
        }

        _cache.Add(key, value);
        _keysOrder.Enqueue(key);
    }

    // Повертає значення за ключем або викидає KeyNotFoundException
    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші");
    }

    // Перевіряє, чи є ключ у кеші
    public bool ContainsKey(TKey key)
    {
        return _cache.ContainsKey(key);
    }
}
