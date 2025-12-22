using Xunit;

namespace Lab3.Tests;

public class BidirectionalLinkedListTests
{
    [Fact]
    public void Add_ShouldIncreaseCount()
    {
        var list = new BidirectionalLinkedList();
        list.Add(1);
        list.Add(2);
        
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        var list = new BidirectionalLinkedList();
        list.Add("first");
        list.Add("second");
        list.Add("third");
        
        Assert.Equal(1, list.IndexOf("second"));
    }

    [Fact]
    public void IndexOf_ShouldReturnMinusOne_WhenNotFound()
    {
        var list = new BidirectionalLinkedList();
        list.Add("test");
        
        Assert.Equal(-1, list.IndexOf("notfound"));
    }

    [Fact]
    public void Contains_ShouldReturnTrue_WhenItemExists()
    {
        var list = new BidirectionalLinkedList();
        list.Add(42);
        
        Assert.True(list.Contains(42));
    }

    [Fact]
    public void Contains_ShouldReturnFalse_WhenItemNotExists()
    {
        var list = new BidirectionalLinkedList();
        list.Add(42);
        
        Assert.False(list.Contains(100));
    }

    [Fact]
    public void Remove_ShouldDecreaseCount()
    {
        var list = new BidirectionalLinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.Remove(2);
        
        Assert.Equal(2, list.Count);
        Assert.False(list.Contains(2));
    }

    [Fact]
    public void RemoveAt_ShouldRemoveItemAtIndex()
    {
        var list = new BidirectionalLinkedList();
        list.Add("a");
        list.Add("b");
        list.Add("c");
        
        list.RemoveAt(1);
        
        Assert.Equal(2, list.Count);
        Assert.Equal("c", list[1]);
    }

    [Fact]
    public void RemoveAt_FirstItem_ShouldWork()
    {
        var list = new BidirectionalLinkedList();
        list.Add("a");
        list.Add("b");
        list.Add("c");
        
        list.RemoveAt(0);
        
        Assert.Equal(2, list.Count);
        Assert.Equal("b", list[0]);
    }

    [Fact]
    public void RemoveAt_LastItem_ShouldWork()
    {
        var list = new BidirectionalLinkedList();
        list.Add("a");
        list.Add("b");
        list.Add("c");
        
        list.RemoveAt(2);
        
        Assert.Equal(2, list.Count);
        Assert.Equal("b", list[1]);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var list = new BidirectionalLinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.Clear();
        
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Indexer_ShouldGetAndSet()
    {
        var list = new BidirectionalLinkedList();
        list.Add(10);
        list.Add(20);
        
        list[0] = 100;
        
        Assert.Equal(100, list[0]);
    }

    [Fact]
    public void Indexer_ShouldThrow_WhenIndexOutOfRange()
    {
        var list = new BidirectionalLinkedList();
        list.Add(1);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list[10]);
    }

    [Fact]
    public void Insert_AtBeginning_ShouldWork()
    {
        var list = new BidirectionalLinkedList();
        list.Add(2);
        list.Add(3);
        
        list.Insert(0, 1);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
    }

    [Fact]
    public void Insert_AtMiddle_ShouldWork()
    {
        var list = new BidirectionalLinkedList();
        list.Add(1);
        list.Add(3);
        
        list.Insert(1, 2);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(2, list[1]);
    }

    [Fact]
    public void Insert_AtEnd_ShouldWork()
    {
        var list = new BidirectionalLinkedList();
        list.Add(1);
        list.Add(2);
        
        list.Insert(2, 3);
        
        Assert.Equal(3, list.Count);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Foreach_ShouldIterateAllItems()
    {
        var list = new BidirectionalLinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        var items = new List<object?>();
        foreach (var item in list)
        {
            items.Add(item);
        }
        
        Assert.Equal(3, items.Count);
        Assert.Equal(1, items[0]);
        Assert.Equal(2, items[1]);
        Assert.Equal(3, items[2]);
    }

    [Fact]
    public void CopyTo_ShouldCopyElements()
    {
        var list = new BidirectionalLinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        var array = new object[5];
        list.CopyTo(array, 1);
        
        Assert.Equal(1, array[1]);
        Assert.Equal(2, array[2]);
        Assert.Equal(3, array[3]);
    }

    [Fact]
    public void Indexer_ShouldWorkFromBothEnds()
    {
        var list = new BidirectionalLinkedList();
        for (int i = 0; i < 10; i++)
        {
            list.Add(i);
        }
        
        Assert.Equal(0, list[0]);
        Assert.Equal(9, list[9]);
        Assert.Equal(5, list[5]);
    }
}

