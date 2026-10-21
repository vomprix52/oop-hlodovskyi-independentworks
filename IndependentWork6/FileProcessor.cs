namespace IndependentWork6;

// Базовий клас для обробників файлів: спільні кроки тут, унікальна обробка в нащадках
public abstract class FileProcessor
{
    // Приватні поля, нащадки працюють лише з готовим вмістом файлу
    private string _filePath;
    private StreamReader? _reader;

    public string FilePath
    {
        get { return _filePath; }
    }

    // Кожен нащадок повідомляє, з яким типом файлів працює
    public abstract string FileType { get; }

    // Конструктор
    protected FileProcessor(string filePath)
    {
        _filePath = filePath;
    }

    // Загальний порядок обробки однаковий для всіх файлів і не перевизначається
    public void Process()
    {
        Console.WriteLine($"Обробник {FileType}, файл {Path.GetFileName(_filePath)}");

        if (!Open())
        {
            return;
        }

        try
        {
            string content = _reader!.ReadToEnd();
            ProcessContent(content);
        }
        finally
        {
            Close();
        }
    }

    // Спільний крок: відкриття файлу з перевіркою, що він існує
    private bool Open()
    {
        if (!File.Exists(_filePath))
        {
            Console.WriteLine("  Помилка: файл не знайдено");
            return false;
        }

        _reader = new StreamReader(_filePath);
        Console.WriteLine("  Файл відкрито");
        return true;
    }

    // Спільний крок: закриття файлу виконується завжди, навіть після помилки
    private void Close()
    {
        _reader?.Dispose();
        _reader = null;
        Console.WriteLine("  Файл закрито");
    }

    // Унікальна логіка, яку кожен нащадок зобов'язаний реалізувати
    protected abstract void ProcessContent(string content);
}
