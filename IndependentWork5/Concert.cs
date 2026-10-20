namespace IndependentWork5;

// Концерт, на який продаються квитки
public class Concert
{
    // Приватні поля
    private string _artist;
    private DateTime _date;
    private int _ticketsLeft;

    public string Artist
    {
        get { return _artist; }
    }

    public DateTime Date
    {
        get { return _date; }
    }

    public int TicketsLeft
    {
        get { return _ticketsLeft; }
    }

    // Конструктор
    public Concert(string artist, DateTime date, int ticketsLeft)
    {
        _artist = artist;
        _date = date;
        _ticketsLeft = ticketsLeft;
    }

    // Перевизначення методу з System.Object
    public override string ToString()
    {
        return $"Концерт гурту {Artist}, {Date:dd.MM.yyyy}, залишилось квитків: {TicketsLeft}";
    }
}
