namespace InvertedIndexDemo;

/// <summary>
/// Інвертований індекс: для кожного слова зберігає множину документів, де воно є.
/// </summary>
public class InvertedIndex
{
    // Слово -> Множина ID документів
    private Dictionary<string, HashSet<int>> _index;

    // ID -> Документ (для швидкого отримання)
    private Dictionary<int, Document> _documents;

    public InvertedIndex()
    {
        _index = new Dictionary<string, HashSet<int>>();
        _documents = new Dictionary<int, Document>();
    }

    public int DocumentCount => _documents.Count;
    public int UniqueWords => _index.Keys.Count;

    /// <summary>
    /// Додає документ до індексу. Документ з таким самим Id додати вдруге не можна.
    /// </summary>
    public void AddDocument(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        // 1. Зберегти документ; TryAdd поверне false, якщо Id уже зайнятий
        if (!_documents.TryAdd(doc.Id, doc))
        {
            throw new ArgumentException($"Документ з Id {doc.Id} уже є в індексі", nameof(doc));
        }

        // 2. Розбити Content на слова і 3. для кожного слова додати doc.Id
        foreach (string word in TextProcessor.Normalize(doc.Content))
        {
            if (!_index.TryGetValue(word, out HashSet<int>? ids))
            {
                ids = new HashSet<int>();
                _index[word] = ids;
            }

            // Повторне слово в тому самому документі HashSet проігнорує
            ids.Add(doc.Id);
        }
    }

    /// <summary>
    /// Шукає документи, що містять слово.
    /// </summary>
    public List<Document> SearchSingleWord(string word)
    {
        // Слово з запиту нормалізується так само, як текст документів
        string? term = TextProcessor.Normalize(word).FirstOrDefault();
        if (term is null || !_index.TryGetValue(term, out HashSet<int>? ids))
        {
            return new List<Document>();
        }

        return ToDocuments(ids);
    }

    /// <summary>
    /// Шукає документи, що містять ВСІ слова (AND).
    /// </summary>
    public List<Document> SearchMultipleWords(params string[] words)
    {
        List<string> terms = NormalizeQuery(words);
        if (terms.Count == 0)
        {
            return new List<Document>();
        }

        HashSet<int>? result = null;
        foreach (string term in terms)
        {
            // Якщо хоч одного слова немає в індексі, перетин гарантовано порожній
            if (!_index.TryGetValue(term, out HashSet<int>? ids))
            {
                return new List<Document>();
            }

            // Копія потрібна, щоб IntersectWith не зіпсував множину в самому індексі
            if (result is null)
            {
                result = new HashSet<int>(ids);
            }
            else
            {
                result.IntersectWith(ids);
            }
        }

        return ToDocuments(result!);
    }

    /// <summary>
    /// Шукає документи, що містять БУДЬ-ЯКЕ слово (OR).
    /// </summary>
    public List<Document> SearchAnyWord(params string[] words)
    {
        HashSet<int> result = new HashSet<int>();
        foreach (string term in NormalizeQuery(words))
        {
            if (_index.TryGetValue(term, out HashSet<int>? ids))
            {
                result.UnionWith(ids);
            }
        }

        return ToDocuments(result);
    }

    /// <summary>
    /// Перетворює слова запиту за правилами індексу; стоп-слова й повтори відкидаються.
    /// </summary>
    private static List<string> NormalizeQuery(string[] words)
    {
        return TextProcessor.Normalize(string.Join(' ', words)).Distinct().ToList();
    }

    /// <summary>
    /// Отримує документи за Id; HashSet не зберігає порядок, тому результат сортується.
    /// </summary>
    private List<Document> ToDocuments(HashSet<int> ids)
    {
        return ids.OrderBy(id => id).Select(id => _documents[id]).ToList();
    }
}
