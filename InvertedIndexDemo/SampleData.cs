namespace InvertedIndexDemo;

// Тестові документи: п'ять базових з умови і генератор великих наборів
public static class SampleData
{
    // Невеликий словник для випадкових слів у згенерованих документах
    private static readonly string[] ExtraWords =
    {
        "database", "network", "security", "cloud", "testing", "framework",
        "compiler", "memory", "thread", "server", "client", "algorithm",
        "interface", "library", "performance", "design", "pattern", "mobile"
    };

    /// <summary>
    /// П'ять тестових документів з умови завдання.
    /// </summary>
    public static List<Document> GetBaseDocuments()
    {
        return new List<Document>
        {
            new Document
            {
                Id = 1,
                Title = "Introduction to C#",
                Content = "C# is a modern object-oriented programming language developed by Microsoft."
            },
            new Document
            {
                Id = 2,
                Title = "Java Basics",
                Content = "Java is a popular programming language known for its portability and performance."
            },
            new Document
            {
                Id = 3,
                Title = "Python Overview",
                Content = "Python is a high-level programming language with simple syntax."
            },
            new Document
            {
                Id = 4,
                Title = "C# Advanced Features",
                Content = "C# supports LINQ, async/await, and many modern programming paradigms."
            },
            new Document
            {
                Id = 5,
                Title = "Comparing Languages",
                Content = "C#, Java, and Python are among the most popular programming languages today."
            }
        };
    }

    /// <summary>
    /// Генерує count документів: текст одного з базових документів плюс кілька випадкових слів.
    /// Фіксоване зерно Random дає однакові дані при кожному запуску.
    /// </summary>
    public static List<Document> GenerateDocuments(int count, int seed = 42)
    {
        List<Document> baseDocuments = GetBaseDocuments();
        Random random = new Random(seed);
        List<Document> result = new List<Document>(count);

        for (int i = 0; i < count; i++)
        {
            Document source = baseDocuments[i % baseDocuments.Count];
            string[] extra = new string[5];
            for (int j = 0; j < extra.Length; j++)
            {
                extra[j] = ExtraWords[random.Next(ExtraWords.Length)];
            }

            result.Add(new Document
            {
                Id = i + 1,
                Title = $"{source.Title} #{i + 1}",
                Content = $"{source.Content} {string.Join(' ', extra)}."
            });
        }

        return result;
    }
}
