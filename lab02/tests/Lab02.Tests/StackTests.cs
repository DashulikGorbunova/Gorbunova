using Xunit;

namespace Lab02.Tests;

public class StackTests
{
    [Fact]
    public void Push_Pop_Works()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        Assert.Equal(3, stack.Count);
        Assert.Equal(3, stack.Pop());
        Assert.Equal(2, stack.Pop());
        Assert.Equal(1, stack.Pop());
        Assert.Empty(stack);
    }

    [Fact]
    public void Contains_Works()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        Assert.Contains(2, stack);
        Assert.DoesNotContain(4, stack);
    }
}
