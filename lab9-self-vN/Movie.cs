namespace lab9_self_vN;

// Фільм у каталозі онлайн-кінотеатру
public class Movie
{
    // Приватні поля
    private string _title;
    private int _year;
    private double _rating;
    private int _durationMinutes;

    public string Title
    {
        get { return _title; }
    }

    public int Year
    {
        get { return _year; }
    }

    public double Rating
    {
        get { return _rating; }
    }

    public int DurationMinutes
    {
        get { return _durationMinutes; }
    }

    // Конструктор
    public Movie(string title, int year, double rating, int durationMinutes)
    {
        _title = title;
        _year = year;
        _rating = rating;
        _durationMinutes = durationMinutes;
    }

    // Тривалість у форматі "2 год 15 хв"
    public string GetDurationText()
    {
        return $"{_durationMinutes / 60} год {_durationMinutes % 60} хв";
    }

    public override string ToString()
    {
        return $"{Title} ({Year}), рейтинг {Rating:F1}, {GetDurationText()}";
    }
}
