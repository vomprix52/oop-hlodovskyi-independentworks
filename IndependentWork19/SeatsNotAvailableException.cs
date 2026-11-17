namespace IndependentWork19;

// Виняток: на сеанс не вистачає вільних місць для бронювання
public class SeatsNotAvailableException : Exception
{
    // Назва сеансу, на який бронювали
    public string SessionName { get; } = string.Empty;

    // Скільки місць хотіли забронювати
    public int RequestedSeats { get; }

    // Скільки місць було вільно
    public int AvailableSeats { get; }

    // Три стандартні конструктори
    public SeatsNotAvailableException()
    {
    }

    public SeatsNotAvailableException(string message)
        : base(message)
    {
    }

    public SeatsNotAvailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Конструктор з даними про бронювання
    public SeatsNotAvailableException(string message, string sessionName, int requestedSeats, int availableSeats)
        : base(message)
    {
        SessionName = sessionName;
        RequestedSeats = requestedSeats;
        AvailableSeats = availableSeats;
    }
}
