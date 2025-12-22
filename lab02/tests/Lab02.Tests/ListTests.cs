using Xunit;

namespace Lab02.Tests;

public class ListTests
{
    [Fact]
    public void Add_Works()
    {
        var list = new List<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Insert_Works()
    {
        var list = new List<int> { 1, 3 };
        list.Insert(1, 2);

        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Remove_Works()
    {
        var list = new List<int> { 1, 2, 3 };
        list.RemoveAt(1);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void Contains_Works()
    {
        var list = new List<int> { 1, 2, 3 };
        Assert.Contains(2, list);
        Assert.DoesNotContain(4, list);
    }

    [Fact]
    public void GetByIndex_Works()
    {
        var list = new List<int> { 10, 20, 30 };
        Assert.Equal(20, list[1]);
    }
}
