namespace IndependentWork12;

// Операції різної складності для кожного елемента колекції
public static class Operations
{
    /// <summary>
    /// Генерує список випадкових чисел; фіксоване зерно дає однакові дані при кожному запуску.
    /// </summary>
    public static List<int> GenerateData(int count, int seed = 12)
    {
        Random random = new Random(seed);
        List<int> data = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            data.Add(random.Next(1, 10_000_000));
        }

        return data;
    }

    /// <summary>
    /// Дешева операція: остача від ділення. Займає кілька наносекунд.
    /// </summary>
    public static bool IsDivisibleByThree(int number)
    {
        return number % 3 == 0;
    }

    /// <summary>
    /// Середня операція: кілька математичних функцій з плаваючою комою.
    /// </summary>
    public static double MathTransform(int number)
    {
        double x = number;
        return Math.Sqrt(x) * Math.Sin(x) + Math.Log(x + 1) * Math.Cos(x / 1000.0);
    }

    /// <summary>
    /// Дорога операція: перевірка на просте число перебором дільників до кореня.
    /// Для великого простого числа це близько 1500 ділень.
    /// </summary>
    public static bool IsPrime(int number)
    {
        if (number < 2)
        {
            return false;
        }

        if (number % 2 == 0)
        {
            return number == 2;
        }

        int limit = (int)Math.Sqrt(number);
        for (int divisor = 3; divisor <= limit; divisor += 2)
        {
            if (number % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Сума цифр числа, використовується як Select для простих чисел.
    /// </summary>
    public static int DigitSum(int number)
    {
        int sum = 0;
        while (number > 0)
        {
            sum += number % 10;
            number /= 10;
        }

        return sum;
    }
}
