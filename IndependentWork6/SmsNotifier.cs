namespace IndependentWork6;

// Сповіщення через SMS
public class SmsNotifier : INotifier
{
    // Максимальна довжина одного SMS кирилицею
    private const int MaxSegmentLength = 70;

    // Приватні поля
    private string _phoneNumber;
    private int _sentSegments;

    public string ChannelName
    {
        get { return "SMS"; }
    }

    // Скільки SMS-частин відправлено загалом
    public int SentSegments
    {
        get { return _sentSegments; }
    }

    // Конструктор
    public SmsNotifier(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        _sentSegments = 0;
    }

    // Довге повідомлення ділиться на кілька SMS
    public bool Send(string message)
    {
        int segments = (int)Math.Ceiling((double)message.Length / MaxSegmentLength);
        _sentSegments += segments;

        Console.WriteLine($"  [{ChannelName}] На номер {_phoneNumber} ({segments} SMS): {message}");
        return true;
    }
}
