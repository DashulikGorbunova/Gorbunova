using System.Collections.Immutable;
using System.Diagnostics;
using Lab02.Models;

namespace Lab02.Services;

public class CollectionBenchmark
{
    private const int CollectionSize = 100_000;
    private const int Iterations = 5;

    public BenchmarkResult RunBenchmarks()
    {
        var results = new BenchmarkResult();

        results.ListResults = BenchmarkList();
        results.LinkedListResults = BenchmarkLinkedList();
        results.QueueResults = BenchmarkQueue();
        results.StackResults = BenchmarkStack();
        results.ImmutableListResults = BenchmarkImmutableList();

        return results;
    }

    private CollectionOperationResults BenchmarkList()
    {
        var results = new CollectionOperationResults { CollectionName = "List<T>" };

        var times = new List<long>();

        for (int i = 0; i < Iterations; i++)
        {
            var list = new List<int>(CollectionSize);
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToEnd = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new List<int>();
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list.Insert(0, j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToStart = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list.Insert(list.Count / 2, j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToMiddle = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveAt(0);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromStart = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveAt(list.Count - 1);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromEnd = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveAt(list.Count / 2);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromMiddle = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                _ = list.Contains(j * 100);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.Search = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new List<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                _ = list[j * 100];
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.GetByIndex = times.Average();

        return results;
    }

    private CollectionOperationResults BenchmarkLinkedList()
    {
        var results = new CollectionOperationResults { CollectionName = "LinkedList<T>" };

        var times = new List<long>();

        for (int i = 0; i < Iterations; i++)
        {
            var list = new LinkedList<int>();
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToEnd = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new LinkedList<int>();
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddFirst(j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToStart = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
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
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list.AddAfter(middle!, j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToMiddle = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveFirst();
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromStart = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list.RemoveLast();
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromEnd = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
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
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                var next = middle!.Next;
                list.Remove(middle);
                middle = next ?? list.First;
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromMiddle = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = new LinkedList<int>();
            for (int j = 0; j < CollectionSize; j++)
            {
                list.AddLast(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                _ = list.Contains(j * 100);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.Search = times.Average();

        results.GetByIndex = -1;

        return results;
    }

    private CollectionOperationResults BenchmarkQueue()
    {
        var results = new CollectionOperationResults { CollectionName = "Queue<T>" };

        var times = new List<long>();

        for (int i = 0; i < Iterations; i++)
        {
            var queue = new Queue<int>(CollectionSize);
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < CollectionSize; j++)
            {
                queue.Enqueue(j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToEnd = times.Average();

        results.AddToStart = -1;
        results.AddToMiddle = -1;

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var queue = new Queue<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                queue.Enqueue(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                queue.Dequeue();
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromStart = times.Average();

        results.RemoveFromEnd = -1;
        results.RemoveFromMiddle = -1;

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var queue = new Queue<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                queue.Enqueue(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                _ = queue.Contains(j * 100);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.Search = times.Average();

        results.GetByIndex = -1;

        return results;
    }

    private CollectionOperationResults BenchmarkStack()
    {
        var results = new CollectionOperationResults { CollectionName = "Stack<T>" };

        var times = new List<long>();

        for (int i = 0; i < Iterations; i++)
        {
            var stack = new Stack<int>(CollectionSize);
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < CollectionSize; j++)
            {
                stack.Push(j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToEnd = times.Average();

        results.AddToStart = -1;
        results.AddToMiddle = -1;

        results.RemoveFromStart = -1;

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var stack = new Stack<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                stack.Push(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                stack.Pop();
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromEnd = times.Average();

        results.RemoveFromMiddle = -1;

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var stack = new Stack<int>(CollectionSize);
            for (int j = 0; j < CollectionSize; j++)
            {
                stack.Push(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                _ = stack.Contains(j * 100);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.Search = times.Average();

        results.GetByIndex = -1;

        return results;
    }

    private CollectionOperationResults BenchmarkImmutableList()
    {
        var results = new CollectionOperationResults { CollectionName = "ImmutableList<T>" };

        var times = new List<long>();

        for (int i = 0; i < Iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToEnd = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list = list.Insert(0, j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToStart = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list = list.Insert(list.Count / 2, j);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.AddToMiddle = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list = list.RemoveAt(0);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromStart = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list = list.RemoveAt(list.Count - 1);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromEnd = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                list = list.RemoveAt(list.Count / 2);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.RemoveFromMiddle = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                _ = list.Contains(j * 100);
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.Search = times.Average();

        times.Clear();
        for (int i = 0; i < Iterations; i++)
        {
            var list = ImmutableList<int>.Empty;
            for (int j = 0; j < CollectionSize; j++)
            {
                list = list.Add(j);
            }
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < 1000; j++)
            {
                _ = list[j * 100];
            }
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }
        results.GetByIndex = times.Average();

        return results;
    }
}
