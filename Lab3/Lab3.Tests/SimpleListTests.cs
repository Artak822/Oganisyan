using Xunit;
using Lab3.Collections;

namespace Lab3.Tests;

public class SimpleListTests
{
    [Fact]
    public void Add_And_Count_ShouldWork()
    {
        var list = new SimpleList();
        list.Add(1);
        list.Add(2);
        
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Indexer_ShouldGetAndSet()
    {
        var list = new SimpleList();
        list.Add(10);
        list.Add(20);
        
        Assert.Equal(10, list[0]);
        list[0] = 100;
        Assert.Equal(100, list[0]);
    }

    [Fact]
    public void Contains_And_IndexOf_ShouldWork()
    {
        var list = new SimpleList();
        list.Add("first");
        list.Add("second");
        
        Assert.True(list.Contains("second"));
        Assert.Equal(1, list.IndexOf("second"));
        Assert.Equal(-1, list.IndexOf("notfound"));
    }

    [Fact]
    public void Remove_And_RemoveAt_ShouldWork()
    {
        var list = new SimpleList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.Remove(2);
        Assert.Equal(2, list.Count);
        Assert.False(list.Contains(2));
        
        list.RemoveAt(0);
        Assert.Equal(1, list.Count);
        Assert.Equal(3, list[0]);
    }

    [Fact]
    public void Insert_And_Clear_ShouldWork()
    {
        var list = new SimpleList();
        list.Add(1);
        list.Add(3);
        
        list.Insert(1, 2);
        Assert.Equal(3, list.Count);
        Assert.Equal(2, list[1]);
        
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Foreach_ShouldIterate()
    {
        var list = new SimpleList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        var items = new List<object>();
        foreach (var item in list)
        {
            items.Add(item);
        }
        
        Assert.Equal(3, items.Count);
        Assert.Equal(1, items[0]);
        Assert.Equal(2, items[1]);
        Assert.Equal(3, items[2]);
    }
}
