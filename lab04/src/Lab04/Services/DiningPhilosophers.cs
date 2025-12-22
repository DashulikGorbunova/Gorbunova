using System.Diagnostics;

namespace Lab04.Services;

/// <summary>
/// Задача об обедающих философах
/// Демонстрирует проблему deadlock и способы её решения
/// </summary>
public class DiningPhilosophers
{
    private const int PhilosopherCount = 5;
    private readonly object[] _forks;
    private readonly Thread[] _philosophers;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly bool _useDeadlockVersion;

    public DiningPhilosophers(bool useDeadlockVersion = false)
    {
        _useDeadlockVersion = useDeadlockVersion;
        _forks = new object[PhilosopherCount];
        _philosophers = new Thread[PhilosopherCount];
        _cancellationTokenSource = new CancellationTokenSource();

        for (int i = 0; i < PhilosopherCount; i++)
        {
            _forks[i] = new object();
        }
    }

    /// <summary>
    /// Версия с deadlock - неправильная реализация
    /// Каждый философ сначала берёт левую вилку, затем правую
    /// Это приводит к deadlock, когда все философы берут левую вилку одновременно
    /// </summary>
    private void PhilosopherWithDeadlock(int philosopherId, CancellationToken cancellationToken)
    {
        int leftFork = philosopherId;
        int rightFork = (philosopherId + 1) % PhilosopherCount;

        while (!cancellationToken.IsCancellationRequested)
        {
            // Думает
            Think(philosopherId);

            // Пытается взять вилки (сначала левую, затем правую)
            lock (_forks[leftFork])
            {
                Console.WriteLine($"Философ {philosopherId} взял левую вилку {leftFork}");
                Thread.Sleep(100); // Небольшая задержка увеличивает вероятность deadlock

                lock (_forks[rightFork])
                {
                    Console.WriteLine($"Философ {philosopherId} взял правую вилку {rightFork}");
                    Eat(philosopherId);
                }

                Console.WriteLine($"Философ {philosopherId} положил правую вилку {rightFork}");
            }

            Console.WriteLine($"Философ {philosopherId} положил левую вилку {leftFork}");
        }
    }

    /// <summary>
    /// Версия без deadlock - правильная реализация
    /// Используется упорядочивание ресурсов: философы берут вилки в определённом порядке
    /// Все философы, кроме последнего, берут сначала левую, затем правую вилку
    /// Последний философ берёт сначала правую, затем левую вилку
    /// Это предотвращает циклическое ожидание
    /// </summary>
    private void PhilosopherWithoutDeadlock(int philosopherId, CancellationToken cancellationToken)
    {
        int leftFork = philosopherId;
        int rightFork = (philosopherId + 1) % PhilosopherCount;

        // Упорядочивание ресурсов: последний философ берёт вилки в обратном порядке
        int firstFork = philosopherId == PhilosopherCount - 1 ? rightFork : leftFork;
        int secondFork = philosopherId == PhilosopherCount - 1 ? leftFork : rightFork;

        while (!cancellationToken.IsCancellationRequested)
        {
            // Думает
            Think(philosopherId);

            // Берёт вилки в упорядоченном порядке
            lock (_forks[firstFork])
            {
                Console.WriteLine($"Философ {philosopherId} взял первую вилку {firstFork}");
                Thread.Sleep(50);

                lock (_forks[secondFork])
                {
                    Console.WriteLine($"Философ {philosopherId} взял вторую вилку {secondFork}");
                    Eat(philosopherId);
                }

                Console.WriteLine($"Философ {philosopherId} положил вторую вилку {secondFork}");
            }

            Console.WriteLine($"Философ {philosopherId} положил первую вилку {firstFork}");
        }
    }

    private void Think(int philosopherId)
    {
        Console.WriteLine($"Философ {philosopherId} думает...");
        Thread.Sleep(Random.Shared.Next(500, 1500));
    }

    private void Eat(int philosopherId)
    {
        Console.WriteLine($"Философ {philosopherId} ЕСТ (использует обе вилки)");
        Thread.Sleep(Random.Shared.Next(300, 800));
        Console.WriteLine($"Философ {philosopherId} закончил есть");
    }

    public void Start()
    {
        Console.WriteLine($"\n{new string('=', 60)}");
        Console.WriteLine(_useDeadlockVersion 
            ? "ОБЕДАЮЩИЕ ФИЛОСОФЫ (версия с DEADLOCK)" 
            : "ОБЕДАЮЩИЕ ФИЛОСОФЫ (версия БЕЗ deadlock)");
        Console.WriteLine($"{new string('=', 60)}\n");

        for (int i = 0; i < PhilosopherCount; i++)
        {
            int id = i;
            _philosophers[i] = new Thread(() =>
            {
                if (_useDeadlockVersion)
                {
                    PhilosopherWithDeadlock(id, _cancellationTokenSource.Token);
                }
                else
                {
                    PhilosopherWithoutDeadlock(id, _cancellationTokenSource.Token);
                }
            })
            {
                Name = $"Philosopher-{id}"
            };
            _philosophers[i].Start();
        }

        Console.WriteLine("\nНажмите Enter, чтобы остановить философов...");
        Console.ReadLine();

        _cancellationTokenSource.Cancel();
        Console.WriteLine("\nОстановка всех философов...");

        foreach (var philosopher in _philosophers)
        {
            philosopher.Join(2000);
        }

        Console.WriteLine("Все философы остановлены.\n");
    }
}

