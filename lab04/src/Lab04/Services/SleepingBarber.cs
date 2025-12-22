using System.Collections.Concurrent;

namespace Lab04.Services;

/// <summary>
/// Задача о спящем парикмахере
/// Демонстрирует использование SemaphoreSlim и Mutex для синхронизации потоков
/// </summary>
public class SleepingBarber
{
    private readonly SemaphoreSlim _barberSemaphore; // Семафор для парикмахера
    private readonly SemaphoreSlim _customerSemaphore; // Семафор для клиентов
    private readonly Mutex _seatMutex; // Мьютекс для защиты критической секции (очередь)
    private readonly ConcurrentQueue<int> _waitingRoom; // Очередь клиентов
    private readonly int _maxSeats;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Thread? _barberThread;

    public SleepingBarber(int maxSeats = 5)
    {
        _maxSeats = maxSeats;
        _barberSemaphore = new SemaphoreSlim(0, 1); // Парикмахер изначально спит
        _customerSemaphore = new SemaphoreSlim(0); // Клиенты ждут
        _seatMutex = new Mutex();
        _waitingRoom = new ConcurrentQueue<int>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Парикмахер работает: если клиентов нет - спит, если клиент пришёл - работает
    /// </summary>
    private void BarberWork(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Парикмахер спит...");
            
            // Ждёт клиента (блокируется, пока не появится клиент)
            _barberSemaphore.Wait(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                break;

            // Пытается взять клиента из очереди
            _seatMutex.WaitOne();
            bool hasCustomer = _waitingRoom.TryDequeue(out int customerId);
            _seatMutex.ReleaseMutex();

            if (hasCustomer)
            {
                Console.WriteLine($"Парикмахер начал стричь клиента {customerId}");
                Thread.Sleep(Random.Shared.Next(1000, 3000)); // Время стрижки
                Console.WriteLine($"Парикмахер закончил стричь клиента {customerId}");
                
                // Сигнализирует клиенту, что стрижка завершена
                _customerSemaphore.Release();
            }
        }

        Console.WriteLine("Парикмахер ушёл домой.");
    }

    /// <summary>
    /// Клиент приходит в парикмахерскую
    /// </summary>
    public void CustomerArrives(int customerId)
    {
        _seatMutex.WaitOne();
        
        if (_waitingRoom.Count < _maxSeats)
        {
            _waitingRoom.Enqueue(customerId);
            Console.WriteLine($"Клиент {customerId} сел в очередь. Мест в очереди: {_waitingRoom.Count}/{_maxSeats}");
            
            // Будит парикмахера, если он спит
            if (_barberSemaphore.CurrentCount == 0)
            {
                _barberSemaphore.Release();
            }
            
            _seatMutex.ReleaseMutex();

            // Ждёт своей очереди
            _customerSemaphore.Wait(_cancellationTokenSource.Token);
            Console.WriteLine($"Клиент {customerId} ушёл после стрижки");
        }
        else
        {
            _seatMutex.ReleaseMutex();
            Console.WriteLine($"Клиент {customerId} ушёл - нет свободных мест (очередь полная)");
        }
    }

    public void Start()
    {
        Console.WriteLine($"\n{new string('=', 60)}");
        Console.WriteLine("СПЯЩИЙ ПАРИКМАХЕР");
        Console.WriteLine($"{new string('=', 60)}\n");
        Console.WriteLine($"Максимальное количество мест в очереди: {_maxSeats}\n");

        // Запускаем парикмахера
        _barberThread = new Thread(() => BarberWork(_cancellationTokenSource.Token))
        {
            Name = "Barber",
            IsBackground = false
        };
        _barberThread.Start();

        // Симулируем приход клиентов
        int customerId = 1;
        var customerThreads = new List<Thread>();

        Console.WriteLine("Начинаем симуляцию. Клиенты будут приходить каждые 0.5-2 секунды...");
        Console.WriteLine("Нажмите Enter, чтобы остановить симуляцию...\n");

        var customerGenerator = new Thread(() =>
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                int id = customerId++;
                var customerThread = new Thread(() => CustomerArrives(id))
                {
                    Name = $"Customer-{id}",
                    IsBackground = true
                };
                customerThread.Start();
                customerThreads.Add(customerThread);

                Thread.Sleep(Random.Shared.Next(500, 2000));
            }
        })
        {
            IsBackground = true
        };

        customerGenerator.Start();

        Console.ReadLine();
        _cancellationTokenSource.Cancel();

        // Будим парикмахера, чтобы он мог завершить работу
        try
        {
            _barberSemaphore.Release();
        }
        catch (SemaphoreFullException)
        {
            // Игнорируем, если семафор уже полный
        }

        Console.WriteLine("\nОстановка симуляции...");
        _barberThread?.Join(3000);

        foreach (var thread in customerThreads)
        {
            thread.Join(1000);
        }

        Console.WriteLine("Симуляция завершена.\n");
    }
}

