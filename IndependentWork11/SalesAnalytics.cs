namespace IndependentWork11;

// Результати пайплайнів; record дає готові ToString, Equals і короткий запис
public record ProductSales(string Name, int TotalQuantity);
public record CategoryTopProducts(string Category, List<ProductSales> TopProducts);
public record CityRevenue(string City, decimal Revenue, int SalesCount);
public record MonthlySales(int Year, int Month, int TotalQuantity, decimal Revenue);
public record CustomerSummary(string Name, string City, int Purchases, decimal TotalSpent, decimal AverageCheck, string FavoriteCategory);

// Аналітичні LINQ-пайплайни над даними продажів
public class SalesAnalytics
{
    private readonly SalesData _data;

    public SalesAnalytics(SalesData data)
    {
        _data = data;
    }

    /// <summary>
    /// Пайплайн 1: три найпопулярніші товари (за кількістю проданих штук) у кожній категорії.
    /// </summary>
    public List<CategoryTopProducts> TopProductsByCategory(int topCount = 3)
    {
        return _data.Sales
            // 1. Групуємо продажі за товаром і рахуємо загальну кількість
            .GroupBy(s => s.ProductId)
            .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
            // 2. Додаємо назву і категорію товару
            .Join(_data.Products,
                  stat => stat.ProductId,
                  product => product.Id,
                  (stat, product) => new { product.Name, product.Category, stat.TotalQuantity })
            // 3. Групуємо за категорією і беремо перші topCount товарів
            .GroupBy(x => x.Category)
            .Select(g => new CategoryTopProducts(
                g.Key,
                g.OrderByDescending(x => x.TotalQuantity)
                 .ThenBy(x => x.Name)
                 .Take(topCount)
                 .Select(x => new ProductSales(x.Name, x.TotalQuantity))
                 .ToList()))
            .OrderBy(c => c.Category)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 2: загальний дохід за містами клієнтів, від найбільшого.
    /// Записаний синтаксисом запитів (query syntax), бо з двома join він читається легше.
    /// </summary>
    public List<CityRevenue> RevenueByCity()
    {
        var query =
            from sale in _data.Sales
            join product in _data.Products on sale.ProductId equals product.Id
            join customer in _data.Customers on sale.CustomerId equals customer.Id
            let revenue = sale.Quantity * product.UnitPrice
            group revenue by customer.City into cityGroup
            orderby cityGroup.Sum() descending
            select new CityRevenue(cityGroup.Key, cityGroup.Sum(), cityGroup.Count());

        return query.ToList();
    }

    /// <summary>
    /// Пайплайн 3: кількість проданих одиниць і дохід за кожен місяць.
    /// </summary>
    public List<MonthlySales> SalesByMonth()
    {
        return _data.Sales
            .Join(_data.Products,
                  sale => sale.ProductId,
                  product => product.Id,
                  (sale, product) => new
                  {
                      sale.SaleDate,
                      sale.Quantity,
                      Revenue = sale.Quantity * product.UnitPrice
                  })
            // Ключ групи складається з двох полів: року і місяця
            .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
            .Select(g => new MonthlySales(
                g.Key.Year,
                g.Key.Month,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.Revenue)))
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 4 (власний): найкращі клієнти за сумою покупок
    /// з кількістю покупок, середнім чеком і улюбленою категорією.
    /// </summary>
    public List<CustomerSummary> TopCustomers(int topCount = 5)
    {
        return _data.Sales
            .Join(_data.Products,
                  sale => sale.ProductId,
                  product => product.Id,
                  (sale, product) => new
                  {
                      sale.CustomerId,
                      product.Category,
                      Amount = sale.Quantity * product.UnitPrice
                  })
            .GroupBy(x => x.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Purchases = g.Count(),
                TotalSpent = g.Sum(x => x.Amount),
                // Улюблена категорія: та, на яку клієнт витратив найбільше
                FavoriteCategory = g.GroupBy(x => x.Category)
                                    .OrderByDescending(c => c.Sum(x => x.Amount))
                                    .First()
                                    .Key
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(topCount)
            .Join(_data.Customers,
                  stat => stat.CustomerId,
                  customer => customer.Id,
                  (stat, customer) => new CustomerSummary(
                      customer.Name,
                      customer.City,
                      stat.Purchases,
                      stat.TotalSpent,
                      Math.Round(stat.TotalSpent / stat.Purchases, 2),
                      stat.FavoriteCategory))
            .ToList();
    }
}
