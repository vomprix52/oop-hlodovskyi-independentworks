using System.Diagnostics;

namespace IndependentWork19;

// Імітація зовнішнього API афіші: перші кілька викликів падають, потім працює
public class ScheduleApi
{
    private readonly int _failuresBeforeSuccess;
    private int _calls;

    public ScheduleApi(int failuresBeforeSuccess)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public string GetSchedule()
    {
        _calls++;
        if (_calls <= _failuresBeforeSuccess)
        {
            throw new HttpRequestException($"503 Service Unavailable (виклик {_calls})");
        }

        return "Дюна 2 о 18:00, Мавка о 20:30";
    }
}

// Імітація платіжного шлюзу, який не працює певний час, а потім відновлюється
public class PaymentGateway
{
    private readonly Stopwatch _clock;
    private readonly TimeSpan _downtime;

    public int RealCalls { get; private set; }

    public PaymentGateway(Stopwatch clock, TimeSpan downtime)
    {
        _clock = clock;
        _downtime = downtime;
    }

    public string Charge(decimal amount)
    {
        RealCalls++;
        if (_clock.Elapsed < _downtime)
        {
            throw new InvalidOperationException("Платіжний шлюз не відповідає");
        }

        return $"оплату {amount} грн проведено";
    }
}

// Імітація сервісу відгуків, який іноді відповідає дуже довго
public class ReviewsService
{
    public async Task<string> GetRatingAsync(string movie, TimeSpan responseTime, CancellationToken cancellationToken)
    {
        // CancellationToken дозволяє Polly перервати очікування, коли спрацює тайм-аут
        await Task.Delay(responseTime, cancellationToken);
        return $"{movie}: 8,4 з 10 (свіжі дані)";
    }
}
