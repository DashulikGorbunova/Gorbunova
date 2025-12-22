using System.Collections.Concurrent;

namespace Lab04.Services;

/// <summary>
/// Задача производитель-потребитель
/// Демонстрирует два подхода: BlockingCollection и SemaphoreSlim + lock
/// </summary>
public class ProducerConsumer
{
    private readonly int _bufferSize;
    private readonly int _producerCount;
    private readonly int _consumerCount;
    private readonly bool _useBlockingCollection;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ProducerConsumer(int bufferSize = 5, int producerCount = 3, int consumerCount = 2, bool useBlockingCollection = true)
    {
        _bufferSize = bufferSize;
        _producerCount = producerCount;
        _consumerCount = consumerCount;
        _useBlockingCollection = useBlockingCollection;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Реализация с использованием BlockingCollection (готовое решение)
    /// </summary>
    public void RunWithBlockingCollection()
    {
        Console.WriteLine($"\n{new string('=', 60)}");
        Console.WriteLine("ПРОИЗВОДИТЕЛЬ-ПОТРЕБИТЕЛЬ (BlockingCollection)");
        Console.WriteLine($"{new string('=', 60)}\n");
        Console.WriteLine($"Размер буфера: {_bufferSize}, Производителей: {_producerCount}, Потребителей: {_consumerCount}\n");

        var buffer = new BlockingCollection<int>(_bufferSize);
        var producers = new List<Thread>();
        var consumers = new List<Thread>();

        // Запускаем производителей
        for (int i = 0; i < _producerCount; i++)
        {
            int producerId = i + 1;
            var producer = new Thread(() =>
            {
                int item = 1;
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        buffer.Add(item, _cancellationTokenSource.Token);
                        Console.WriteLine($"Производитель {producerId} произвёл товар {item}");
                        item++;
                        Thread.Sleep(Random.Shared.Next(500, 1500));
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                Console.WriteLine($"Производитель {producerId} завершил работу");
            })
            {
                Name = $"Producer-{producerId}"
            };
            producer.Start();
            producers.Add(producer);
        }

        // Запускаем потребителей
        for (int i = 0; i < _consumerCount; i++)
        {
            int consumerId = i + 1;
            var consumer = new Thread(() =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        int item = buffer.Take(_cancellationTokenSource.Token);
                        Console.WriteLine($"  Потребитель {consumerId} забрал товар {item}");
                        Thread.Sleep(Random.Shared.Next(800, 2000));
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                Console.WriteLine($"Потребитель {consumerId} завершил работу");
            })
            {
                Name = $"Consumer-{consumerId}"
            };
            consumer.Start();
            consumers.Add(consumer);
        }

        Console.WriteLine("\nНажмите Enter, чтобы остановить симуляцию...");
        Console.ReadLine();

        _cancellationTokenSource.Cancel();
        buffer.CompleteAdding();

        Console.WriteLine("\nОстановка всех потоков...");
        foreach (var producer in producers)
        {
            producer.Join(2000);
        }

        foreach (var consumer in consumers)
        {
            consumer.Join(2000);
        }

        Console.WriteLine("Симуляция завершена.\n");
    }

    /// <summary>
    /// Реализация с использованием SemaphoreSlim + lock (ручная синхронизация)
    /// </summary>
    public void RunWithSemaphoreAndLock()
    {
        Console.WriteLine($"\n{new string('=', 60)}");
        Console.WriteLine("ПРОИЗВОДИТЕЛЬ-ПОТРЕБИТЕЛЬ (SemaphoreSlim + lock)");
        Console.WriteLine($"{new string('=', 60)}\n");
        Console.WriteLine($"Размер буфера: {_bufferSize}, Производителей: {_producerCount}, Потребителей: {_consumerCount}\n");

        var buffer = new Queue<int>();
        var bufferLock = new object();
        var emptySlots = new SemaphoreSlim(_bufferSize, _bufferSize); // Свободные места в буфере
        var filledSlots = new SemaphoreSlim(0, _bufferSize); // Заполненные места в буфере

        var producers = new List<Thread>();
        var consumers = new List<Thread>();

        // Запускаем производителей
        for (int i = 0; i < _producerCount; i++)
        {
            int producerId = i + 1;
            var producer = new Thread(() =>
            {
                int item = 1;
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Ждём свободного места в буфере
                        emptySlots.Wait(_cancellationTokenSource.Token);

                        lock (bufferLock)
                        {
                            buffer.Enqueue(item);
                            Console.WriteLine($"Производитель {producerId} произвёл товар {item} (в буфере: {buffer.Count}/{_bufferSize})");
                        }

                        // Сигнализируем, что появился товар
                        filledSlots.Release();
                        item++;
                        Thread.Sleep(Random.Shared.Next(500, 1500));
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                Console.WriteLine($"Производитель {producerId} завершил работу");
            })
            {
                Name = $"Producer-{producerId}"
            };
            producer.Start();
            producers.Add(producer);
        }

        // Запускаем потребителей
        for (int i = 0; i < _consumerCount; i++)
        {
            int consumerId = i + 1;
            var consumer = new Thread(() =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Ждём появления товара в буфере
                        filledSlots.Wait(_cancellationTokenSource.Token);

                        int item;
                        lock (bufferLock)
                        {
                            item = buffer.Dequeue();
                            Console.WriteLine($"  Потребитель {consumerId} забрал товар {item} (в буфере: {buffer.Count}/{_bufferSize})");
                        }

                        // Сигнализируем, что освободилось место
                        emptySlots.Release();
                        Thread.Sleep(Random.Shared.Next(800, 2000));
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                Console.WriteLine($"Потребитель {consumerId} завершил работу");
            })
            {
                Name = $"Consumer-{consumerId}"
            };
            consumer.Start();
            consumers.Add(consumer);
        }

        Console.WriteLine("\nНажмите Enter, чтобы остановить симуляцию...");
        Console.ReadLine();

        _cancellationTokenSource.Cancel();

        Console.WriteLine("\nОстановка всех потоков...");
        foreach (var producer in producers)
        {
            producer.Join(2000);
        }

        foreach (var consumer in consumers)
        {
            consumer.Join(2000);
        }

        Console.WriteLine("Симуляция завершена.\n");
    }

    public void Start()
    {
        if (_useBlockingCollection)
        {
            RunWithBlockingCollection();
        }
        else
        {
            RunWithSemaphoreAndLock();
        }
    }
}

