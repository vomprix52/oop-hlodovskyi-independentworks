using System.Text;

namespace IndependentWork8;

// Сценарій 3: інвертований індекс для пошуку (Dictionary + HashSet)
public class SearchIndex
{
    // Слово -> множина номерів документів, де воно зустрічається
    private Dictionary<string, HashSet<int>> _invertedIndex;

    // Кількість різних слів в індексі
    public int WordCount
    {
        get { return _invertedIndex.Count; }
    }

    // Конструктор; пошук не залежить від регістру
    public SearchIndex()
    {
        _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
    }

    // Розбиває текст на слова і додає номер документа до кожного слова
    public void AddDocument(int docId, string content)
    {
        foreach (string word in SplitIntoWords(content))
        {
            if (!_invertedIndex.TryGetValue(word, out HashSet<int>? documents))
            {
                documents = new HashSet<int>();
                _invertedIndex[word] = documents;
            }

            // Якщо слово повторюється в тому самому документі, HashSet не додасть номер вдруге
            documents.Add(docId);
        }
    }

    // Документи, де є слово; копія, щоб не можна було зіпсувати індекс
    public HashSet<int> Search(string word)
    {
        if (_invertedIndex.TryGetValue(word, out HashSet<int>? documents))
        {
            return new HashSet<int>(documents);
        }

        return new HashSet<int>();
    }

    // Документи, що містять усі слова одночасно (перетин множин)
    public HashSet<int> SearchMultiple(params string[] words)
    {
        if (words.Length == 0)
        {
            return new HashSet<int>();
        }

        HashSet<int> result = Search(words[0]);
        for (int i = 1; i < words.Length; i++)
        {
            result.IntersectWith(Search(words[i]));
        }

        return result;
    }

    // Слова відокремлюються всім, що не є літерою чи цифрою
    private static IEnumerable<string> SplitIntoWords(string content)
    {
        List<string> words = new List<string>();
        StringBuilder current = new StringBuilder();

        foreach (char symbol in content)
        {
            if (char.IsLetterOrDigit(symbol) || symbol == '\'')
            {
                current.Append(symbol);
            }
            else if (current.Length > 0)
            {
                words.Add(current.ToString());
                current.Clear();
            }
        }

        if (current.Length > 0)
        {
            words.Add(current.ToString());
        }

        return words;
    }
}
