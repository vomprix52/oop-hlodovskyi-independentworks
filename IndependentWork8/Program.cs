using System.Security.Cryptography;
using System.Text;

namespace IndependentWork8;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер
        Console.OutputEncoding = Encoding.UTF8;

        DemoFileMetadataCache();
        DemoTagManager();
        DemoSearchIndex();
    }

    // Сценарій 1: Dictionary як кеш метаданих
    static void DemoFileMetadataCache()
    {
        Console.WriteLine("Сценарій 1. Кеш метаданих файлів (Dictionary)");

        FileMetadataCache cache = new FileMetadataCache();
        cache.Add(CreateMetadata("report.docx", "Звіт з практики", new DateTime(2026, 10, 1, 9, 30, 0)));
        cache.Add(CreateMetadata("photo.jpg", "Фото з екскурсії", new DateTime(2026, 9, 14, 18, 5, 0)));
        cache.Add(CreateMetadata("notes.txt", "Конспект з ООП", new DateTime(2026, 10, 20, 21, 45, 0)));
        Console.WriteLine($"Файлів у кеші: {cache.Count}");

        // Пошук за ключем не залежить від регістру, як у файловій системі Windows
        Console.WriteLine($"Get(\"REPORT.DOCX\"): {cache.Get("REPORT.DOCX")}");
        Console.WriteLine($"Get(\"video.mp4\"): {cache.Get("video.mp4")?.ToString() ?? "немає в кеші"}");

        // Повторне додавання того самого файлу оновлює запис, а не створює новий
        bool isNew = cache.Add(CreateMetadata("notes.txt", "Конспект з ООП, доповнений", new DateTime(2026, 10, 21, 8, 0, 0)));
        Console.WriteLine($"Оновлення notes.txt: {(isNew ? "новий файл" : "запис оновлено")}, файлів у кеші: {cache.Count}");
        Console.WriteLine($"Get(\"notes.txt\"): {cache.Get("notes.txt")}");

        Console.WriteLine($"Contains(\"photo.jpg\"): {cache.Contains("photo.jpg")}");
        Console.WriteLine($"Remove(\"photo.jpg\"): {cache.Remove("photo.jpg")}");
        Console.WriteLine($"Contains(\"photo.jpg\") після видалення: {cache.Contains("photo.jpg")}");
        Console.WriteLine($"Файлів у кеші: {cache.Count}");
        Console.WriteLine();
    }

    // Створює метадані з вигаданим вмістом файлу і рахує його хеш
    static FileMetadata CreateMetadata(string fileName, string content, DateTime lastModified)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        string hash = Convert.ToHexString(SHA256.HashData(bytes)).Substring(0, 8);
        return new FileMetadata(fileName, bytes.Length, lastModified, hash);
    }

    // Сценарій 2: HashSet для унікальних тегів і операцій над множинами
    static void DemoTagManager()
    {
        Console.WriteLine("Сценарій 2. Теги статей блогу (HashSet)");

        TagManager tags = new TagManager();
        tags.AddTag("article-1", "C#");
        tags.AddTag("article-1", "ООП");
        tags.AddTag("article-1", "колекції");
        tags.AddTag("article-1", "HashSet");

        tags.AddTag("article-2", "C#");
        tags.AddTag("article-2", "колекції");
        tags.AddTag("article-2", "LINQ");

        tags.AddTag("article-3", "Python");
        tags.AddTag("article-3", "ооп");

        // Дублікати не додаються, навіть якщо відрізняються регістром
        bool added = tags.AddTag("article-1", "c#");
        Console.WriteLine($"Повторне додавання тегу \"c#\" до article-1: {(added ? "додано" : "вже є, не додано")}");

        Console.WriteLine($"Теги article-1: {Format(tags.GetTags("article-1"))}");
        Console.WriteLine($"Теги article-2: {Format(tags.GetTags("article-2"))}");
        Console.WriteLine($"Теги article-3: {Format(tags.GetTags("article-3"))}");
        Console.WriteLine($"Усі унікальні теги (UnionWith): {Format(tags.GetAllUniqueTags())}");
        Console.WriteLine($"Спільні теги article-1 і article-2 (IntersectWith): {Format(tags.FindCommonTags("article-1", "article-2"))}");
        Console.WriteLine($"Є в article-1, але немає в article-2 (ExceptWith): {Format(tags.FindDifferentTags("article-1", "article-2"))}");

        tags.RemoveTag("article-1", "HashSet");
        Console.WriteLine($"Теги article-1 після RemoveTag(\"HashSet\"): {Format(tags.GetTags("article-1"))}");
        Console.WriteLine();
    }

    // Сценарій 3: інвертований індекс, тобто Dictionary з HashSet усередині
    static void DemoSearchIndex()
    {
        Console.WriteLine("Сценарій 3. Пошуковий індекс (Dictionary + HashSet)");

        string[] documents =
        {
            "Dictionary зберігає пари ключ і значення",
            "HashSet зберігає лише унікальні значення",
            "Dictionary і HashSet шукають елемент за хешем",
            "List шукає елемент перебором, тому пошук повільніший"
        };

        SearchIndex index = new SearchIndex();
        for (int i = 0; i < documents.Length; i++)
        {
            index.AddDocument(i + 1, documents[i]);
            Console.WriteLine($"Документ {i + 1}: {documents[i]}");
        }

        Console.WriteLine($"Різних слів в індексі: {index.WordCount}");
        Console.WriteLine($"Search(\"dictionary\"): {Format(index.Search("dictionary"))}");
        Console.WriteLine($"Search(\"значення\"): {Format(index.Search("значення"))}");
        Console.WriteLine($"Search(\"елемент\"): {Format(index.Search("елемент"))}");
        Console.WriteLine($"SearchMultiple(\"зберігає\", \"значення\"): {Format(index.SearchMultiple("зберігає", "значення"))}");
        Console.WriteLine($"SearchMultiple(\"HashSet\", \"хешем\"): {Format(index.SearchMultiple("HashSet", "хешем"))}");
        Console.WriteLine($"Search(\"Java\"): {Format(index.Search("Java"))}");
    }

    // HashSet не гарантує порядок, тому для зручного виводу елементи сортуються
    static string Format<T>(HashSet<T> set)
    {
        if (set.Count == 0)
        {
            return "нічого не знайдено";
        }

        return string.Join(", ", set.Order());
    }
}
