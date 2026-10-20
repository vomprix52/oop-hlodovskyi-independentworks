using System.Globalization;
using System.Text;

namespace IndependentWork5;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер і формату чисел
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        DemoObjectPolymorphism();
        DemoEnumerablePolymorphism();
        DemoStreamPolymorphism();
    }

    // Сценарій 1: поліморфізм через System.Object і ToString()
    static void DemoObjectPolymorphism()
    {
        Console.WriteLine("Сценарій 1. ToString() через List<object>");

        List<object> items = new List<object>
        {
            new Student("Іван Петренко", "ПІ-21", 4.6),
            new Laptop("Lenovo IdeaPad 5", 16, 28999.99m),
            new Concert("Хвиля", new DateTime(2026, 12, 5), 120),
            2026,
            new object()
        };

        // Змінна має тип object, але викликається ToString() того класу, яким є об'єкт
        foreach (object item in items)
        {
            Console.WriteLine($"{item.GetType().Name}: {item}");
        }

        Console.WriteLine();
    }

    // Сценарій 2: поліморфізм через інтерфейс IEnumerable<string>
    static void DemoEnumerablePolymorphism()
    {
        Console.WriteLine("Сценарій 2. Власна колекція з IEnumerable<string>");

        MyCustomCollection subjects = new MyCustomCollection();
        subjects.Add("Об'єктно-орієнтоване програмування");
        subjects.Add("Бази даних");
        subjects.Add("Вища математика");
        subjects.Add("   ");

        Console.WriteLine($"Предметів у колекції: {subjects.Count}");
        foreach (string subject in subjects)
        {
            Console.WriteLine($"  {subject}");
        }

        // Один метод працює з трьома різними типами колекцій
        string[] days = { "Понеділок", "Вівторок" };
        List<string> rooms = new List<string> { "Аудиторія 101", "Аудиторія 205" };

        PrintAll("MyCustomCollection", subjects);
        PrintAll("string[]", days);
        PrintAll("List<string>", rooms);

        Console.WriteLine();
    }

    // Приймає будь-яку колекцію рядків, що реалізує IEnumerable<string>
    static void PrintAll(string title, IEnumerable<string> collection)
    {
        Console.WriteLine($"{title}: {string.Join(", ", collection)}");
    }

    // Сценарій 3: поліморфізм через абстрактний клас Stream
    static void DemoStreamPolymorphism()
    {
        Console.WriteLine("Сценарій 3. Абстрактний клас Stream");

        // Тестовий файл створюється програмою поруч із виконуваним файлом
        string filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        File.WriteAllText(filePath, "Дані з файлу на диску");

        byte[] memoryData = Encoding.UTF8.GetBytes("Рядок з оперативної пам'яті");

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (MemoryStream memoryStream = new MemoryStream(memoryData))
        {
            ProcessStream(fileStream);
            ProcessStream(memoryStream);
        }
    }

    // Працює з будь-яким потоком, не знаючи, звідки беруться дані
    static void ProcessStream(Stream stream)
    {
        Console.WriteLine($"Тип потоку: {stream.GetType().Name}");
        Console.WriteLine($"  Розмір: {stream.Length} байтів, можна читати: {(stream.CanRead ? "так" : "ні")}");

        // Читаємо перші 8 байтів, щоб показати метод Read
        byte[] firstBytes = new byte[8];
        int bytesRead = stream.Read(firstBytes, 0, firstBytes.Length);
        Console.WriteLine($"  Перші {bytesRead} байтів: {BitConverter.ToString(firstBytes, 0, bytesRead)}");

        // Повертаємось на початок і читаємо весь текст
        stream.Position = 0;
        byte[] allBytes = new byte[stream.Length];
        stream.ReadExactly(allBytes);
        Console.WriteLine($"  Вміст: {Encoding.UTF8.GetString(allBytes)}");
    }
}
