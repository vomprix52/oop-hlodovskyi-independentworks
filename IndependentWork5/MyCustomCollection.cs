using System.Collections;

namespace IndependentWork5;

// Власна колекція рядків, яку можна перебирати через foreach
public class MyCustomCollection : IEnumerable<string>
{
    // Внутрішній список, схований від зовнішнього коду
    private List<string> _items;

    public int Count
    {
        get { return _items.Count; }
    }

    // Конструктор
    public MyCustomCollection()
    {
        _items = new List<string>();
    }

    // Додає рядок, порожні рядки не приймаються
    public void Add(string item)
    {
        if (!string.IsNullOrWhiteSpace(item))
        {
            _items.Add(item);
        }
    }

    // Віддає елементи по одному, саме цей метод викликає foreach
    public IEnumerator<string> GetEnumerator()
    {
        foreach (string item in _items)
        {
            yield return item;
        }
    }

    // Неузагальнена версія, яку вимагає старий інтерфейс IEnumerable
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
