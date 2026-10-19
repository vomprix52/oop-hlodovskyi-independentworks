using System.Globalization;

namespace IndependentWork1;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер і формату чисел
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        DemoFitnessTracker();
        DemoParkingTicket();
        DemoAquarium();
    }

    // Демонстрація роботи фітнес-браслета
    static void DemoFitnessTracker()
    {
        Console.WriteLine("Фітнес-браслет");

        FitnessTracker tracker = new FitnessTracker("Олена", 8000);
        Console.WriteLine($"Власник: {tracker.OwnerName}, денна ціль: {tracker.DailyStepGoal} кроків");

        tracker.AddSteps(3500);
        tracker.AddSteps(2700);
        bool wrongStepsAdded = tracker.AddSteps(-100);
        Console.WriteLine($"Спроба додати -100 кроків: {(wrongStepsAdded ? "прийнято" : "відхилено")}");

        Console.WriteLine($"Пройдено кроків: {tracker.StepsToday}");
        Console.WriteLine($"Виконання цілі: {tracker.GetGoalProgress():F1} %");
        Console.WriteLine($"Ціль досягнуто: {(tracker.IsGoalReached() ? "так" : "ні")}");

        tracker.AddSteps(2000);
        Console.WriteLine($"Після вечірньої прогулянки: {tracker.StepsToday} кроків, ціль досягнуто: {(tracker.IsGoalReached() ? "так" : "ні")}");
        Console.WriteLine($"Спалено приблизно {tracker.GetBurnedCalories():F0} ккал");
        Console.WriteLine();
    }

    // Демонстрація роботи талона на стоянку
    static void DemoParkingTicket()
    {
        Console.WriteLine("Талон на автостоянку");

        DateTime entryTime = new DateTime(2026, 10, 19, 9, 15, 0);
        DateTime exitTime = new DateTime(2026, 10, 19, 12, 40, 0);
        ParkingTicket ticket = new ParkingTicket("ВК 1234 АР", entryTime, 25m);

        Console.WriteLine($"Автомобіль {ticket.PlateNumber} заїхав о {ticket.EntryTime:HH:mm}, виїхав о {exitTime:HH:mm}");
        Console.WriteLine($"Тариф: {ticket.HourlyRate:F2} грн за годину");
        Console.WriteLine($"Оплачуваних годин: {ticket.GetBilledHours(exitTime)}");
        Console.WriteLine($"До сплати: {ticket.CalculateCost(exitTime):F2} грн");

        ticket.HourlyRate = 30m;
        Console.WriteLine($"Після зміни тарифу на {ticket.HourlyRate:F2} грн до сплати: {ticket.CalculateCost(exitTime):F2} грн");

        Console.WriteLine($"Перша оплата: {(ticket.Pay() ? "успішна" : "відхилена")}");
        Console.WriteLine($"Повторна оплата: {(ticket.Pay() ? "успішна" : "відхилена, талон уже оплачено")}");
        Console.WriteLine($"Талон оплачено: {(ticket.IsPaid ? "так" : "ні")}");
        Console.WriteLine();
    }

    // Демонстрація роботи акваріума
    static void DemoAquarium()
    {
        Console.WriteLine("Акваріум");

        Aquarium aquarium = new Aquarium(60, 22.5);
        Console.WriteLine($"Об'єм: {aquarium.VolumeLiters} л, максимум рибок: {aquarium.GetMaxFishCount()}");

        Console.WriteLine($"Додати 4 рибки: {(aquarium.AddFish(4) ? "додано" : "немає місця")}");
        Console.WriteLine($"Додати ще 3 рибки: {(aquarium.AddFish(3) ? "додано" : "немає місця")}");
        Console.WriteLine($"Стан: {aquarium.GetStatus()}");

        aquarium.WaterTemperature = 25.5;
        Console.WriteLine($"Після ввімкнення нагрівача: {aquarium.GetStatus()}");
    }
}
