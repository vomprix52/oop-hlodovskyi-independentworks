namespace IndependentWork4;

// Журнал, має номер випуску
public class Journal : LibraryItem
{
    // Приватне поле
    private int _issueNumber;

    // Номер випуску
    public int IssueNumber
    {
        get { return _issueNumber; }
        set { _issueNumber = value; }
    }

    // Конструктор передає номер, назву і рік у базовий клас
    public Journal(string uniqueId, string title, int publicationYear, int issueNumber)
        : base(uniqueId, title, publicationYear)
    {
        _issueNumber = issueNumber;
    }

    // Базова інформація плюс номер випуску
    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"Номер випуску: {IssueNumber}");
    }
}
