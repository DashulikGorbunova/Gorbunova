using System.Collections;

namespace Lab03;

public class DoublyLinkedList<T> : IList<T>
{
    private class Node
    {
        public T Value { get; set; }
        public Node? Previous { get; set; }
        public Node? Next { get; set; }

        public Node(T value)
        {
            Value = value;
        }
    }

    private Node? _head;
    private Node? _tail;
    private int _count;

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));

            Node? node = GetNodeAt(index);
            return node!.Value;
        }
        set
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));

            Node? node = GetNodeAt(index);
            node!.Value = value;
        }
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        Node newNode = new Node(item);

        if (_head == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            _tail!.Next = newNode;
            newNode.Previous = _tail;
            _tail = newNode;
        }

        _count++;
    }

    public void Clear()
    {
        _head = null;
        _tail = null;
        _count = 0;
    }

    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < _count)
            throw new ArgumentException("Destination array is not long enough.");

        Node? current = _head;
        int index = 0;
        while (current != null)
        {
            array[arrayIndex + index] = current.Value;
            current = current.Next;
            index++;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node? current = _head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item)
    {
        Node? current = _head;
        int index = 0;

        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, item))
                return index;

            current = current.Next;
            index++;
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Node newNode = new Node(item);

        if (index == 0)
        {
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                newNode.Next = _head;
                _head.Previous = newNode;
                _head = newNode;
            }
        }
        else if (index == _count)
        {
            _tail!.Next = newNode;
            newNode.Previous = _tail;
            _tail = newNode;
        }
        else
        {
            Node? nodeAt = GetNodeAt(index);
            newNode.Next = nodeAt;
            newNode.Previous = nodeAt!.Previous;
            nodeAt.Previous!.Next = newNode;
            nodeAt.Previous = newNode;
        }

        _count++;
    }

    public bool Remove(T item)
    {
        Node? current = _head;

        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, item))
            {
                RemoveNode(current);
                return true;
            }
            current = current.Next;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Node? nodeToRemove = GetNodeAt(index);
        RemoveNode(nodeToRemove!);
    }

    private Node? GetNodeAt(int index)
    {
        if (index < 0 || index >= _count)
            return null;

        if (index < _count / 2)
        {
            Node? current = _head;
            for (int i = 0; i < index; i++)
            {
                current = current!.Next;
            }
            return current;
        }
        else
        {
            Node? current = _tail;
            for (int i = _count - 1; i > index; i--)
            {
                current = current!.Previous;
            }
            return current;
        }
    }

    private void RemoveNode(Node node)
    {
        if (node.Previous != null)
            node.Previous.Next = node.Next;
        else
            _head = node.Next;

        if (node.Next != null)
            node.Next.Previous = node.Previous;
        else
            _tail = node.Previous;

        _count--;
    }
}
