namespace IndependentWork1;

// Домашній акваріум з рибками
public class Aquarium
{
    // Скільки літрів води потрібно на одну рибку
    private const double LitersPerFish = 10.0;

    // Комфортний діапазон температури для тропічних рибок
    private const double MinComfortTemperature = 24.0;
    private const double MaxComfortTemperature = 28.0;

    // Приватні поля
    private double _volumeLiters;
    private int _fishCount;
    private double _waterTemperature;

    // Об'єм лише для читання, розмір акваріума не змінюється
    public double VolumeLiters
    {
        get { return _volumeLiters; }
    }

    // Кількість рибок лише для читання, змінюється через AddFish
    public int FishCount
    {
        get { return _fishCount; }
    }

    // Температура води, яку можна регулювати нагрівачем
    public double WaterTemperature
    {
        get { return _waterTemperature; }
        set { _waterTemperature = value; }
    }

    // Конструктор
    public Aquarium(double volumeLiters, double waterTemperature)
    {
        _volumeLiters = volumeLiters > 0 ? volumeLiters : LitersPerFish;
        _waterTemperature = waterTemperature;
        _fishCount = 0;
    }

    // Максимальна кількість рибок для цього об'єму
    public int GetMaxFishCount()
    {
        return (int)(_volumeLiters / LitersPerFish);
    }

    // Додає рибок, якщо для них вистачає місця
    public bool AddFish(int count)
    {
        if (count <= 0 || _fishCount + count > GetMaxFishCount())
        {
            return false;
        }

        _fishCount += count;
        return true;
    }

    // Перевірка, чи температура підходить для рибок
    public bool IsTemperatureComfortable()
    {
        return _waterTemperature >= MinComfortTemperature
            && _waterTemperature <= MaxComfortTemperature;
    }

    // Короткий опис стану акваріума
    public string GetStatus()
    {
        string temperatureState = IsTemperatureComfortable()
            ? "температура в нормі"
            : "потрібно відрегулювати температуру";

        return $"{_fishCount} з {GetMaxFishCount()} рибок, вода {_waterTemperature:F1} °C, {temperatureState}";
    }
}
