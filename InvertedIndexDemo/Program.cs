using System.Text;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo;

class Program
{
    // Скільки символів вмісту показувати в результатах
    private const int SnippetLength = 100;

    static void Main(string[] args)
    {
        // Коректне відображення українських літер
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        InvertedIndex index = new InvertedIndex();

        // Завантаження документів
        LoadDocuments(index);
        Console.WriteLine($"Loaded {index.DocumentCount} documents");
        Console.WriteLine($"Index contains {index.UniqueWords} unique words");
        Console.WriteLine();
        PrintHelp();

        while (true)
        {
            Console.Write("Enter search query (or 'exit'): ");
            string? query = Console.ReadLine();

            // null означає кінець вводу, наприклад якщо команди подаються з файлу
            if (query is null)
            {
                break;
            }

            // PowerShell при передачі тексту через | додає на початок невидимий символ BOM
            query = query.Trim().TrimStart((char)0xFEFF);
            if (query.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            // Якщо запити подаються з файлу, консоль їх не показує, тому виводимо самі
            if (Console.IsInputRedirected)
            {
                Console.WriteLine(query);
            }

            if (query.Length == 0)
            {
                continue;
            }

            if (query.Equals("perf", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                // "python" є у 40% документів, "rust" немає в жодному
                PerformanceTest.RunAll("python", "rust");
                Console.WriteLine();
                continue;
            }

            // Запит, що починається з "or ", шукає будь-яке зі слів, інакше всі слова разом
            bool isOrQuery = query.StartsWith("or ", StringComparison.OrdinalIgnoreCase);
            string[] words = (isOrQuery ? query.Substring(3) : query)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            List<Document> results = isOrQuery
                ? index.SearchAnyWord(words)
                : index.SearchMultipleWords(words);

            Console.WriteLine($"\n--- Search Results ({(isOrQuery ? "OR" : "AND")}) ---");
            PrintResults(results, words);
            Console.WriteLine($"\nFound {results.Count} documents\n");
        }
    }

    // Додає в індекс п'ять тестових документів
    static void LoadDocuments(InvertedIndex index)
    {
        foreach (Document doc in SampleData.GetBaseDocuments())
        {
            index.AddDocument(doc);
        }
    }

    static void PrintHelp()
    {
        Console.WriteLine("Приклади запитів (документи англійською, тому й слова англійською):");
        Console.WriteLine("  programming language   документи, де є ВСІ слова (AND)");
        Console.WriteLine("  or python java         документи, де є БУДЬ-ЯКЕ зі слів (OR), запит починається з or");
        Console.WriteLine("  perf                   вимірювання продуктивності для 10/100/1000/10000 документів");
        Console.WriteLine("  exit                   вихід");
        Console.WriteLine();
    }

    // Виводить знайдені документи з початком вмісту
    static void PrintResults(List<Document> results, string[] queryWords)
    {
        HashSet<string> terms = new HashSet<string>(TextProcessor.Normalize(string.Join(' ', queryWords)));

        foreach (Document doc in results)
        {
            Console.WriteLine(doc);

            // Крапка в кінці речення прибирається, щоб після неї не вийшло чотири крапки
            string snippet = doc.Content.Substring(0, Math.Min(SnippetLength, doc.Content.Length)).TrimEnd('.');
            Console.Write(" ");
            WriteHighlighted(snippet, terms);
            Console.WriteLine("...");
        }
    }

    // Завдання з зірочкою: слова із запиту виводяться жовтим кольором
    static void WriteHighlighted(string text, HashSet<string> terms)
    {
        int position = 0;
        foreach (Match match in TextProcessor.Pattern.Matches(text))
        {
            // Пунктуація і пробіли між словами виводяться як є
            Console.Write(text.Substring(position, match.Index - position));

            if (terms.Contains(match.Value.ToLowerInvariant()))
            {
                ConsoleColor previous = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(match.Value);
                Console.ForegroundColor = previous;
            }
            else
            {
                Console.Write(match.Value);
            }

            position = match.Index + match.Length;
        }

        Console.Write(text.Substring(position));
    }
}
