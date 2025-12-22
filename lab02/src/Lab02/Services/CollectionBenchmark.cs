using Lab02.Models;
using Lab02.Services.Benchmarks;

namespace Lab02.Services;

public class CollectionBenchmark
{
    public BenchmarkResult RunBenchmarks()
    {
        var results = new BenchmarkResult();

        results.ListResults = new ListBenchmark().Run();
        results.LinkedListResults = new LinkedListBenchmark().Run();
        results.QueueResults = new QueueBenchmark().Run();
        results.StackResults = new StackBenchmark().Run();
        results.ImmutableListResults = new ImmutableListBenchmark().Run();

        return results;
    }
}
