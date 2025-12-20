
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== ДВА ПОТОКА ===");
        
        
        Thread thread1 = new Thread(() =>
        {
            for (int i = 1; i <= 100; i++)
            {
                Console.WriteLine($"Цифра: {i}");
                Thread.Sleep(50); 
            }
        });
        
        Thread thread2 = new Thread(() =>
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                Console.WriteLine($"Буква: {c}");
                Thread.Sleep(50); 
            }
        });
        
        thread1.Start();
        thread2.Start();
        
        thread1.Join();
        thread2.Join();
        
        Console.WriteLine("\nГотово!");
    }
}