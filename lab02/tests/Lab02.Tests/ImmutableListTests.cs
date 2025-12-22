using System.Collections.Immutable;
using Xunit;

namespace Lab02.Tests;

public class ImmutableListTests
{
    [Fact]
    public void Add_Works()
    {
        var list = ImmutableList<int>.Empty;
        list = list.Add(1);
        list = list.Add(2);
        list = list.Add(3);

        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Insert_Works()
    {
        var list = ImmutableList<int>.Empty;
        list = list.Add(1);
        list = list.Add(3);
        list = list.Insert(1, 2);

        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Remove_Works()
    {
        var list = ImmutableList<int>.Empty;
        list = list.Add(1);
        list = list.Add(2);
        list = list.Add(3);
        list = list.RemoveAt(1);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void Contains_Works()
    {
        var list = ImmutableList<int>.Empty;
        list = list.Add(1);
        list = list.Add(2);
        list = list.Add(3);

        Assert.Contains(2, list);
        Assert.DoesNotContain(4, list);
    }

    [Fact]
    public void GetByIndex_Works()
    {
        var list = ImmutableList<int>.Empty;
        list = list.Add(10);
        list = list.Add(20);
        list = list.Add(30);

        Assert.Equal(20, list[1]);
    }

    [Fact]
    public void IsImmutable()
    {
        var list1 = ImmutableList<int>.Empty;
        list1 = list1.Add(1);
        list1 = list1.Add(2);

        var list2 = list1.Add(3);

        Assert.Equal(2, list1.Count);
        Assert.Equal(3, list2.Count);
        Assert.DoesNotContain(3, list1);
        Assert.Contains(3, list2);
    }
}
