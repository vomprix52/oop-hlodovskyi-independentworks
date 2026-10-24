namespace IndependentWork11;

// Товар магазину
public class Product
{
    public int Id { get; }
    public string Name { get; }
    public string Category { get; }
    public decimal UnitPrice { get; }

    // Конструктор
    public Product(int id, string name, string category, decimal unitPrice)
    {
        Id = id;
        Name = name;
        Category = category;
        UnitPrice = unitPrice;
    }
}
