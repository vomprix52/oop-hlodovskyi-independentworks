namespace IndependentWork11;

// Набір тестових даних: товари, клієнти і продажі
public class SalesData
{
    // Товари за категоріями з діапазоном цін
    private static readonly (string Category, decimal MinPrice, decimal MaxPrice, string[] Names)[] Catalog =
    {
        ("Електроніка", 500m, 40000m, new[]
        {
            "Смартфон", "Ноутбук", "Навушники", "Планшет", "Павербанк", "Смарт-годинник",
            "Монітор", "Клавіатура", "Мишка", "Колонка", "Роутер", "Вебкамера"
        }),
        ("Книги", 150m, 900m, new[]
        {
            "Кобзар", "Тигролови", "Лісова пісня", "Чистий код", "Алгоритми", "Мистецтво програмування",
            "Атлас світу", "Англійська граматика", "Збірка задач", "Енциклопедія", "Детектив", "Фентезі"
        }),
        ("Одяг", 300m, 4000m, new[]
        {
            "Футболка", "Джинси", "Куртка", "Светр", "Кросівки", "Шапка",
            "Шарф", "Худі", "Сорочка", "Шорти", "Рукавиці", "Вишиванка"
        }),
        ("Спорт", 200m, 15000m, new[]
        {
            "М'яч", "Гантелі", "Скакалка", "Килимок для йоги", "Велосипед", "Ракетка",
            "Рюкзак", "Пляшка для води", "Еспандер", "Самокат", "Ковзани", "Намет"
        }),
        ("Дім", 100m, 8000m, new[]
        {
            "Чайник", "Лампа", "Подушка", "Ковдра", "Сковорідка", "Набір чашок",
            "Пилосос", "Праска", "Рушник", "Годинник", "Ваза", "Органайзер"
        })
    };

    private static readonly string[] Cities =
    {
        "Київ", "Львів", "Харків", "Одеса", "Дніпро", "Рівне", "Луцьк", "Вінниця"
    };

    private static readonly string[] FirstNames =
    {
        "Олександр", "Марія", "Іван", "Олена", "Андрій", "Наталія", "Дмитро", "Ірина",
        "Сергій", "Юлія", "Максим", "Тетяна", "Віктор", "Оксана", "Богдан", "Софія"
    };

    private static readonly string[] LastNames =
    {
        "Коваленко", "Шевченко", "Бондаренко", "Ткаченко", "Кравченко", "Мельник",
        "Олійник", "Поліщук", "Лисенко", "Мороз", "Савчук", "Гончар"
    };

    public List<Product> Products { get; }
    public List<Customer> Customers { get; }
    public List<Sale> Sales { get; }

    private SalesData(List<Product> products, List<Customer> customers, List<Sale> sales)
    {
        Products = products;
        Customers = customers;
        Sales = sales;
    }

    /// <summary>
    /// Генерує дані з фіксованим зерном Random, тому результат однаковий при кожному запуску.
    /// </summary>
    public static SalesData Generate(int customerCount, int saleCount, int seed = 2026)
    {
        Random random = new Random(seed);

        // 5 категорій по 12 товарів, усього 60 товарів
        List<Product> products = new List<Product>();
        foreach (var (category, minPrice, maxPrice, names) in Catalog)
        {
            foreach (string name in names)
            {
                // Ціна округлюється до 10 грн, щоб виглядала реалістично
                decimal price = Math.Round((minPrice + (maxPrice - minPrice) * (decimal)random.NextDouble()) / 10) * 10;
                products.Add(new Product(products.Count + 1, name, category, price));
            }
        }

        List<Customer> customers = new List<Customer>();
        for (int i = 1; i <= customerCount; i++)
        {
            string name = $"{FirstNames[random.Next(FirstNames.Length)]} {LastNames[random.Next(LastNames.Length)]}";
            customers.Add(new Customer(i, name, Cities[SkewedIndex(random, Cities.Length)]));
        }

        // Продажі за 18 місяців: з 1 січня 2025 до 30 червня 2026
        DateTime start = new DateTime(2025, 1, 1);
        int totalDays = (new DateTime(2026, 6, 30) - start).Days + 1;

        // Категорія обирається рівномірно, а всередині неї перші товари популярніші за інші
        List<List<Product>> productsByCategory = products
            .GroupBy(p => p.Category)
            .Select(g => g.ToList())
            .ToList();

        List<Sale> sales = new List<Sale>();
        for (int i = 1; i <= saleCount; i++)
        {
            List<Product> category = productsByCategory[random.Next(productsByCategory.Count)];
            Product product = category[SkewedIndex(random, category.Count)];
            Customer customer = customers[random.Next(customers.Count)];
            DateTime date = start.AddDays(random.Next(totalDays));
            sales.Add(new Sale(i, product.Id, random.Next(1, 6), date, customer.Id));
        }

        return new SalesData(products, customers, sales);
    }

    // Випадковий індекс, де перші елементи випадають частіше: так з'являються популярні товари і великі міста
    private static int SkewedIndex(Random random, int count)
    {
        double value = random.NextDouble();
        return (int)(value * value * count);
    }
}
