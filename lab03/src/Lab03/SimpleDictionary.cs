using System.Collections;

namespace Lab03;

public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{
    private struct Entry
    {
        public int HashCode;
        public int Next;
        public TKey Key;
        public TValue Value;
    }

    private int[]? _buckets;
    private Entry[] _entries;
    private int _count;
    private int _freeList;
    private int _freeCount;
    private const int DefaultCapacity = 4;

    public SimpleDictionary()
    {
        _buckets = new int[DefaultCapacity];
        _entries = new Entry[DefaultCapacity];
        _freeList = -1;
    }

    public SimpleDictionary(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));
        
        int size = capacity > DefaultCapacity ? capacity : DefaultCapacity;
        _buckets = new int[size];
        _entries = new Entry[size];
        _freeList = -1;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int entryIndex = FindEntry(key);
            if (entryIndex >= 0)
                return _entries[entryIndex].Value;
            
            throw new KeyNotFoundException();
        }
        set
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            Insert(key, value, false);
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            for (int i = 0; i < _count; i++)
            {
                if (_entries[i].HashCode >= 0)
                    keys.Add(_entries[i].Key);
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>();
            for (int i = 0; i < _count; i++)
            {
                if (_entries[i].HashCode >= 0)
                    values.Add(_entries[i].Value);
            }
            return values;
        }
    }

    public int Count => _count - _freeCount;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        Insert(key, value, true);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        if (_count > 0)
        {
            Array.Clear(_buckets!, 0, _buckets!.Length);
            Array.Clear(_entries, 0, _count);
            _freeList = -1;
            _count = 0;
            _freeCount = 0;
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int entryIndex = FindEntry(item.Key);
        if (entryIndex >= 0 && EqualityComparer<TValue>.Default.Equals(_entries[entryIndex].Value, item.Value))
            return true;
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        return FindEntry(key) >= 0;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count)
            throw new ArgumentException("Destination array is not long enough.");

        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_entries[i].HashCode >= 0)
            {
                array[arrayIndex + index] = new KeyValuePair<TKey, TValue>(_entries[i].Key, _entries[i].Value);
                index++;
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            if (_entries[i].HashCode >= 0)
                yield return new KeyValuePair<TKey, TValue>(_entries[i].Key, _entries[i].Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (_buckets == null)
            return false;

        int hashCode = key.GetHashCode() & 0x7FFFFFFF;
        int bucket = hashCode % _buckets.Length;
        int last = -1;
        int entryIndex = _buckets[bucket];

        while (entryIndex >= 0)
        {
            if (_entries[entryIndex].HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(_entries[entryIndex].Key, key))
            {
                if (last < 0)
                    _buckets[bucket] = _entries[entryIndex].Next;
                else
                    _entries[last].Next = _entries[entryIndex].Next;

                _entries[entryIndex].HashCode = -1;
                _entries[entryIndex].Next = _freeList;
                _entries[entryIndex].Key = default(TKey)!;
                _entries[entryIndex].Value = default(TValue)!;
                _freeList = entryIndex;
                _freeCount++;
                return true;
            }

            last = entryIndex;
            entryIndex = _entries[entryIndex].Next;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int entryIndex = FindEntry(item.Key);
        if (entryIndex >= 0 && EqualityComparer<TValue>.Default.Equals(_entries[entryIndex].Value, item.Value))
        {
            return Remove(item.Key);
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int entryIndex = FindEntry(key);
        if (entryIndex >= 0)
        {
            value = _entries[entryIndex].Value;
            return true;
        }

        value = default(TValue)!;
        return false;
    }

    private int FindEntry(TKey key)
    {
        if (_buckets == null)
            return -1;

        int hashCode = key.GetHashCode() & 0x7FFFFFFF;
        int entryIndex = _buckets[hashCode % _buckets.Length];

        while (entryIndex >= 0)
        {
            if (_entries[entryIndex].HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(_entries[entryIndex].Key, key))
                return entryIndex;
            entryIndex = _entries[entryIndex].Next;
        }

        return -1;
    }

    private void Insert(TKey key, TValue value, bool add)
    {
        if (_buckets == null)
            Initialize(DefaultCapacity);

        int hashCode = key.GetHashCode() & 0x7FFFFFFF;
        int targetBucket = hashCode % _buckets!.Length;

        for (int i = _buckets[targetBucket]; i >= 0; i = _entries[i].Next)
        {
            if (_entries[i].HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(_entries[i].Key, key))
            {
                if (add)
                    throw new ArgumentException("An item with the same key has already been added.");
                _entries[i].Value = value;
                return;
            }
        }

        int index;
        if (_freeCount > 0)
        {
            index = _freeList;
            _freeList = _entries[index].Next;
            _freeCount--;
        }
        else
        {
            if (_count == _entries.Length)
            {
                Resize();
                targetBucket = hashCode % _buckets.Length;
            }
            index = _count;
            _count++;
        }

        _entries[index].HashCode = hashCode;
        _entries[index].Next = _buckets[targetBucket];
        _entries[index].Key = key;
        _entries[index].Value = value;
        _buckets[targetBucket] = index;
    }

    private void Initialize(int capacity)
    {
        int size = capacity > DefaultCapacity ? capacity : DefaultCapacity;
        _buckets = new int[size];
        _entries = new Entry[size];
        _freeList = -1;
    }

    private void Resize()
    {
        int newSize = _entries.Length * 2;
        int[] newBuckets = new int[newSize];
        Entry[] newEntries = new Entry[newSize];
        Array.Copy(_entries, 0, newEntries, 0, _count);

        for (int i = 0; i < _count; i++)
        {
            if (newEntries[i].HashCode >= 0)
            {
                int bucket = newEntries[i].HashCode % newSize;
                newEntries[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }
        }

        _buckets = newBuckets;
        _entries = newEntries;
    }
}
