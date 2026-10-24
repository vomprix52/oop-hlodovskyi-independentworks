using System.Globalization;
using System.Text;

namespace IndependentWork12;

class Program
{
    static void Main(string[] args)
    {
        // Коректне відображення українських літер і формату чисел
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        Console.WriteLine("Частина 1. Продуктивність LINQ і PLINQ");
        Console.WriteLine();
        PerformanceExperiment.Run(new[] { 1_000_000, 5_000_000, 10_000_000 });

        Console.WriteLine("Частина 2. Побічні ефекти в PLINQ");
        Console.WriteLine();
        SideEffectsDemo.Run();
    }
}
