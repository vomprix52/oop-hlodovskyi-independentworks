using System.Xml.Linq;

namespace IndependentWork6;

// Обробник XML-файлів: знаходить кореневий елемент і виводить вкладені записи
public class XmlFileProcessor : FileProcessor
{
    public override string FileType
    {
        get { return "XML"; }
    }

    // Конструктор передає шлях у базовий клас
    public XmlFileProcessor(string filePath)
        : base(filePath)
    {
    }

    protected override void ProcessContent(string content)
    {
        XDocument document = XDocument.Parse(content);
        XElement root = document.Root!;

        Console.WriteLine($"  Кореневий елемент: {root.Name}, записів: {root.Elements().Count()}");
        foreach (XElement element in root.Elements())
        {
            Console.WriteLine($"    {element.Attribute("name")?.Value}: {element.Value}");
        }
    }
}
