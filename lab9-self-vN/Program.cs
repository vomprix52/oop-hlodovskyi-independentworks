using System.Globalization;
using System.Text;

namespace lab9_self_vN;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер і формату чисел
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        DemoNumbers();
        DemoStrings();
        DemoMovies();
        DemoDelegateForms();
        DemoCompareWithLinq();
    }

    // Колекція 1: List<int>, температура повітря за тиждень
    static void DemoNumbers()
    {
        Console.WriteLine("Колекція 1. List<int>: температура за тиждень");

        List<int> temperatures = new List<int> { 12, 15, 9, 7, 14, 18, 11 };
        Console.WriteLine($"Температури: {string.Join(", ", temperatures)}");

        IEnumerable<double> fahrenheit = CollectionUtils.Map(temperatures, t => t * 9.0 / 5 + 32);
        // Десяткові числа пишуться з комою, тому між ними крапка з комою
        Console.WriteLine($"Map (у Фаренгейтах): {string.Join("; ", fahrenheit)}");

        IEnumerable<int> warmDays = CollectionUtils.Filter(temperatures, t => t >= 14);
        Console.WriteLine($"Filter (від 14 °C): {string.Join(", ", warmDays)}");

        // Ключем є відстань від комфортних 20 °C зі знаком мінус, тож максимум це найближчий день
        int closestToComfort = CollectionUtils.MaxBy(temperatures, t => -Math.Abs(t - 20));
        Console.WriteLine($"MaxBy (найближче до 20 °C): {closestToComfort}");

        // Лямбда змінює зовнішню змінну sum, тобто захоплює її (замикання)
        int sum = 0;
        CollectionUtils.ForEach(temperatures, t => sum += t);
        Console.WriteLine($"ForEach (сума для середнього): {sum}, середня температура {(double)sum / temperatures.Count:F1} °C");
        Console.WriteLine();
    }

    // Колекція 2: string[], масив теж реалізує IEnumerable<string>
    static void DemoStrings()
    {
        Console.WriteLine("Колекція 2. string[]: міста України");

        string[] cities = { "Рівне", "Київ", "Івано-Франківськ", "Луцьк", "Запоріжжя", "Ужгород" };
        Console.WriteLine($"Міста: {string.Join(", ", cities)}");

        IEnumerable<string> upper = CollectionUtils.Map(cities, c => c.ToUpper());
        Console.WriteLine($"Map (великими літерами): {string.Join(", ", upper)}");

        IEnumerable<int> lengths = CollectionUtils.Map(cities, c => c.Length);
        Console.WriteLine($"Map (довжина назв): {string.Join(", ", lengths)}");

        IEnumerable<string> shortNames = CollectionUtils.Filter(cities, c => c.Length <= 5);
        Console.WriteLine($"Filter (не довше 5 літер): {string.Join(", ", shortNames)}");

        string longest = CollectionUtils.MaxBy(cities, c => c.Length);
        Console.WriteLine($"MaxBy (найдовша назва): {longest}");

        Console.WriteLine("ForEach (нумерований список):");
        int number = 1;
        CollectionUtils.ForEach(cities, c => Console.WriteLine($"  {number++}. {c}"));
        Console.WriteLine();
    }

    // Колекція 3: List<Movie>, власний клас
    static void DemoMovies()
    {
        Console.WriteLine("Колекція 3. List<Movie>: каталог фільмів");

        List<Movie> movies = CreateMovies();
        CollectionUtils.ForEach(movies, m => Console.WriteLine($"  {m}"));

        IEnumerable<string> titles = CollectionUtils.Map(movies, m => m.Title);
        Console.WriteLine($"Map (лише назви): {string.Join(", ", titles)}");

        Movie bestRated = CollectionUtils.MaxBy(movies, m => m.Rating);
        Console.WriteLine($"MaxBy (найвищий рейтинг): {bestRated.Title}, {bestRated.Rating:F1}");

        Movie longestMovie = CollectionUtils.MaxBy(movies, m => m.DurationMinutes);
        Console.WriteLine($"MaxBy (найдовший): {longestMovie.Title}, {longestMovie.GetDurationText()}");

        // Лямбда з блоком коду, коли умова складається з кількох кроків
        IEnumerable<Movie> eveningMovies = CollectionUtils.Filter(movies, m =>
        {
            bool isRecent = m.Year >= 2020;
            bool fitsInEvening = m.DurationMinutes <= 120;
            return isRecent && fitsInEvening;
        });

        Console.WriteLine("Filter (новіші за 2020 і не довші за 2 години):");
        CollectionUtils.ForEach(eveningMovies, m => Console.WriteLine($"  {m.Title} ({m.Year}), {m.GetDurationText()}"));
        Console.WriteLine();
    }

    // Різні способи передати поведінку в той самий метод Filter
    static void DemoDelegateForms()
    {
        Console.WriteLine("Різні способи передати делегат у Filter");

        List<Movie> movies = CreateMovies();

        // 1. Звичайний іменований метод
        IEnumerable<Movie> byMethod = CollectionUtils.Filter(movies, IsHighlyRated);
        Console.WriteLine($"Іменований метод IsHighlyRated: {string.Join(", ", CollectionUtils.Map(byMethod, m => m.Title))}");

        // 2. Анонімний метод, старий синтаксис з C# 2.0
        IEnumerable<Movie> byAnonymous = CollectionUtils.Filter(movies, delegate (Movie m) { return m.Rating >= 8.0; });
        Console.WriteLine($"Анонімний метод delegate: {string.Join(", ", CollectionUtils.Map(byAnonymous, m => m.Title))}");

        // 3. Лямбда-вираз, найкоротший запис
        IEnumerable<Movie> byLambda = CollectionUtils.Filter(movies, m => m.Rating >= 8.0);
        Console.WriteLine($"Лямбда-вираз: {string.Join(", ", CollectionUtils.Map(byLambda, m => m.Title))}");

        // 4. Делегат у змінній можна використати кілька разів
        Predicate<Movie> isLong = m => m.DurationMinutes > 130;
        Console.WriteLine($"Делегат у змінній isLong: {string.Join(", ", CollectionUtils.Map(CollectionUtils.Filter(movies, isLong), m => m.Title))}");
        Console.WriteLine();
    }

    // Метод з тією самою сигнатурою, що й Predicate<Movie>
    static bool IsHighlyRated(Movie movie)
    {
        return movie.Rating >= 8.0;
    }

    // Порівняння результатів моїх методів і LINQ на тих самих даних
    static void DemoCompareWithLinq()
    {
        Console.WriteLine("Порівняння з LINQ");

        List<Movie> movies = CreateMovies();

        List<string> myMap = CollectionUtils.Map(movies, m => m.Title).ToList();
        List<string> linqSelect = movies.Select(m => m.Title).ToList();
        Console.WriteLine($"Map і Select дають однаковий результат: {(myMap.SequenceEqual(linqSelect) ? "так" : "ні")}");

        List<Movie> myFilter = CollectionUtils.Filter(movies, m => m.Year >= 2020).ToList();
        List<Movie> linqWhere = movies.Where(m => m.Year >= 2020).ToList();
        Console.WriteLine($"Filter і Where дають однаковий результат: {(myFilter.SequenceEqual(linqWhere) ? "так" : "ні")}");

        Movie myMax = CollectionUtils.MaxBy(movies, m => m.Rating);
        Movie? linqMax = movies.MaxBy(m => m.Rating);
        Console.WriteLine($"MaxBy і LINQ MaxBy знаходять той самий фільм: {(ReferenceEquals(myMax, linqMax) ? "так" : "ні")}");

        // Різниця: мій Filter виконується одразу, а LINQ Where лише тоді, коли результат перебирають
        List<int> numbers = new List<int> { 1, 2, 3 };
        IEnumerable<int> myResult = CollectionUtils.Filter(numbers, n => n > 1);
        IEnumerable<int> linqResult = numbers.Where(n => n > 1);
        numbers.Add(4);
        Console.WriteLine("Після створення фільтра до списку додали 4:");
        Console.WriteLine($"  мій Filter: {string.Join(", ", myResult)}");
        Console.WriteLine($"  LINQ Where: {string.Join(", ", linqResult)}");

        // Ще одна різниця: на порожній колекції мій MaxBy викидає виняток
        try
        {
            CollectionUtils.MaxBy(new List<Movie>(), m => m.Rating);
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine($"MaxBy на порожній колекції: {exception.Message}");
        }
    }

    // Тестові дані для демонстрацій
    static List<Movie> CreateMovies()
    {
        return new List<Movie>
        {
            new Movie("Зоряний маршрут", 2021, 8.3, 142),
            new Movie("Тиха гавань", 2018, 7.4, 98),
            new Movie("Останній потяг", 2023, 8.7, 115),
            new Movie("Код доступу", 2022, 6.9, 104),
            new Movie("Літо в Карпатах", 2016, 7.9, 131)
        };
    }
}
