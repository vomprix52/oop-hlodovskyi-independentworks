namespace IndependentWork11;

// Один продаж: який товар, скільки штук, коли і кому
public class Sale
{
    public int Id { get; }
    public int ProductId { get; }
    public int Quantity { get; }
    public DateTime SaleDate { get; }
    public int CustomerId { get; }

    // Конструктор
    public Sale(int id, int productId, int quantity, DateTime saleDate, int customerId)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        SaleDate = saleDate;
        CustomerId = customerId;
    }
}
