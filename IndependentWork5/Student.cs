namespace IndependentWork5;

// Студент коледжу
public class Student
{
    // Приватні поля
    private string _fullName;
    private string _group;
    private double _averageGrade;

    public string FullName
    {
        get { return _fullName; }
    }

    public string Group
    {
        get { return _group; }
    }

    public double AverageGrade
    {
        get { return _averageGrade; }
    }

    // Конструктор
    public Student(string fullName, string group, double averageGrade)
    {
        _fullName = fullName;
        _group = group;
        _averageGrade = averageGrade;
    }

    // Перевизначення методу з System.Object
    public override string ToString()
    {
        return $"Студент {FullName}, група {Group}, середній бал {AverageGrade:F1}";
    }
}
