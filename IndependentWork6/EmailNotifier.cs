namespace IndependentWork6;

// Сповіщення електронною поштою
public class EmailNotifier : INotifier
{
    // Приватні поля
    private string _emailAddress;
    private string _subject;

    public string ChannelName
    {
        get { return "Email"; }
    }

    // Конструктор
    public EmailNotifier(string emailAddress, string subject)
    {
        _emailAddress = emailAddress;
        _subject = subject;
    }

    // Лист відправляється лише на адресу з символом @
    public bool Send(string message)
    {
        if (!_emailAddress.Contains('@'))
        {
            Console.WriteLine($"  [{ChannelName}] Помилка: некоректна адреса {_emailAddress}");
            return false;
        }

        Console.WriteLine($"  [{ChannelName}] Лист на {_emailAddress}, тема \"{_subject}\": {message}");
        return true;
    }
}
