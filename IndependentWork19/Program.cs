using System.Text;

namespace IndependentWork19;

class Program
{
    static async Task Main(string[] args)
    {
        // Коректне відображення українських літер
        Console.OutputEncoding = Encoding.UTF8;

        DemoCustomException();

        PollyScenarios.RetryScenario();
        PollyScenarios.CircuitBreakerScenario();
        await PollyScenarios.TimeoutFallbackScenario();
    }

    // Демонстрація власного винятку SeatsNotAvailableException
    static void DemoCustomException()
    {
        Console.WriteLine("Власний виняток SeatsNotAvailableException");

        CinemaSession session = new CinemaSession("Дюна 2, 18:00", freeSeats: 5);
        string[] requests = { "3", "4", "два", "-1" };

        foreach (string request in requests)
        {
            try
            {
                session.BookSeatsFromText(request);
                Console.WriteLine($"  Запит \"{request}\": заброньовано, вільних місць залишилось {session.FreeSeats}");
            }
            // Спочатку найбільш конкретні винятки, в кінці найзагальніший
            catch (SeatsNotAvailableException exception) when (exception.InnerException is not null)
            {
                Console.WriteLine($"  Запит \"{request}\": {exception.Message}");
                Console.WriteLine($"    причина ({exception.InnerException.GetType().Name}): {exception.InnerException.Message}");
            }
            catch (SeatsNotAvailableException exception)
            {
                Console.WriteLine($"  Запит \"{request}\": {exception.Message}");
                Console.WriteLine($"    сеанс: {exception.SessionName}, запитано: {exception.RequestedSeats}, вільно: {exception.AvailableSeats}");
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine($"  Запит \"{request}\": некоректний аргумент ({exception.GetType().Name}): {exception.Message}");
            }
            finally
            {
                // Виконується завжди, незалежно від того, чи був виняток
                Console.WriteLine($"    запит \"{request}\" оброблено");
            }
        }

        Console.WriteLine();
    }
}
