using Xunit;

namespace Lab02.Tests;

public class QueueTests
{
    [Fact]
    public void Enqueue_Dequeue_Works()
    {
        var queue = new Queue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        Assert.Equal(3, queue.Count);
        Assert.Equal(1, queue.Dequeue());
        Assert.Equal(2, queue.Dequeue());
        Assert.Equal(3, queue.Dequeue());
        Assert.Empty(queue);
    }

    [Fact]
    public void Contains_Works()
    {
        var queue = new Queue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        Assert.Contains(2, queue);
        Assert.DoesNotContain(4, queue);
    }
}
