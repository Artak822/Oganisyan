using Xunit;

namespace Lab3.Tests;

public class SimpleDictionaryTests
{
    [Fact]
    public void Add_ShouldIncreaseCount()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);
        
        Assert.Equal(2, dict.Count);
    }

    [Fact]
    public void Add_ShouldThrow_WhenKeyExists()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key", 1);
        
        Assert.Throws<ArgumentException>(() => dict.Add("key", 2));
    }

    [Fact]
    public void ContainsKey_ShouldReturnTrue_WhenKeyExists()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("test", 42);
        
        Assert.True(dict.ContainsKey("test"));
    }

    [Fact]
    public void ContainsKey_ShouldReturnFalse_WhenKeyNotExists()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("test", 42);
        
        Assert.False(dict.ContainsKey("other"));
    }

    [Fact]
    public void Indexer_ShouldGetAndSet()
    {
        var dict = new SimpleDictionary<string, int>();
        dict["key"] = 100;
        
        Assert.Equal(100, dict["key"]);
        
        dict["key"] = 200;
        Assert.Equal(200, dict["key"]);
    }

    [Fact]
    public void Indexer_ShouldThrow_WhenKeyNotFound()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.Throws<KeyNotFoundException>(() => dict["missing"]);
    }

    [Fact]
    public void TryGetValue_ShouldReturnTrue_WhenKeyExists()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("test", 42);
        
        Assert.True(dict.TryGetValue("test", out int value));
        Assert.Equal(42, value);
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalse_WhenKeyNotExists()
    {
        var dict = new SimpleDictionary<string, int>();
        
        Assert.False(dict.TryGetValue("missing", out int value));
        Assert.Equal(0, value);
    }

    [Fact]
    public void Remove_ShouldDecreaseCount()
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
    public void Remove_ShouldReturnFalse_WhenKeyNotExists()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        
        bool removed = dict.Remove("two");
        
        Assert.False(removed);
        Assert.Equal(1, dict.Count);
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
    public void Foreach_ShouldIterateAllItems()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("a", 1);
        dict.Add("b", 2);
        dict.Add("c", 3);
        
        var items = new List<KeyValuePair<string, int>>();
        foreach (var item in dict)
        {
            items.Add(item);
        }
        
        Assert.Equal(3, items.Count);
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

    [Fact]
    public void Contains_ShouldReturnTrue_WhenKeyValuePairExists()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("test", 42);
        
        Assert.True(dict.Contains(new KeyValuePair<string, int>("test", 42)));
    }

    [Fact]
    public void Contains_ShouldReturnFalse_WhenValueDifferent()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("test", 42);
        
        Assert.False(dict.Contains(new KeyValuePair<string, int>("test", 100)));
    }
}

