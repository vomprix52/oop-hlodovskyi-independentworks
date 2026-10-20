namespace IndependentWork4;

// Базовий клас для всіх елементів бібліотеки
public class LibraryItem
{
    // Приватні поля
    private string _uniqueId;
    private string _title;
    private int _publicationYear;

    // Інвентарний номер лише для читання, після створення не змінюється
    public string UniqueId
    {
        get { return _uniqueId; }
    }

    // Назва елемента
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    // Рік видання
    public int PublicationYear
    {
        get { return _publicationYear; }
        set { _publicationYear = value; }
    }

    // Конструктор
    public LibraryItem(string uniqueId, string title, int publicationYear)
    {
        _uniqueId = uniqueId;
        _title = title;
        _publicationYear = publicationYear;
    }

    // Виводить базову інформацію, похідні класи доповнюють її
    public virtual void DisplayDetails()
    {
        Console.WriteLine($"Інвентарний номер: {UniqueId}");
        Console.WriteLine($"Назва: {Title}");
        Console.WriteLine($"Рік видання: {PublicationYear}");
    }
}
