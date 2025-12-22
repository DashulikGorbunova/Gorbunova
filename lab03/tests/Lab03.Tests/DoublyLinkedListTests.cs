namespace Lab03.Tests;

public class DoublyLinkedListTests
{
    [Fact]
    public void Add_ShouldIncreaseCount()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Contains_ShouldReturnTrue_WhenItemExists()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);

        Assert.True(list.Contains(1));
        Assert.False(list.Contains(3));
    }

    [Fact]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);

        Assert.Equal(0, list.IndexOf(10));
        Assert.Equal(1, list.IndexOf(20));
        Assert.Equal(-1, list.IndexOf(40));
    }

    [Fact]
    public void Insert_ShouldAddItemAtSpecifiedIndex()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(3);
        list.Insert(1, 2);

        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Insert_AtBeginning_ShouldWork()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(2);
        list.Insert(0, 1);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void Insert_AtEnd_ShouldWork()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Insert(1, 2);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void Remove_ShouldRemoveItem()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        bool removed = list.Remove(2);

        Assert.True(removed);
        Assert.Equal(2, list.Count);
        Assert.False(list.Contains(2));
    }

    [Fact]
    public void RemoveAt_ShouldRemoveItemAtIndex()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.RemoveAt(1);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void RemoveAt_First_ShouldWork()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.RemoveAt(0);

        Assert.Equal(1, list.Count);
        Assert.Equal(2, list[0]);
    }

    [Fact]
    public void RemoveAt_Last_ShouldWork()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.RemoveAt(1);

        Assert.Equal(1, list.Count);
        Assert.Equal(1, list[0]);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Clear();

        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Indexer_ShouldGetAndSetValues()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list[1] = 20;

        Assert.Equal(20, list[1]);
    }

    [Fact]
    public void Indexer_ShouldThrow_WhenIndexOutOfRange()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);

        Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[10]);
    }

    [Fact]
    public void GetEnumerator_ShouldIterateAllItems()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var items = new List<int>();
        foreach (var item in list)
        {
            items.Add(item);
        }

        Assert.Equal(3, items.Count);
        Assert.Contains(1, items);
        Assert.Contains(2, items);
        Assert.Contains(3, items);
    }

    [Fact]
    public void CopyTo_ShouldCopyItemsToArray()
    {
        var list = new DoublyLinkedList<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var array = new int[5];
        list.CopyTo(array, 1);

        Assert.Equal(0, array[0]);
        Assert.Equal(1, array[1]);
        Assert.Equal(2, array[2]);
        Assert.Equal(3, array[3]);
    }
}
