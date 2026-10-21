using System.Globalization;

namespace IndependentWork6;

// Обробник CSV-файлів: рахує середнє значення в останній колонці
public class CsvFileProcessor : FileProcessor
{
    public override string FileType
    {
        get { return "CSV"; }
    }

    // Конструктор передає шлях у базовий клас
    public CsvFileProcessor(string filePath)
        : base(filePath)
    {
    }

    protected override void ProcessContent(string content)
    {
        string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string[] header = lines[0].Trim().Split(';');

        // Перший рядок є заголовком, тому дані починаються з другого
        double sum = 0;
        int rows = lines.Length - 1;
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cells = lines[i].Trim().Split(';');
            sum += double.Parse(cells[cells.Length - 1], CultureInfo.InvariantCulture);
        }

        Console.WriteLine($"  Колонки: {string.Join(", ", header)}");
        Console.WriteLine($"  Рядків з даними: {rows}, середнє ({header[header.Length - 1]}): {sum / rows:F2}");
    }
}
