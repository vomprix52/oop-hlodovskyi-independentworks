namespace IndependentWork4;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Book book = new Book("B-001", "Тіні забутих предків", 1911, "Михайло Коцюбинський");
        Journal journal = new Journal("J-001", "Наука і життя", 2024, 7);
        AudioBook audioBook = new AudioBook("A-001", "Кайдашева сім'я", 1879, "Іван Нечуй-Левицький", "Олег Коваль");

        Console.WriteLine("Книга:");
        book.DisplayDetails();
        Console.WriteLine();

        Console.WriteLine("Журнал:");
        journal.DisplayDetails();
        Console.WriteLine();

        Console.WriteLine("Аудіокнига:");
        audioBook.DisplayDetails();
        Console.WriteLine();

        // Поліморфізм: усі об'єкти в одному масиві базового типу,
        // але для кожного викликається його власна версія DisplayDetails
        LibraryItem[] catalog = { book, journal, audioBook };

        Console.WriteLine("Каталог бібліотеки (виклик через тип LibraryItem):");
        foreach (LibraryItem item in catalog)
        {
            Console.WriteLine();
            Console.WriteLine($"Тип: {item.GetType().Name}");
            item.DisplayDetails();
        }
    }
}
