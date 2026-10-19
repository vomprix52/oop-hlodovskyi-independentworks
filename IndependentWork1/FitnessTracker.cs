namespace IndependentWork1;

// Фітнес-браслет, що рахує кроки власника за день
public class FitnessTracker
{
    // Середня кількість калорій, що спалюється за один крок
    private const double CaloriesPerStep = 0.04;

    // Приватні поля
    private string _ownerName;
    private int _dailyStepGoal;
    private int _stepsToday;

    // Ім'я власника браслета
    public string OwnerName
    {
        get { return _ownerName; }
        set { _ownerName = value; }
    }

    // Денна ціль; від'ємне або нульове значення не приймається
    public int DailyStepGoal
    {
        get { return _dailyStepGoal; }
        set
        {
            if (value > 0)
            {
                _dailyStepGoal = value;
            }
        }
    }

    // Кількість кроків лише для читання, змінюється тільки через AddSteps
    public int StepsToday
    {
        get { return _stepsToday; }
    }

    // Конструктор
    public FitnessTracker(string ownerName, int dailyStepGoal)
    {
        _ownerName = ownerName;
        _dailyStepGoal = dailyStepGoal > 0 ? dailyStepGoal : 10000;
        _stepsToday = 0;
    }

    // Додає кроки, повертає false для некоректного значення
    public bool AddSteps(int steps)
    {
        if (steps <= 0)
        {
            return false;
        }

        _stepsToday += steps;
        return true;
    }

    // Відсоток виконання денної цілі
    public double GetGoalProgress()
    {
        return (double)_stepsToday / _dailyStepGoal * 100;
    }

    // Перевірка, чи досягнуто ціль
    public bool IsGoalReached()
    {
        return _stepsToday >= _dailyStepGoal;
    }

    // Приблизна кількість спалених калорій
    public double GetBurnedCalories()
    {
        return _stepsToday * CaloriesPerStep;
    }
}
