using System.Diagnostics;

namespace Lab02.Services;

public static class BenchmarkHelper
{
    private const int Iterations = 5;

    public static double MeasureAverageTime(Action action, int iterations = Iterations)
    {
        var times = new List<long>();

        for (int i = 0; i < iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }

        return times.Average();
    }
}
