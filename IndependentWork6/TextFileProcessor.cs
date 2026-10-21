namespace IndependentWork6;

// Обробник текстових файлів: рахує рядки і слова
public class TextFileProcessor : FileProcessor
{
    public override string FileType
    {
        get { return "TXT"; }
    }

    // Конструктор передає шлях у базовий клас
    public TextFileProcessor(string filePath)
        : base(filePath)
    {
    }

    protected override void ProcessContent(string content)
    {
        string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string[] words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        Console.WriteLine($"  Рядків: {lines.Length}, слів: {words.Length}");
    }
}
