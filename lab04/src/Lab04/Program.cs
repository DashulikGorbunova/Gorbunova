using Lab04.Services;

namespace Lab04;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Лабораторная работа 4: Синхронизация потоков");
        Console.WriteLine("Изучение deadlock, starvation и проблем синхронизации\n");

        while (true)
        {
            Console.WriteLine("Выберите задачу:");
            Console.WriteLine("1. Обедающие философы (с deadlock)");
            Console.WriteLine("2. Обедающие философы (без deadlock)");
            Console.WriteLine("3. Спящий парикмахер");
            Console.WriteLine("4. Производитель-Потребитель (BlockingCollection)");
            Console.WriteLine("5. Производитель-Потребитель (SemaphoreSlim + lock)");
            Console.WriteLine("0. Выход");
            Console.Write("\nВаш выбор: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var philosophersWithDeadlock = new DiningPhilosophers(useDeadlockVersion: true);
                    philosophersWithDeadlock.Start();
                    break;

                case "2":
                    var philosophersWithoutDeadlock = new DiningPhilosophers(useDeadlockVersion: false);
                    philosophersWithoutDeadlock.Start();
                    break;

                case "3":
                    var barber = new SleepingBarber(maxSeats: 5);
                    barber.Start();
                    break;

                case "4":
                    var producerConsumer1 = new ProducerConsumer(
                        bufferSize: 5,
                        producerCount: 3,
                        consumerCount: 2,
                        useBlockingCollection: true);
                    producerConsumer1.Start();
                    break;

                case "5":
                    var producerConsumer2 = new ProducerConsumer(
                        bufferSize: 5,
                        producerCount: 3,
                        consumerCount: 2,
                        useBlockingCollection: false);
                    producerConsumer2.Start();
                    break;

                case "0":
                    Console.WriteLine("Выход из программы.");
                    return;

                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.\n");
                    break;
            }

            Console.WriteLine();
        }
    }
}

