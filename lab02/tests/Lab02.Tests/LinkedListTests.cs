using Xunit;

namespace Lab02.Tests;

public class LinkedListTests
{
    [Fact]
    public void AddFirst_Works()
    {
        var list = new LinkedList<int>();
        list.AddFirst(1);
        list.AddFirst(2);

        Assert.Equal(2, list.Count);
        Assert.Equal(2, list.First!.Value);
        Assert.Equal(1, list.Last!.Value);
    }

    [Fact]
    public void AddLast_Works()
    {
        var list = new LinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list.First!.Value);
        Assert.Equal(2, list.Last!.Value);
    }

    [Fact]
    public void Remove_Works()
    {
        var list = new LinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);

        list.Remove(2);

        Assert.Equal(2, list.Count);
        Assert.DoesNotContain(2, list);
    }

    [Fact]
    public void Contains_Works()
    {
        var list = new LinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);

        Assert.Contains(2, list);
        Assert.DoesNotContain(4, list);
    }
}
