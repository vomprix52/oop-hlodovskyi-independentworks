namespace IndependentWork4;

// Книга, має автора
public class Book : LibraryItem
{
    // Приватне поле
    private string _author;

    // Автор книги
    public string Author
    {
        get { return _author; }
        set { _author = value; }
    }

    // Конструктор передає номер, назву і рік у базовий клас
    public Book(string uniqueId, string title, int publicationYear, string author)
        : base(uniqueId, title, publicationYear)
    {
        _author = author;
    }

    // Базова інформація плюс автор
    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"Автор: {Author}");
    }
}
