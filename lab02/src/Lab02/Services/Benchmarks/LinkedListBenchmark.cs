using Lab02.Models;
using Lab02.Services;

namespace Lab02.Services.Benchmarks;

public class LinkedListBenchmark
{
    private const int CollectionSize = 100_000;

    public CollectionOperationResults Run()
    {
        var results = new CollectionOperationResults { CollectionName = "LinkedList<T>" };

        results.AddToEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
        });

        results.AddToStart = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddFirst(j);
            }
        });

        results.AddToMiddle = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            var middle = list.First;
            for (int k = 0; k < CollectionSize / 2; k++)
            {
                middle = middle!.Next;
            }
            for (int j = 0; j < 1000; j++)
            {
                list.AddAfter(middle!, j);
            }
        });

        results.RemoveFromStart = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveFirst();
            }
        });

        results.RemoveFromEnd = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveLast();
            }
        });

        results.RemoveFromMiddle = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            var middle = list.First;
            for (int k = 0; k < CollectionSize / 2; k++)
            {
                middle = middle!.Next;
            }
            for (int j = 0; j < 1000; j++)
            {
                var next = middle!.Next;
                list.Remove(middle);
                middle = next ?? list.First;
            }
        });

        results.Search = BenchmarkHelper.MeasureAverageTime(() =>
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            for (int j = 0; j < 1000; j++)
            {
                _ = list.Contains(j * 100);
            }
        });

        results.GetByIndex = -1;

        return results;
    }
}
