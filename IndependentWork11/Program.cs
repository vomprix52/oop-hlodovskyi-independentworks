using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace IndependentWork11;

class Program
{
    private static readonly CultureInfo Ukrainian = new CultureInfo("uk-UA");

    static void Main(string[] args)
    {
        // Коректне відображення українських літер і формату чисел
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = Ukrainian;

        // Extract: завантаження (генерація) даних
        SalesData data = SalesData.Generate(customerCount: 80, saleCount: 1000);
        Console.WriteLine($"Товарів: {data.Products.Count}, клієнтів: {data.Customers.Count}, продажів: {data.Sales.Count}");
        Console.WriteLine();

        SalesAnalytics analytics = new SalesAnalytics(data);

        // Transform + Load: кожен пайплайн перетворює дані і складає результат у список
        PrintTopProducts(Measure(() => analytics.TopProductsByCategory(), out double time1), time1);
        PrintRevenueByCity(Measure(() => analytics.RevenueByCity(), out double time2), time2);
        PrintSalesByMonth(Measure(() => analytics.SalesByMonth(), out double time3), time3);
        PrintTopCustomers(Measure(() => analytics.TopCustomers(), out double time4), time4);

        RunPerformanceComparison();
    }

    // Виконує пайплайн один раз і вимірює час; для кожного пайплайну свій Stopwatch
    static List<T> Measure<T>(Func<List<T>> pipeline, out double milliseconds)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<T> result = pipeline();
        stopwatch.Stop();
        milliseconds = stopwatch.Elapsed.TotalMilliseconds;
        return result;
    }

    // Середній час із кількох повторів, коли код уже скомпільований і оптимізований
    static double MeasureAverage<T>(Func<List<T>> pipeline, int repetitions)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < repetitions; i++)
        {
            pipeline();
        }
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMilliseconds / repetitions;
    }

    static void PrintTopProducts(List<CategoryTopProducts> result, double milliseconds)
    {
        Console.WriteLine("Пайплайн 1. Найпопулярніші товари за категоріями");
        foreach (CategoryTopProducts category in result)
        {
            Console.WriteLine($"  {category.Category}:");
            for (int i = 0; i < category.TopProducts.Count; i++)
            {
                ProductSales product = category.TopProducts[i];
                Console.WriteLine($"    {i + 1}. {product.Name}: {product.TotalQuantity} шт.");
            }
        }
        Console.WriteLine($"  Час виконання: {milliseconds:F3} мс");
        Console.WriteLine();
    }

    static void PrintRevenueByCity(List<CityRevenue> result, double milliseconds)
    {
        Console.WriteLine("Пайплайн 2. Загальний дохід за містами клієнтів");
        foreach (CityRevenue city in result)
        {
            Console.WriteLine($"  {city.City,-8} {city.Revenue,14:N2} грн  (продажів: {city.SalesCount})");
        }
        Console.WriteLine($"  Разом: {result.Sum(c => c.Revenue):N2} грн");
        Console.WriteLine($"  Час виконання: {milliseconds:F3} мс");
        Console.WriteLine();
    }

    static void PrintSalesByMonth(List<MonthlySales> result, double milliseconds)
    {
        Console.WriteLine("Пайплайн 3. Продажі за місяцями та роками");
        foreach (MonthlySales month in result)
        {
            string monthName = Ukrainian.TextInfo.ToTitleCase(Ukrainian.DateTimeFormat.GetMonthName(month.Month));
            Console.WriteLine($"  {monthName,-9} {month.Year}: {month.TotalQuantity,4} шт., {month.Revenue,14:N2} грн");
        }
        Console.WriteLine($"  Час виконання: {milliseconds:F3} мс");
        Console.WriteLine();
    }

    static void PrintTopCustomers(List<CustomerSummary> result, double milliseconds)
    {
        Console.WriteLine("Пайплайн 4. П'ять найкращих клієнтів за сумою покупок");
        for (int i = 0; i < result.Count; i++)
        {
            CustomerSummary customer = result[i];
            Console.WriteLine($"  {i + 1}. {customer.Name} ({customer.City})");
            Console.WriteLine($"     покупок: {customer.Purchases}, сума: {customer.TotalSpent:N2} грн, " +
                              $"середній чек: {customer.AverageCheck:N2} грн, улюблена категорія: {customer.FavoriteCategory}");
        }
        Console.WriteLine($"  Час виконання: {milliseconds:F3} мс");
        Console.WriteLine();
    }

    // Порівняння часу на різних обсягах даних, щоб побачити, як він залежить від кількості продажів
    static void RunPerformanceComparison()
    {
        Console.WriteLine("Порівняння продуктивності (середній час одного виконання після прогріву, мс)");

        // Ті самі 60 товарів і 80 клієнтів, але різна кількість продажів
        List<SalesAnalytics> datasets = new List<SalesAnalytics>
        {
            new SalesAnalytics(SalesData.Generate(customerCount: 80, saleCount: 1000)),
            new SalesAnalytics(SalesData.Generate(customerCount: 80, saleCount: 10000)),
            new SalesAnalytics(SalesData.Generate(customerCount: 80, saleCount: 100000))
        };

        Console.WriteLine($"  {"Пайплайн",-30} {"1 000",10} {"10 000",10} {"100 000",10}");
        PrintPerformanceRow("1. Топ товарів за категоріями", datasets, a => a.TopProductsByCategory());
        PrintPerformanceRow("2. Дохід за містами", datasets, a => a.RevenueByCity());
        PrintPerformanceRow("3. Продажі за місяцями", datasets, a => a.SalesByMonth());
        PrintPerformanceRow("4. Найкращі клієнти", datasets, a => a.TopCustomers());
    }

    // Один рядок таблиці: середній час пайплайну на кожному наборі даних
    static void PrintPerformanceRow<T>(string name, List<SalesAnalytics> datasets, Func<SalesAnalytics, List<T>> pipeline)
    {
        // Чим більше даних, тим менше повторів, щоб загальний час був розумним
        int[] repetitions = { 200, 20, 3 };

        StringBuilder line = new StringBuilder($"  {name,-30}");
        for (int i = 0; i < datasets.Count; i++)
        {
            SalesAnalytics analytics = datasets[i];

            // Прогрів: перші виконання повільніші через JIT-компіляцію
            for (int j = 0; j < 3; j++)
            {
                pipeline(analytics);
            }

            double average = MeasureAverage(() => pipeline(analytics), repetitions[i]);
            line.Append($" {average,10:F3}");
        }

        Console.WriteLine(line);
    }
}
