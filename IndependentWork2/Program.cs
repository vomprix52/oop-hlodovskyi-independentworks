using System.Globalization;

namespace IndependentWork2;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер і символу гривні
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        Console.WriteLine("Створення товарів");

        // Основний конструктор: усі п'ять параметрів
        Product laptop = new Product(101, "Laptop", 35000m, "Electronics", 15);
        Console.WriteLine($"Товар 1 (основний конструктор): {laptop}");

        // Скорочений конструктор: лише id, назва і ціна
        Product mouse = new Product(102, "Mouse", 800m);
        Console.WriteLine($"Товар 2 (скорочений конструктор): {mouse}");

        // Конструктор копіювання: дублікат товару laptop
        Product laptopCopy = new Product(laptop);
        Console.WriteLine($"Товар 3 (конструктор копіювання): {laptopCopy}");

        // Копія містить ті самі дані, але це окремий об'єкт у пам'яті
        bool isSameObject = ReferenceEquals(laptop, laptopCopy);
        Console.WriteLine($"Копія є тим самим об'єктом, що й оригінал: {(isSameObject ? "так" : "ні")}");
    }
}
