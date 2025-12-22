using System.Collections.Immutable;
using Xunit;

namespace Lab02.Tests;

public class CollectionTests
{
    [Fact]
    public void List_Add_Works()
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
    public void List_Insert_Works()
    {
        var list = new List<int> { 1, 3 };
        list.Insert(1, 2);

        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void List_Remove_Works()
    {
        var list = new List<int> { 1, 2, 3 };
        list.RemoveAt(1);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void List_Contains_Works()
    {
        var list = new List<int> { 1, 2, 3 };
        Assert.Contains(2, list);
        Assert.DoesNotContain(4, list);
    }

    [Fact]
    public void List_GetByIndex_Works()
    {
        var list = new List<int> { 10, 20, 30 };
        Assert.Equal(20, list[1]);
    }

    [Fact]
    public void LinkedList_AddFirst_Works()
    {
        var list = new LinkedList<int>();
        list.AddFirst(1);
        list.AddFirst(2);

        Assert.Equal(2, list.Count);
        Assert.Equal(2, list.First!.Value);
        Assert.Equal(1, list.Last!.Value);
    }

    [Fact]
    public void LinkedList_AddLast_Works()
    {
        var list = new LinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list.First!.Value);
        Assert.Equal(2, list.Last!.Value);
    }

    [Fact]
    public void LinkedList_Remove_Works()
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
    public void LinkedList_Contains_Works()
    {
        var list = new LinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);

        Assert.Contains(2, list);
        Assert.DoesNotContain(4, list);
    }

    [Fact]
    public void Queue_Enqueue_Dequeue_Works()
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
    public void Queue_Contains_Works()
    {
        var queue = new Queue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        Assert.Contains(2, queue);
        Assert.DoesNotContain(4, queue);
    }

    [Fact]
    public void Stack_Push_Pop_Works()
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
    public void Stack_Contains_Works()
    {
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        Assert.Contains(2, stack);
        Assert.DoesNotContain(4, stack);
    }

    [Fact]
    public void ImmutableList_Add_Works()
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
    public void ImmutableList_Insert_Works()
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
    public void ImmutableList_Remove_Works()
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
    public void ImmutableList_Contains_Works()
    {
        var list = ImmutableList<int>.Empty;
        list = list.Add(1);
        list = list.Add(2);
        list = list.Add(3);

        Assert.Contains(2, list);
        Assert.DoesNotContain(4, list);
    }

    [Fact]
    public void ImmutableList_GetByIndex_Works()
    {
        var list = ImmutableList<int>.Empty;
        list = list.Add(10);
        list = list.Add(20);
        list = list.Add(30);

        Assert.Equal(20, list[1]);
    }

    [Fact]
    public void ImmutableList_IsImmutable()
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
