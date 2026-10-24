using System.Diagnostics;

namespace IndependentWork12;

// Порівняння звичайного LINQ і PLINQ на тій самій операції
public static class PerformanceExperiment
{
    // Кожен замір повторюється, і береться медіана, щоб випадкові затримки не спотворили результат
    private const int Repetitions = 3;

    // Опис однієї операції: назва і два варіанти запиту з однаковою логікою
    private record Experiment(
        string Name,
        Func<List<int>, List<double>> Linq,
        Func<List<int>, List<double>> Plinq);

    private static readonly Experiment[] Experiments =
    {
        new Experiment(
            "Дешева: x % 3 == 0, потім x * 2",
            data => data.Where(Operations.IsDivisibleByThree).Select(x => x * 2.0).ToList(),
            data => data.AsParallel().Where(Operations.IsDivisibleByThree).Select(x => x * 2.0).ToList()),

        new Experiment(
            "Середня: парні, потім sqrt, sin, log, cos",
            data => data.Where(x => x % 2 == 0).Select(Operations.MathTransform).ToList(),
            data => data.AsParallel().Where(x => x % 2 == 0).Select(Operations.MathTransform).ToList()),

        new Experiment(
            "Дорога: прості числа, потім сума цифр",
            data => data.Where(Operations.IsPrime).Select(x => (double)Operations.DigitSum(x)).ToList(),
            data => data.AsParallel().Where(Operations.IsPrime).Select(x => (double)Operations.DigitSum(x)).ToList())
    };

    /// <summary>
    /// Проводить усі експерименти для кожного розміру колекції.
    /// </summary>
    public static void Run(int[] sizes)
    {
        Console.WriteLine($"Логічних процесорів: {Environment.ProcessorCount}");
        Console.WriteLine($"Кожен замір повторено {Repetitions} рази, показано медіану");
        Console.WriteLine();

        WarmUp();

        foreach (int size in sizes)
        {
            List<int> data = Operations.GenerateData(size);
            Console.WriteLine($"Розмір колекції: {size:N0} елементів");

            foreach (Experiment experiment in Experiments)
            {
                double linqMs = MeasureMedian(() => experiment.Linq(data), out List<double> linqResult);
                double plinqMs = MeasureMedian(() => experiment.Plinq(data), out List<double> plinqResult);

                // PLINQ не зберігає порядок, тому результати порівнюються за кількістю і сумою.
                // Сума double залежить від порядку додавання, тому допускається крихітна відносна різниця
                double linqSum = linqResult.Sum();
                double plinqSum = plinqResult.Sum();
                bool sameResult = linqResult.Count == plinqResult.Count
                                  && Math.Abs(linqSum - plinqSum) <= Math.Abs(linqSum) * 1e-9;

                // Різницю менше 10% вважаємо похибкою вимірювання
                double ratio = linqMs / plinqMs;
                string verdict = ratio >= 1.1
                    ? $"PLINQ швидший у {ratio:F1} раза"
                    : ratio <= 1 / 1.1
                        ? $"PLINQ повільніший у {1 / ratio:F1} раза"
                        : "приблизно однаково";

                Console.WriteLine($"  {experiment.Name,-42} LINQ {linqMs,8:F1} мс | PLINQ {plinqMs,8:F1} мс | {verdict}");
                Console.WriteLine($"  {"",-42} знайдено {linqResult.Count:N0}, результати {(sameResult ? "збігаються" : "РІЗНІ")}");
            }

            Console.WriteLine();
        }
    }

    // Виконує запит кілька разів і повертає медіанний час у мілісекундах
    private static double MeasureMedian(Func<List<double>> query, out List<double> result)
    {
        List<double> times = new List<double>();
        result = new List<double>();

        for (int i = 0; i < Repetitions; i++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            result = query();
            stopwatch.Stop();
            times.Add(stopwatch.Elapsed.TotalMilliseconds);
        }

        times.Sort();
        return times[times.Count / 2];
    }

    // Прогрів на маленькій колекції: перший запуск повільніший через JIT-компіляцію і запуск потоків
    private static void WarmUp()
    {
        List<int> data = Operations.GenerateData(100_000);
        foreach (Experiment experiment in Experiments)
        {
            for (int i = 0; i < 3; i++)
            {
                experiment.Linq(data);
                experiment.Plinq(data);
            }
        }
    }
}
