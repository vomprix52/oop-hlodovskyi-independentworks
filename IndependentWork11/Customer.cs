namespace IndependentWork11;

// Клієнт магазину
public class Customer
{
    public int Id { get; }
    public string Name { get; }
    public string City { get; }

    // Конструктор
    public Customer(int id, string name, string city)
    {
        Id = id;
        Name = name;
        City = city;
    }
}
