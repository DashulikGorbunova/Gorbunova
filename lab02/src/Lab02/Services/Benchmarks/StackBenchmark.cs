using Lab02.Models;
using Lab02.Services;

namespace Lab02.Services.Benchmarks;

public class StackBenchmark
{
    private const int CollectionSize = 100_000;

    public CollectionOperationResults Run()
    {
        var results = new CollectionOperationResults { CollectionName = "Stack<T>" };

        results.AddToEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var stack = new Stack<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                stack.Push(j);
            }
        });

        results.AddToStart = -1;
        results.AddToMiddle = -1;
        results.RemoveFromStart = -1;

        results.RemoveFromEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var stack = new Stack<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                stack.Push(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                stack.Pop();
            }
        });

        results.RemoveFromMiddle = -1;

        results.Search = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var stack = new Stack<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                stack.Push(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                _ = stack.Contains(j * 100);
            }
        });

        results.GetByIndex = -1;

        return results;
    }
}
