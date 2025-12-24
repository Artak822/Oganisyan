using Xunit;
using Lab3.Collections;

namespace Lab3.Tests;

public class SimpleDictionaryTests
{
    [Fact]
    public void Add_And_Count_ShouldWork()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);
        
        Assert.Equal(2, dict.Count);
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
    public void ContainsKey_And_TryGetValue_ShouldWork()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("test", 42);
        
        Assert.True(dict.ContainsKey("test"));
        Assert.False(dict.ContainsKey("other"));
        
        Assert.True(dict.TryGetValue("test", out int value));
        Assert.Equal(42, value);
    }

    [Fact]
    public void Remove_And_Clear_ShouldWork()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("one", 1);
        dict.Add("two", 2);
        
        Assert.True(dict.Remove("one"));
        Assert.Equal(1, dict.Count);
        Assert.False(dict.ContainsKey("one"));
        
        dict.Clear();
        Assert.Equal(0, dict.Count);
    }

    [Fact]
    public void Foreach_ShouldIterate()
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
}
