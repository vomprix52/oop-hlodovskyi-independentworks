namespace IndependentWork6;

// Push-сповіщення в мобільний застосунок
public class PushNotifier : INotifier
{
    // На екрані телефону показується лише короткий текст
    private const int MaxPreviewLength = 40;

    // Приватні поля
    private string _deviceName;
    private bool _isDeviceOnline;

    public string ChannelName
    {
        get { return "Push"; }
    }

    // Чи пристрій зараз у мережі
    public bool IsDeviceOnline
    {
        get { return _isDeviceOnline; }
        set { _isDeviceOnline = value; }
    }

    // Конструктор
    public PushNotifier(string deviceName, bool isDeviceOnline)
    {
        _deviceName = deviceName;
        _isDeviceOnline = isDeviceOnline;
    }

    // Відправка можлива лише на пристрій у мережі, довгий текст обрізається
    public bool Send(string message)
    {
        if (!_isDeviceOnline)
        {
            Console.WriteLine($"  [{ChannelName}] Пристрій {_deviceName} не в мережі, сповіщення не доставлено");
            return false;
        }

        string preview = message.Length > MaxPreviewLength
            ? message.Substring(0, MaxPreviewLength) + "..."
            : message;

        Console.WriteLine($"  [{ChannelName}] На пристрій {_deviceName}: {preview}");
        return true;
    }
}
