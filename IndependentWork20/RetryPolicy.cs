namespace IndependentWork20;

// Політика повторних спроб з експоненційною затримкою і випадковим jitter.
// Метод Execute<T> узагальнений, тому політика підходить для будь-якої операції Func<T>
public class RetryPolicy
{
    // Приватні поля
    private int _maxRetries;
    private TimeSpan _initialDelay;
    private double _jitterFactor;
    private Random _random;

    public int MaxRetries
    {
        get { return _maxRetries; }
    }

    public TimeSpan InitialDelay
    {
        get { return _initialDelay; }
    }

    public double JitterFactor
    {
        get { return _jitterFactor; }
    }

    // Конструктор з перевіркою параметрів
    public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxRetries);
        ArgumentOutOfRangeException.ThrowIfLessThan(initialDelay, TimeSpan.Zero);

        // Jitter більше 100% дав би від'ємну затримку
        if (jitterFactor < 0 || jitterFactor > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(jitterFactor), "Коефіцієнт jitter має бути від 0 до 1");
        }

        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _jitterFactor = jitterFactor;
        _random = new Random();
    }

    /// <summary>
    /// Виконує операцію; при невдачі чекає і повторює, поки не вичерпано maxRetries.
    /// shouldRetry вирішує, чи варто повторювати для конкретного винятку; якщо його не передано, повторюються всі.
    /// </summary>
    public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);

        int totalAttempts = _maxRetries + 1;
        for (int retryAttempt = 0; ; retryAttempt++)
        {
            int attemptNumber = retryAttempt + 1;
            try
            {
                Console.WriteLine($"  Спроба {attemptNumber} з {totalAttempts}...");
                T result = operation();
                Console.WriteLine($"  Спроба {attemptNumber}: успіх");
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"  Спроба {attemptNumber}: невдача, {exception.GetType().Name}: {exception.Message}");

                // Деякі помилки повтор не виправить, наприклад неправильний пароль
                if (shouldRetry is not null && !shouldRetry(exception))
                {
                    Console.WriteLine("  Цей виняток не підлягає повтору, операцію зупинено");
                    throw;
                }

                if (retryAttempt >= _maxRetries)
                {
                    Console.WriteLine($"  Усі {totalAttempts} спроби вичерпано, виняток передається далі");
                    throw;
                }

                TimeSpan delay = CalculateDelay(retryAttempt);
                Console.WriteLine($"  Затримка перед повтором #{attemptNumber}: {delay.TotalMilliseconds:F0} мс");
                Thread.Sleep(delay);
            }
        }
    }

    /// <summary>
    /// Затримка за формулою з умови:
    /// initialDelay * 2^retryAttempt * (1 + random * jitterFactor * 2 - jitterFactor).
    /// Множник у дужках випадковий у межах від (1 - jitterFactor) до (1 + jitterFactor).
    /// </summary>
    public TimeSpan CalculateDelay(int retryAttempt)
    {
        double jitter = 1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor;
        return _initialDelay * Math.Pow(2, retryAttempt) * jitter;
    }
}
