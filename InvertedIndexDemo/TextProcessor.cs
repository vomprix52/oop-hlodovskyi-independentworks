using System.Text.RegularExpressions;

namespace InvertedIndexDemo;

// Обробка тексту; ті самі правила використовуються і для індексу, і для лінійного пошуку
public static class TextProcessor
{
    // Слово починається з літери чи цифри; символи # і + всередині дозволені, щоб "c#" і "c++" не розпадалися
    private static readonly Regex WordPattern = new Regex(@"\w[\w#+]*", RegexOptions.Compiled);

    // Службові слова, які трапляються майже скрізь і нічого не дають для пошуку
    private static readonly HashSet<string> StopWords = new HashSet<string>
    {
        "a", "an", "the", "is", "are", "was", "were",
        "in", "on", "at", "to", "for", "of", "and", "or"
    };

    /// <summary>
    /// Розбиває текст на слова, видаляє пунктуацію, переводить у нижній регістр.
    /// Приклад: "Hello, world!" -> ["hello", "world"]
    /// </summary>
    public static IEnumerable<string> Tokenize(string text)
    {
        foreach (Match match in WordPattern.Matches(text.ToLowerInvariant()))
        {
            yield return match.Value;
        }
    }

    /// <summary>
    /// Видаляє стоп-слова (a, an, the, is, are тощо).
    /// </summary>
    public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
    {
        return words.Where(w => !StopWords.Contains(w));
    }

    /// <summary>
    /// Повна нормалізація: токенізація плюс видалення стоп-слів.
    /// Саме її викликають усі способи пошуку, щоб порівняння було чесним.
    /// </summary>
    public static IEnumerable<string> Normalize(string text)
    {
        return RemoveStopWords(Tokenize(text));
    }

    /// <summary>
    /// Шаблон слова для тих, кому потрібно розбити текст без втрати пунктуації (підсвітка).
    /// </summary>
    public static Regex Pattern => WordPattern;
}
