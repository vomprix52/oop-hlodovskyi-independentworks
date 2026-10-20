namespace IndependentWork5;

// Ноутбук у магазині техніки
public class Laptop
{
    // Приватні поля
    private string _model;
    private int _ramGb;
    private decimal _price;

    public string Model
    {
        get { return _model; }
    }

    public int RamGb
    {
        get { return _ramGb; }
    }

    public decimal Price
    {
        get { return _price; }
    }

    // Конструктор
    public Laptop(string model, int ramGb, decimal price)
    {
        _model = model;
        _ramGb = ramGb;
        _price = price;
    }

    // Перевизначення методу з System.Object
    public override string ToString()
    {
        return $"Ноутбук {Model}, {RamGb} ГБ ОЗП, ціна {Price:F2} грн";
    }
}
