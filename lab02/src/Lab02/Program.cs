using Lab02.Services;

namespace Lab02;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Запуск бенчмарков коллекций...");
        Console.WriteLine("Это может занять некоторое время...\n");

        var benchmark = new CollectionBenchmark();
        var results = benchmark.RunBenchmarks();

        var reportGenerator = new ReportGenerator();
        reportGenerator.PrintReport(results);
    }
}
