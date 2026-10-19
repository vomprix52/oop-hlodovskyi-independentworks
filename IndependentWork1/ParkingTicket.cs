namespace IndependentWork1;

// Талон на платній автостоянці
public class ParkingTicket
{
    // Приватні поля
    private string _plateNumber;
    private DateTime _entryTime;
    private decimal _hourlyRate;
    private bool _isPaid;

    // Номер автомобіля лише для читання, після видачі талона не змінюється
    public string PlateNumber
    {
        get { return _plateNumber; }
    }

    // Час заїзду лише для читання
    public DateTime EntryTime
    {
        get { return _entryTime; }
    }

    // Тариф за годину; від'ємний тариф не приймається
    public decimal HourlyRate
    {
        get { return _hourlyRate; }
        set
        {
            if (value >= 0)
            {
                _hourlyRate = value;
            }
        }
    }

    // Стан оплати лише для читання
    public bool IsPaid
    {
        get { return _isPaid; }
    }

    // Конструктор
    public ParkingTicket(string plateNumber, DateTime entryTime, decimal hourlyRate)
    {
        _plateNumber = plateNumber;
        _entryTime = entryTime;
        _hourlyRate = hourlyRate >= 0 ? hourlyRate : 0;
        _isPaid = false;
    }

    // Кількість оплачуваних годин, неповна година рахується як повна
    public int GetBilledHours(DateTime exitTime)
    {
        if (exitTime <= _entryTime)
        {
            return 0;
        }

        TimeSpan duration = exitTime - _entryTime;
        return (int)Math.Ceiling(duration.TotalHours);
    }

    // Вартість стоянки до моменту виїзду
    public decimal CalculateCost(DateTime exitTime)
    {
        return GetBilledHours(exitTime) * _hourlyRate;
    }

    // Оплата талона, повторна оплата неможлива
    public bool Pay()
    {
        if (_isPaid)
        {
            return false;
        }

        _isPaid = true;
        return true;
    }
}
