using System.Collections.Immutable;
using Lab02.Models;
using Lab02.Services;

namespace Lab02.Services.Benchmarks;

public class ImmutableListBenchmark
{
    private const int CollectionSize = 100_000;

    public CollectionOperationResults Run()
    {
        var results = new CollectionOperationResults { CollectionName = "ImmutableList<T>" };

        results.AddToEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
        });

        results.AddToStart = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < 1000; j++)
            {
                list = list.Insert(0, j);
            }
        });

        results.AddToMiddle = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list = list.Insert(list.Count / 2, j);
            }
        });

        results.RemoveFromStart = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list = list.RemoveAt(0);
            }
        });

        results.RemoveFromEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list = list.RemoveAt(list.Count - 1);
            }
        });

        results.RemoveFromMiddle = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list = list.RemoveAt(list.Count / 2);
            }
        });

        results.Search = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                _ = list.Contains(j * 100);
            }
        });

        results.GetByIndex = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                _ = list[j * 100];
            }
        });

        return results;
    }
}
