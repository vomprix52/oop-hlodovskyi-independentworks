namespace IndependentWork8;

// Сценарій 1: кеш метаданих файлів на основі Dictionary
public class FileMetadataCache
{
    // Ключ це назва файлу; у Windows регістр у назвах не важливий, тому порівняння без регістру
    private Dictionary<string, FileMetadata> _cache;

    // Кількість файлів у кеші
    public int Count
    {
        get { return _cache.Count; }
    }

    // Конструктор
    public FileMetadataCache()
    {
        _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);
    }

    // Додає або оновлює метадані, повертає true, якщо файл новий
    public bool Add(FileMetadata metadata)
    {
        bool isNew = !_cache.ContainsKey(metadata.FileName);
        _cache[metadata.FileName] = metadata;
        return isNew;
    }

    // Повертає метадані або null, якщо файлу немає в кеші
    public FileMetadata? Get(string fileName)
    {
        if (_cache.TryGetValue(fileName, out FileMetadata? metadata))
        {
            return metadata;
        }

        return null;
    }

    // Видаляє файл з кешу, повертає true, якщо він там був
    public bool Remove(string fileName)
    {
        return _cache.Remove(fileName);
    }

    // Перевіряє, чи є файл у кеші
    public bool Contains(string fileName)
    {
        return _cache.ContainsKey(fileName);
    }
}
