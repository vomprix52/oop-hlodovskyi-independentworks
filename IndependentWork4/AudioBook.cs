namespace IndependentWork4;

// Аудіокнига: це книга, яку ще й начитав оповідач
public class AudioBook : Book
{
    // Приватне поле
    private string _narrator;

    // Хто начитав аудіокнигу
    public string Narrator
    {
        get { return _narrator; }
        set { _narrator = value; }
    }

    // Конструктор передає дані книги в Book, а той далі в LibraryItem
    public AudioBook(string uniqueId, string title, int publicationYear, string author, string narrator)
        : base(uniqueId, title, publicationYear, author)
    {
        _narrator = narrator;
    }

    // Інформація про книгу плюс оповідач
    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"Оповідач: {Narrator}");
    }
}
