using Lab02.Models;
using Lab02.Services;

namespace Lab02.Services.Benchmarks;

public class ListBenchmark
{
    private const int CollectionSize = 100_000;

    public CollectionOperationResults Run()
    {
        var results = new CollectionOperationResults { CollectionName = "List<T>" };

        results.AddToEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
        });

        results.AddToStart = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new List<int>();
            for (int j = 0; j < 1000; j++)
            {
                list.Insert(0, j);
            }
        });

        results.AddToMiddle = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list.Insert(list.Count / 2, j);
            }
        });

        results.RemoveFromStart = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveAt(0);
            }
        });

        results.RemoveFromEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveAt(list.Count - 1);
            }
        });

        results.RemoveFromMiddle = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveAt(list.Count / 2);
            }
        });

        results.Search = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                _ = list.Contains(j * 100);
            }
        });

        results.GetByIndex = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                _ = list[j * 100];
            }
        });

        return results;
    }
}
