using System.Collections.Concurrent;
using System.Diagnostics;

namespace IndependentWork12;

// Демонстрація проблем зі спільними змінними в PLINQ і способів їх виправити
public static class SideEffectsDemo
{
    private const int Runs = 5;

    public static void Run()
    {
        List<int> data = Operations.GenerateData(1_000_000);

        // Правильна відповідь, порахована звичайним LINQ в одному потоці
        int expected = data.Count(x => x % 2 == 0);
        Console.WriteLine($"Колекція: {data.Count:N0} чисел, правильна кількість парних: {expected:N0}");
        Console.WriteLine();

        ShowUnsafeCounter(data);
        ShowUnsafeList(data);
        ShowFixes(data, expected);
    }

    // Проблема 1: кілька потоків одночасно збільшують звичайну змінну
    private static void ShowUnsafeCounter(List<int> data)
    {
        Console.WriteLine("Проблема 1. Спільний лічильник count++ без синхронізації");

        for (int run = 1; run <= Runs; run++)
        {
            int count = 0;
            data.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    count++;
                }
            });

            Console.WriteLine($"  Запуск {run}: {count:N0}");
        }

        Console.WriteLine();
    }

    // Проблема 2: кілька потоків одночасно додають у звичайний List<T>
    private static void ShowUnsafeList(List<int> data)
    {
        Console.WriteLine("Проблема 2. Спільний List<int>.Add без синхронізації");

        for (int run = 1; run <= Runs; run++)
        {
            List<int> evens = new List<int>();
            try
            {
                data.AsParallel().Where(x => x % 2 == 0).ForAll(x => evens.Add(x));
                Console.WriteLine($"  Запуск {run}: у списку {evens.Count:N0} елементів");
            }
            catch (AggregateException exception)
            {
                // PLINQ збирає винятки з усіх потоків в один AggregateException
                Exception inner = exception.InnerExceptions[0];
                Console.WriteLine($"  Запуск {run}: виняток {inner.GetType().Name}");
            }
        }

        Console.WriteLine();
    }

    // Чотири правильні способи і час кожного
    private static void ShowFixes(List<int> data, int expected)
    {
        Console.WriteLine("Виправлення (кожен спосіб запущено 5 разів)");

        PrintFix("lock", expected, () =>
        {
            int count = 0;
            object locker = new object();
            data.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    // Лише один потік одночасно може бути всередині lock
                    lock (locker)
                    {
                        count++;
                    }
                }
            });
            return count;
        });

        PrintFix("Interlocked.Increment", expected, () =>
        {
            int count = 0;
            data.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    // Атомарне збільшення: читання, додавання і запис як одна неподільна дія
                    Interlocked.Increment(ref count);
                }
            });
            return count;
        });

        PrintFix("ConcurrentBag<int>", expected, () =>
        {
            ConcurrentBag<int> evens = new ConcurrentBag<int>();
            data.AsParallel().Where(x => x % 2 == 0).ForAll(x => evens.Add(x));
            return evens.Count;
        });

        PrintFix("без побічних ефектів: Count()", expected, () =>
        {
            // Кожен потік рахує свою частину, а PLINQ сам безпечно додає підсумки
            return data.AsParallel().Count(x => x % 2 == 0);
        });

        Console.WriteLine();
    }

    private static void PrintFix(string name, int expected, Func<int> countEvens)
    {
        List<int> results = new List<int>();
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int run = 0; run < Runs; run++)
        {
            results.Add(countEvens());
        }
        stopwatch.Stop();

        bool allCorrect = results.All(r => r == expected);
        double averageMs = stopwatch.Elapsed.TotalMilliseconds / Runs;
        Console.WriteLine($"  {name,-30} результати: {string.Join(", ", results.Select(r => r.ToString("N0")))} " +
                          $"| {(allCorrect ? "усі правильні" : "Є ПОМИЛКИ")} | {averageMs:F1} мс");
    }
}
