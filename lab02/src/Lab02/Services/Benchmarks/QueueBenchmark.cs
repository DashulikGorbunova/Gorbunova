using Lab02.Models;
using Lab02.Services;

namespace Lab02.Services.Benchmarks;

public class QueueBenchmark
{
    private const int CollectionSize = 100_000;

    public CollectionOperationResults Run()
    {
        var results = new CollectionOperationResults { CollectionName = "Queue<T>" };

        results.AddToEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var queue = new Queue<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                queue.Enqueue(j);
            }
        });

        results.AddToStart = -1;
        results.AddToMiddle = -1;

        results.RemoveFromStart = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var queue = new Queue<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                queue.Enqueue(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                queue.Dequeue();
            }
        });

        results.RemoveFromEnd = -1;
        results.RemoveFromMiddle = -1;

        results.Search = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var queue = new Queue<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                queue.Enqueue(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                _ = queue.Contains(j * 100);
            }
        });

        results.GetByIndex = -1;

        return results;
    }
}
