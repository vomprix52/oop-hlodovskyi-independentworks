namespace IndependentWork2;

// Товар інтернет-магазину
public class Product
{
    // Значення за замовчуванням для скороченого конструктора
    private const string DefaultCategory = "Uncategorized";
    private const int DefaultStockCount = 0;

    // Приватні поля
    private int _id;
    private string _name;
    private decimal _price;
    private string _category;
    private int _stockCount;

    // Властивості лише для читання, після створення товар не змінюється
    public int Id
    {
        get { return _id; }
    }

    public string Name
    {
        get { return _name; }
    }

    public decimal Price
    {
        get { return _price; }
    }

    public string Category
    {
        get { return _category; }
    }

    public int StockCount
    {
        get { return _stockCount; }
    }

    // Конструктор 1 (основний): ініціалізує всі поля
    public Product(int id, string name, decimal price, string category, int stockCount)
    {
        _id = id;
        _name = name;
        _price = price;
        _category = category;
        _stockCount = stockCount;
    }

    // Конструктор 2 (скорочений): категорія і кількість отримують значення за замовчуванням
    public Product(int id, string name, decimal price)
        : this(id, name, price, DefaultCategory, DefaultStockCount)
    {
    }

    // Конструктор 3 (копіювання): бере дані з іншого товару
    public Product(Product other)
        : this(other.Id, other.Name, other.Price, other.Category, other.StockCount)
    {
    }

    // Повна інформація про товар одним рядком
    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
    }
}
