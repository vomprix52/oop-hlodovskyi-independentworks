namespace IndependentWork8;

// Метадані файлу: назва, розмір, дата зміни і хеш вмісту
public class FileMetadata
{
    public string FileName { get; set; }
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; }

    // Конструктор
    public FileMetadata(string fileName, long size, DateTime lastModified, string hash)
    {
        FileName = fileName;
        Size = size;
        LastModified = lastModified;
        Hash = hash;
    }

    public override string ToString()
    {
        return $"{FileName}, {Size} байтів, змінено {LastModified:dd.MM.yyyy HH:mm}, хеш {Hash}";
    }
}
