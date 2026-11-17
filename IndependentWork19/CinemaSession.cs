namespace IndependentWork19;

// Сеанс у кінотеатрі з обмеженою кількістю місць
public class CinemaSession
{
    private string _name;
    private int _freeSeats;

    public string Name
    {
        get { return _name; }
    }

    public int FreeSeats
    {
        get { return _freeSeats; }
    }

    public CinemaSession(string name, int freeSeats)
    {
        _name = name;
        _freeSeats = freeSeats;
    }

    // Бронює місця; якщо їх не вистачає, викидає власний виняток з деталями
    public void BookSeats(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Кількість місць має бути більшою за нуль");
        }

        if (count > _freeSeats)
        {
            throw new SeatsNotAvailableException(
                $"На сеанс \"{_name}\" не вистачає місць",
                _name,
                count,
                _freeSeats);
        }

        _freeSeats -= count;
    }

    // Бронювання за текстовим запитом, наприклад з форми на сайті
    public void BookSeatsFromText(string text)
    {
        try
        {
            BookSeats(int.Parse(text));
        }
        catch (FormatException exception)
        {
            // Початкова причина зберігається як InnerException
            throw new SeatsNotAvailableException($"Не вдалося розпізнати кількість місць: \"{text}\"", exception);
        }
    }
}
