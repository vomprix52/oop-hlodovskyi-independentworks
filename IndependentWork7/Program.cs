namespace IndependentWork7;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Кеш прогнозів погоди: ключ це місто, значення це прогноз
        Cache<string, string> weatherCache = new Cache<string, string>(3);
        Console.WriteLine($"Створено кеш на {weatherCache.Capacity} елементи");

        weatherCache.Add("Київ", "+12 °C, хмарно");
        weatherCache.Add("Львів", "+9 °C, дощ");
        weatherCache.Add("Рівне", "+10 °C, сонячно");
        Console.WriteLine($"Додано 3 міста, елементів у кеші: {weatherCache.Count}");

        Console.WriteLine($"Погода в Рівному: {weatherCache.Get("Рівне")}");
        Console.WriteLine();

        // Кеш повний, тому новий елемент витісняє найстаріший (Київ)
        Console.WriteLine("Додаємо Одесу, кеш уже повний");
        weatherCache.Add("Одеса", "+15 °C, вітряно");
        Console.WriteLine($"Елементів у кеші: {weatherCache.Count}");
        PrintCity(weatherCache, "Київ");
        PrintCity(weatherCache, "Львів");
        PrintCity(weatherCache, "Рівне");
        PrintCity(weatherCache, "Одеса");
        Console.WriteLine();

        // Наступним витісняється Львів, бо він тепер найстаріший
        Console.WriteLine("Додаємо Харків");
        weatherCache.Add("Харків", "+11 °C, туман");
        PrintCity(weatherCache, "Львів");
        PrintCity(weatherCache, "Харків");
        Console.WriteLine();

        // Запит до відсутнього ключа викидає KeyNotFoundException
        Console.WriteLine("Спроба отримати погоду в Києві через Get:");
        try
        {
            weatherCache.Get("Київ");
        }
        catch (KeyNotFoundException exception)
        {
            Console.WriteLine($"  Виняток KeyNotFoundException: {exception.Message}");
        }
    }

    // Виводить прогноз, якщо місто є в кеші
    static void PrintCity(Cache<string, string> cache, string city)
    {
        if (cache.ContainsKey(city))
        {
            Console.WriteLine($"  {city}: {cache.Get(city)}");
        }
        else
        {
            Console.WriteLine($"  {city}: немає в кеші");
        }
    }
}
