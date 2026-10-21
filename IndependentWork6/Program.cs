using System.Globalization;
using System.Text;

namespace IndependentWork6;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер і формату чисел
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        DemoNotifications();
        DemoFileProcessing();
    }

    // Сценарій 1: різні канали сповіщень через інтерфейс INotifier
    static void DemoNotifications()
    {
        Console.WriteLine("Сценарій 1. Система сповіщень (інтерфейс INotifier)");

        List<INotifier> notifiers = new List<INotifier>
        {
            new EmailNotifier("student@college.edu.ua", "Розклад"),
            new SmsNotifier("+380 67 123 45 67"),
            new PushNotifier("Samsung Galaxy A54", true),
            new EmailNotifier("wrong-address", "Розклад"),
            new PushNotifier("iPhone 13", false)
        };

        string message = "Завтра пара з ООП переноситься на 10:00 в аудиторію 205";

        // Клієнтський код не знає, який саме канал перед ним
        int delivered = 0;
        foreach (INotifier notifier in notifiers)
        {
            if (notifier.Send(message))
            {
                delivered++;
            }
        }

        Console.WriteLine($"Доставлено {delivered} з {notifiers.Count} сповіщень");
        Console.WriteLine();
    }

    // Сценарій 2: обробка різних файлів через абстрактний клас FileProcessor
    static void DemoFileProcessing()
    {
        Console.WriteLine("Сценарій 2. Обробка файлів (абстрактний клас FileProcessor)");

        // Тестові файли створюються програмою поруч із виконуваним файлом
        string folder = AppContext.BaseDirectory;
        string textPath = Path.Combine(folder, "notes.txt");
        string xmlPath = Path.Combine(folder, "settings.xml");
        string csvPath = Path.Combine(folder, "grades.csv");

        File.WriteAllText(textPath, "Інтерфейс задає контракт\nАбстрактний клас дає спільний код\nОбидва підтримують поліморфізм");
        File.WriteAllText(xmlPath, "<settings><option name=\"theme\">dark</option><option name=\"language\">uk</option></settings>");
        File.WriteAllText(csvPath, "student;subject;grade\nПетренко;ООП;4.5\nКоваленко;ООП;5\nШевчук;ООП;3.5");

        List<FileProcessor> processors = new List<FileProcessor>
        {
            new TextFileProcessor(textPath),
            new XmlFileProcessor(xmlPath),
            new CsvFileProcessor(csvPath),
            new TextFileProcessor(Path.Combine(folder, "missing.txt"))
        };

        // Для кожного файлу викликається той самий Process, а вміст обробляється по-різному
        foreach (FileProcessor processor in processors)
        {
            processor.Process();
        }
    }
}
