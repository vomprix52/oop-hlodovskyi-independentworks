using System.Diagnostics;

namespace InvertedIndexDemo;

// Порівняння лінійного пошуку і пошуку через інвертований індекс
public static class PerformanceTest
{
    // Кількість повторів пошуку через індекс; один пошук надто швидкий, щоб виміряти його окремо
    private const int IndexRepetitions = 10000;

    /// <summary>
    /// Порівнює два способи пошуку слова в документах і виводить один рядок таблиці.
    /// </summary>
    public static void CompareSearchMethods(List<Document> documents, string searchWord)
    {
        // Лінійний пошук повторюється так, щоб загалом оброблялося близько 50 000 документів
        int linearRepetitions = Math.Max(3, 50000 / documents.Count);

        // 1. Лінійний пошук: кожен документ нормалізується під час кожного пошуку
        string normalizedWord = TextProcessor.Normalize(searchWord).First();
        LinearSearch(documents, normalizedWord);

        Stopwatch sw = Stopwatch.StartNew();
        List<Document> linearResults = new List<Document>();
        for (int i = 0; i < linearRepetitions; i++)
        {
            linearResults = LinearSearch(documents, normalizedWord);
        }
        sw.Stop();
        double linearMicroseconds = sw.Elapsed.TotalMicroseconds / linearRepetitions;

        // 2. Побудова індексу вимірюється окремо, бо це разова підготовча робота
        sw.Restart();
        InvertedIndex index = new InvertedIndex();
        foreach (Document doc in documents)
        {
            index.AddDocument(doc);
        }
        sw.Stop();
        double buildMicroseconds = sw.Elapsed.TotalMicroseconds;

        // 3. Пошук через індекс
        index.SearchSingleWord(searchWord);
        sw.Restart();
        List<Document> indexResults = new List<Document>();
        for (int i = 0; i < IndexRepetitions; i++)
        {
            indexResults = index.SearchSingleWord(searchWord);
        }
        sw.Stop();
        double indexMicroseconds = sw.Elapsed.TotalMicroseconds / IndexRepetitions;

        // Перевірка коректності: обидва способи мають знайти ті самі документи
        bool sameResults = linearResults.Select(d => d.Id).SequenceEqual(indexResults.Select(d => d.Id));

        // Скільки пошуків треба, щоб окупити побудову індексу
        double savedPerSearch = linearMicroseconds - indexMicroseconds;
        string breakEven = savedPerSearch > 0
            ? Math.Ceiling(buildMicroseconds / savedPerSearch).ToString("N0")
            : "не окупається";

        Console.WriteLine(
            $"{documents.Count,10:N0} | {linearMicroseconds,12:N1} | {indexMicroseconds,10:N2} | " +
            $"{linearMicroseconds / indexMicroseconds,10:N0}x | {buildMicroseconds,12:N0} | {breakEven,14} | " +
            $"{linearResults.Count,6:N0} / {indexResults.Count,-6:N0} {(sameResults ? "збіг" : "РІЗНІ")}");
    }

    /// <summary>
    /// Проводить вимірювання для 10, 100, 1000 і 10000 документів для кожного слова.
    /// </summary>
    public static void RunAll(params string[] searchWords)
    {
        Console.WriteLine("Вимірювання продуктивності");
        Console.WriteLine("Час одного пошуку і побудови індексу в мікросекундах (мкс)");
        WarmUp(searchWords[0]);

        foreach (string searchWord in searchWords)
        {
            Console.WriteLine();
            Console.WriteLine($"Слово для пошуку: \"{searchWord}\"");
            Console.WriteLine($"{"Документів",10} | {"Лінійний",12} | {"Індекс",10} | {"Швидше",11} | {"Побудова",12} | {"Окупність",14} | Знайдено (лін. / індекс)");

            foreach (int count in new[] { 10, 100, 1000, 10000 })
            {
                CompareSearchMethods(SampleData.GenerateDocuments(count), searchWord);
            }
        }

        Console.WriteLine();
        Console.WriteLine("Окупність: після скількох пошуків побудова індексу виграє в лінійного пошуку");
    }

    // Прогрів: .NET спочатку запускає неоптимізований код і оптимізує його лише після
    // кількох десятків викликів, тому всі операції спершу виконуються вхолосту
    private static void WarmUp(string searchWord)
    {
        List<Document> documents = SampleData.GenerateDocuments(100);
        string normalizedWord = TextProcessor.Normalize(searchWord).First();

        for (int round = 0; round < 5; round++)
        {
            for (int i = 0; i < 50; i++)
            {
                LinearSearch(documents, normalizedWord);
            }

            InvertedIndex index = new InvertedIndex();
            foreach (Document doc in documents)
            {
                index.AddDocument(doc);
            }

            for (int i = 0; i < 1000; i++)
            {
                index.SearchSingleWord(searchWord);
            }

            // Оптимізація відбувається у фоновому потоці, тому даємо їй трохи часу
            Thread.Sleep(100);
        }
    }

    // Лінійний пошук без індексу з тією самою нормалізацією тексту
    private static List<Document> LinearSearch(List<Document> documents, string normalizedWord)
    {
        return documents
            .Where(d => TextProcessor.Normalize(d.Content).Contains(normalizedWord))
            .ToList();
    }
}
