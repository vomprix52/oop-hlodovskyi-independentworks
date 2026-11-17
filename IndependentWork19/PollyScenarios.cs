using System.Diagnostics;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19;

// Три сценарії відмовостійкості на основі Polly
public static class PollyScenarios
{
    /// <summary>
    /// Сценарій 1: Retry з експоненційною затримкою і jitter для тимчасово недоступного API.
    /// </summary>
    public static void RetryScenario()
    {
        Console.WriteLine("Сценарій 1. Retry: сервіс афіші тимчасово недоступний");
        Stopwatch clock = Stopwatch.StartNew();

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                // Повторюємо лише мережеві помилки, а не помилки в нашому коді
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                MaxRetryAttempts = 4,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    Log(clock, $"невдача: {args.Outcome.Exception?.Message}; " +
                               $"повтор #{args.AttemptNumber + 1} через {args.RetryDelay.TotalMilliseconds:F0} мс");
                    return default;
                }
            })
            .Build();

        // API падає 3 рази, а з четвертої спроби відповідає
        ScheduleApi api = new ScheduleApi(failuresBeforeSuccess: 3);
        string schedule = pipeline.Execute(() =>
        {
            Log(clock, "запит до API афіші");
            return api.GetSchedule();
        });

        Log(clock, $"успіх: {schedule}");
        Console.WriteLine();
    }

    /// <summary>
    /// Сценарій 2: Circuit Breaker для платіжного шлюзу, який не працює довше, ніж кілька секунд.
    /// </summary>
    public static void CircuitBreakerScenario()
    {
        Console.WriteLine("Сценарій 2. Circuit Breaker: платіжний шлюз не працює 1,3 с");
        Stopwatch clock = Stopwatch.StartNew();

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                // Вимикач розмикається, коли половина з щонайменше 3 запитів за 5 с невдалі
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                SamplingDuration = TimeSpan.FromSeconds(5),
                // Скільки часу вимикач тримається розімкненим перед пробним запитом
                BreakDuration = TimeSpan.FromMilliseconds(600),
                OnOpened = args =>
                {
                    Log(clock, $"   >>> вимикач РОЗІМКНЕНО на {args.BreakDuration.TotalMilliseconds:F0} мс");
                    return default;
                },
                OnHalfOpened = args =>
                {
                    Log(clock, "   >>> вимикач НАПІВРОЗІМКНЕНО, пропускаємо пробний запит");
                    return default;
                },
                OnClosed = args =>
                {
                    Log(clock, "   >>> вимикач ЗАМКНЕНО, шлюз знову працює");
                    return default;
                }
            })
            .Build();

        PaymentGateway gateway = new PaymentGateway(clock, downtime: TimeSpan.FromMilliseconds(1300));

        // 12 оплат з інтервалом 200 мс
        for (int i = 1; i <= 12; i++)
        {
            try
            {
                string result = pipeline.Execute(() => gateway.Charge(150m * i));
                Log(clock, $"оплата {i}: {result}");
            }
            catch (BrokenCircuitException)
            {
                // Запит навіть не дійшов до шлюзу
                Log(clock, $"оплата {i}: відхилено миттєво, вимикач розімкнений");
            }
            catch (InvalidOperationException exception)
            {
                Log(clock, $"оплата {i}: помилка шлюзу: {exception.Message}");
            }

            Thread.Sleep(200);
        }

        Console.WriteLine($"  Усього оплат: 12, з них реально дійшло до шлюзу: {gateway.RealCalls}");
        Console.WriteLine();
    }

    /// <summary>
    /// Сценарій 3: Timeout + Fallback для повільного сервісу відгуків.
    /// </summary>
    public static async Task TimeoutFallbackScenario()
    {
        Console.WriteLine("Сценарій 3. Timeout + Fallback: сервіс відгуків іноді дуже повільний");
        Stopwatch clock = Stopwatch.StartNew();

        // Fallback додається першим, тому він зовнішній і перехоплює тайм-аут внутрішньої стратегії
        ResiliencePipeline<string> pipeline = new ResiliencePipelineBuilder<string>()
            .AddFallback(new FallbackStrategyOptions<string>
            {
                ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutRejectedException>(),
                FallbackAction = args => Outcome.FromResultAsValueTask("рейтинг з кешу: 8,2 з 10 (дані за вчора)"),
                OnFallback = args =>
                {
                    Log(clock, "   >>> спрацював Fallback, повертаємо дані з кешу");
                    return default;
                }
            })
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMilliseconds(500),
                OnTimeout = args =>
                {
                    Log(clock, $"   >>> тайм-аут {args.Timeout.TotalMilliseconds:F0} мс, запит перервано");
                    return default;
                }
            })
            .Build();

        ReviewsService reviews = new ReviewsService();

        // Перший запит швидкий, другий дуже повільний, третій знову швидкий
        TimeSpan[] responseTimes =
        {
            TimeSpan.FromMilliseconds(150),
            TimeSpan.FromSeconds(3),
            TimeSpan.FromMilliseconds(200)
        };

        string[] movies = { "Дюна 2", "Мавка", "Довбуш" };
        for (int i = 0; i < movies.Length; i++)
        {
            Log(clock, $"запит рейтингу \"{movies[i]}\" (сервіс відповість через {responseTimes[i].TotalMilliseconds:F0} мс)");
            TimeSpan responseTime = responseTimes[i];
            string movie = movies[i];

            string rating = await pipeline.ExecuteAsync(
                async token => await reviews.GetRatingAsync(movie, responseTime, token));

            Log(clock, $"результат: {rating}");
        }

        Console.WriteLine();
    }

    // Рядок логу з часом від початку сценарію
    private static void Log(Stopwatch clock, string message)
    {
        Console.WriteLine($"  [{clock.ElapsedMilliseconds,5} мс] {message}");
    }
}
