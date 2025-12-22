namespace Lab03.Tests;

public class SimpleDictionaryTests
{
    [Fact]
    public void Add_ShouldAddKeyValuePair()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);

        Assert.Equal(2, dict.Count);
    }

    [Fact]
    public void ContainsKey_ShouldReturnTrue_WhenKeyExists()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);

        Assert.True(dict.ContainsKey("one"));
        Assert.False(dict.ContainsKey("two"));
    }

    [Fact]
    public void Indexer_ShouldGetAndSetValues()
    {
        var dict = new SimpleDictionary<string, int>();
        dict["one"] = 1;
        dict["two"] = 2;

        Assert.Equal(1, dict["one"]);
        Assert.Equal(2, dict["two"]);
    }

    [Fact]
    public void Indexer_ShouldThrow_WhenKeyNotFound()
    {
        var dict = new SimpleDictionary<string, int>();

        Assert.Throws<KeyNotFoundException>(() => dict["nonexistent"]);
    }

    [Fact]
    public void Remove_ShouldRemoveKeyValuePair()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);

        bool removed = dict.Remove("one");

        Assert.True(removed);
        Assert.Equal(1, dict.Count);
        Assert.False(dict.ContainsKey("one"));
    }

    [Fact]
    public void TryGetValue_ShouldReturnTrue_WhenKeyExists()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);

        bool found = dict.TryGetValue("one", out int value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        var dict = new SimpleDictionary<string, int>();

        bool found = dict.TryGetValue("nonexistent", out int value);

        Assert.False(found);
        Assert.Equal(0, value);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);
        dict.Clear();

        Assert.Equal(0, dict.Count);
    }

    [Fact]
    public void GetEnumerator_ShouldIterateAllItems()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);

        var items = new List<KeyValuePair<string, int>>();
        foreach (var item in dict)
        {
            items.Add(item);
        }

        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void Add_ShouldThrow_WhenDuplicateKey()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);

        Assert.Throws<ArgumentException>(() => dict.Add("one", 2));
    }

    [Fact]
    public void Keys_ShouldReturnAllKeys()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);

        var keys = dict.Keys;

        Assert.Equal(2, keys.Count);
        Assert.Contains("one", keys);
        Assert.Contains("two", keys);
    }

    [Fact]
    public void Values_ShouldReturnAllValues()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);

        var values = dict.Values;

        Assert.Equal(2, values.Count);
        Assert.Contains(1, values);
        Assert.Contains(2, values);
    }
}
