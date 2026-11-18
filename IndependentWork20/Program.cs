using System.Globalization;
using System.Text;

namespace IndependentWork20;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер і формату чисел
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        // Обрані параметри: 3 повтори, початкова затримка 300 мс, jitter ±25%
        RetryPolicy policy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(300), jitterFactor: 0.25);
        Console.WriteLine($"RetryPolicy: повторів {policy.MaxRetries}, початкова затримка {policy.InitialDelay.TotalMilliseconds} мс, jitter ±{policy.JitterFactor:P0}");
        Console.WriteLine();

        CheckDelayFormula(policy);
        DemoSuccessAfterFailures(policy);
        DemoFinalFailure(policy);
        DemoNonRetryableException(policy);
    }

    // Перевірка формули: затримки мають подвоюватися і лишатися в межах jitter
    static void CheckDelayFormula(RetryPolicy policy)
    {
        Console.WriteLine("Перевірка формули затримки (1000 обчислень для кожного повтору)");
        for (int retryAttempt = 0; retryAttempt < policy.MaxRetries; retryAttempt++)
        {
            List<double> delays = Enumerable.Range(0, 1000)
                .Select(_ => policy.CalculateDelay(retryAttempt).TotalMilliseconds)
                .ToList();

            double baseDelay = policy.InitialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt);
            double expectedMin = baseDelay * (1 - policy.JitterFactor);
            double expectedMax = baseDelay * (1 + policy.JitterFactor);
            bool inRange = delays.All(d => d >= expectedMin && d <= expectedMax);

            Console.WriteLine($"  Повтор #{retryAttempt + 1}: база {baseDelay:F0} мс, очікувано {expectedMin:F0}–{expectedMax:F0} мс, " +
                              $"отримано {delays.Min():F0}–{delays.Max():F0} мс, середнє {delays.Average():F0} мс " +
                              $"| {(inRange ? "усі в межах" : "Є ВИХІД ЗА МЕЖІ")}");
        }
        Console.WriteLine();
    }

    // Сценарій 1: операція двічі падає, а з третьої спроби вдається
    static void DemoSuccessAfterFailures(RetryPolicy policy)
    {
        Console.WriteLine("Сценарій 1. Завантаження звіту: сервер двічі не відповідає, потім працює");

        int calls = 0;
        string report = policy.Execute(() =>
        {
            calls++;
            if (calls <= 2)
            {
                throw new TimeoutException("сервер звітів не відповів вчасно");
            }
            return "звіт за жовтень (24 сторінки)";
        });

        Console.WriteLine($"  Результат: {report}, викликів операції: {calls}");
        Console.WriteLine();
    }

    // Сценарій 2: операція падає завжди, політика вичерпує спроби і викидає виняток
    static void DemoFinalFailure(RetryPolicy policy)
    {
        Console.WriteLine("Сценарій 2. Кількість користувачів онлайн: сервер статистики зламаний");

        int calls = 0;
        try
        {
            int usersOnline = policy.Execute<int>(() =>
            {
                calls++;
                throw new HttpRequestException("503 Service Unavailable");
            });
            Console.WriteLine($"  Користувачів онлайн: {usersOnline}");
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine($"  Main отримав виняток після всіх спроб: {exception.Message}, викликів операції: {calls}");
        }
        Console.WriteLine();
    }

    // Сценарій 3: shouldRetry забороняє повтор, бо неправильний пароль від повтору не стане правильним
    static void DemoNonRetryableException(RetryPolicy policy)
    {
        Console.WriteLine("Сценарій 3. Вхід з неправильним паролем: shouldRetry забороняє повтор");

        int calls = 0;
        try
        {
            bool loggedIn = policy.Execute<bool>(
                () =>
                {
                    calls++;
                    throw new UnauthorizedAccessException("неправильний пароль");
                },
                shouldRetry: exception => exception is TimeoutException or HttpRequestException);
            Console.WriteLine($"  Вхід виконано: {loggedIn}");
        }
        catch (UnauthorizedAccessException exception)
        {
            Console.WriteLine($"  Main отримав виняток одразу: {exception.Message}, викликів операції: {calls}");
        }
    }
}
