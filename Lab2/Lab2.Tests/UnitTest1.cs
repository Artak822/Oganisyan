using System.Collections.Immutable;

namespace Lab2.Tests;

public class CollectionCorrectnessTests
{
    private const int TestSize = 1000;

    #region List Tests

    [Fact]
    public void List_AddToEnd_ShouldAddElements()
    {
        // Arrange
        var list = new List<int>();

        // Act
        for (int i = 0; i < TestSize; i++)
        {
            list.Add(i);
        }

        // Assert
        Assert.Equal(TestSize, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(TestSize - 1, list[TestSize - 1]);
    }

    [Fact]
    public void List_AddToBeginning_ShouldAddElements()
    {
        // Arrange
        var list = new List<int>();

        // Act
        for (int i = 0; i < TestSize; i++)
        {
            list.Insert(0, i);
        }

        // Assert
        Assert.Equal(TestSize, list.Count);
        Assert.Equal(TestSize - 1, list[0]);
        Assert.Equal(0, list[TestSize - 1]);
    }

    [Fact]
    public void List_AddToMiddle_ShouldAddElements()
    {
        // Arrange
        var list = new List<int> { 0, 1, 2, 3, 4 };

        // Act
        list.Insert(2, 99);

        // Assert
        Assert.Equal(6, list.Count);
        Assert.Equal(99, list[2]);
        Assert.Equal(2, list[3]);
    }

    [Fact]
    public void List_RemoveFromEnd_ShouldRemoveElements()
    {
        // Arrange
        var list = new List<int>(Enumerable.Range(0, TestSize));

        // Act
        while (list.Count > 0)
        {
            list.RemoveAt(list.Count - 1);
        }

        // Assert
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void List_RemoveFromBeginning_ShouldRemoveElements()
    {
        // Arrange
        var list = new List<int>(Enumerable.Range(0, TestSize));

        // Act
        while (list.Count > 0)
        {
            list.RemoveAt(0);
        }

        // Assert
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void List_RemoveFromMiddle_ShouldRemoveElements()
    {
        // Arrange
        var list = new List<int> { 0, 1, 2, 3, 4 };

        // Act
        list.RemoveAt(2);

        // Assert
        Assert.Equal(4, list.Count);
        Assert.Equal(3, list[2]);
        Assert.DoesNotContain(2, list);
    }

    [Fact]
    public void List_Search_ShouldFindElement()
    {
        // Arrange
        var list = new List<int>(Enumerable.Range(0, TestSize));
        var searchValue = TestSize / 2;

        // Act
        var index = list.IndexOf(searchValue);

        // Assert
        Assert.Equal(searchValue, index);
        Assert.Equal(searchValue, list[index]);
    }

    [Fact]
    public void List_GetByIndex_ShouldReturnCorrectElement()
    {
        // Arrange
        var list = new List<int>(Enumerable.Range(0, TestSize));

        // Act & Assert
        for (int i = 0; i < TestSize; i++)
        {
            Assert.Equal(i, list[i]);
        }
    }

    #endregion

    #region LinkedList Tests

    [Fact]
    public void LinkedList_AddToEnd_ShouldAddElements()
    {
        // Arrange
        var list = new LinkedList<int>();

        // Act
        for (int i = 0; i < TestSize; i++)
        {
            list.AddLast(i);
        }

        // Assert
        Assert.Equal(TestSize, list.Count);
        Assert.Equal(0, list.First!.Value);
        Assert.Equal(TestSize - 1, list.Last!.Value);
    }

    [Fact]
    public void LinkedList_AddToBeginning_ShouldAddElements()
    {
        // Arrange
        var list = new LinkedList<int>();

        // Act
        for (int i = 0; i < TestSize; i++)
        {
            list.AddFirst(i);
        }

        // Assert
        Assert.Equal(TestSize, list.Count);
        Assert.Equal(TestSize - 1, list.First!.Value);
        Assert.Equal(0, list.Last!.Value);
    }

    [Fact]
    public void LinkedList_AddToMiddle_ShouldAddElements()
    {
        // Arrange
        var list = new LinkedList<int>();
        list.AddLast(0);
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        var middleNode = list.Find(2)!;

        // Act
        list.AddAfter(middleNode, 99);

        // Assert
        Assert.Equal(5, list.Count);
        Assert.Equal(99, middleNode.Next!.Value);
    }

    [Fact]
    public void LinkedList_RemoveFromEnd_ShouldRemoveElements()
    {
        // Arrange
        var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

        // Act
        while (list.Count > 0)
        {
            list.RemoveLast();
        }

        // Assert
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void LinkedList_RemoveFromBeginning_ShouldRemoveElements()
    {
        // Arrange
        var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

        // Act
        while (list.Count > 0)
        {
            list.RemoveFirst();
        }

        // Assert
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void LinkedList_RemoveFromMiddle_ShouldRemoveElements()
    {
        // Arrange
        var list = new LinkedList<int>();
        list.AddLast(0);
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        list.AddLast(4);
        var nodeToRemove = list.Find(2)!;

        // Act
        list.Remove(nodeToRemove);

        // Assert
        Assert.Equal(4, list.Count);
        Assert.DoesNotContain(2, list);
    }

    [Fact]
    public void LinkedList_Search_ShouldFindElement()
    {
        // Arrange
        var list = new LinkedList<int>(Enumerable.Range(0, TestSize));
        var searchValue = TestSize / 2;

        // Act
        var node = list.Find(searchValue);

        // Assert
        Assert.NotNull(node);
        Assert.Equal(searchValue, node!.Value);
    }

    [Fact]
    public void LinkedList_GetByIndex_ShouldReturnCorrectElement()
    {
        // Arrange
        var list = new LinkedList<int>(Enumerable.Range(0, TestSize));

        // Act & Assert
        var node = list.First;
        int index = 0;
        while (node != null)
        {
            Assert.Equal(index, node.Value);
            node = node.Next;
            index++;
        }
    }

    #endregion

    #region Queue Tests

    [Fact]
    public void Queue_Enqueue_ShouldAddElements()
    {
        // Arrange
        var queue = new Queue<int>();

        // Act
        for (int i = 0; i < TestSize; i++)
        {
            queue.Enqueue(i);
        }

        // Assert
        Assert.Equal(TestSize, queue.Count);
    }

    [Fact]
    public void Queue_Dequeue_ShouldRemoveElements()
    {
        // Arrange
        var queue = new Queue<int>(Enumerable.Range(0, TestSize));

        // Act
        var dequeued = new List<int>();
        while (queue.Count > 0)
        {
            dequeued.Add(queue.Dequeue());
        }

        // Assert
        Assert.Equal(0, queue.Count);
        Assert.Equal(TestSize, dequeued.Count);
        Assert.Equal(0, dequeued[0]);
        Assert.Equal(TestSize - 1, dequeued[TestSize - 1]);
    }

    [Fact]
    public void Queue_Search_ShouldFindElement()
    {
        // Arrange
        var queue = new Queue<int>(Enumerable.Range(0, TestSize));
        var searchValue = TestSize / 2;

        // Act
        var found = queue.Contains(searchValue);

        // Assert
        Assert.True(found);
    }

    [Fact]
    public void Queue_FIFO_Order_ShouldBeMaintained()
    {
        // Arrange
        var queue = new Queue<int>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            queue.Enqueue(i);
        }

        // Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(i, queue.Dequeue());
        }
    }

    #endregion

    #region Stack Tests

    [Fact]
    public void Stack_Push_ShouldAddElements()
    {
        // Arrange
        var stack = new Stack<int>();

        // Act
        for (int i = 0; i < TestSize; i++)
        {
            stack.Push(i);
        }

        // Assert
        Assert.Equal(TestSize, stack.Count);
    }

    [Fact]
    public void Stack_Pop_ShouldRemoveElements()
    {
        // Arrange
        var stack = new Stack<int>(Enumerable.Range(0, TestSize));

        // Act
        var popped = new List<int>();
        while (stack.Count > 0)
        {
            popped.Add(stack.Pop());
        }

        // Assert
        Assert.Equal(0, stack.Count);
        Assert.Equal(TestSize, popped.Count);
        Assert.Equal(TestSize - 1, popped[0]);
        Assert.Equal(0, popped[TestSize - 1]);
    }

    [Fact]
    public void Stack_Search_ShouldFindElement()
    {
        // Arrange
        var stack = new Stack<int>(Enumerable.Range(0, TestSize));
        var searchValue = TestSize / 2;

        // Act
        var found = stack.Contains(searchValue);

        // Assert
        Assert.True(found);
    }

    [Fact]
    public void Stack_LIFO_Order_ShouldBeMaintained()
    {
        // Arrange
        var stack = new Stack<int>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            stack.Push(i);
        }

        // Assert
        for (int i = 9; i >= 0; i--)
        {
            Assert.Equal(i, stack.Pop());
        }
    }

    #endregion

    #region ImmutableList Tests

    [Fact]
    public void ImmutableList_AddToEnd_ShouldAddElements()
    {
        // Arrange
        var list = ImmutableList<int>.Empty;

        // Act
        for (int i = 0; i < TestSize; i++)
        {
            list = list.Add(i);
        }

        // Assert
        Assert.Equal(TestSize, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(TestSize - 1, list[TestSize - 1]);
    }

    [Fact]
    public void ImmutableList_AddToBeginning_ShouldAddElements()
    {
        // Arrange
        var list = ImmutableList<int>.Empty;

        // Act
        for (int i = 0; i < TestSize; i++)
        {
            list = list.Insert(0, i);
        }

        // Assert
        Assert.Equal(TestSize, list.Count);
        Assert.Equal(TestSize - 1, list[0]);
        Assert.Equal(0, list[TestSize - 1]);
    }

    [Fact]
    public void ImmutableList_AddToMiddle_ShouldAddElements()
    {
        // Arrange
        var list = ImmutableList<int>.Empty.AddRange(new[] { 0, 1, 2, 3, 4 });

        // Act
        list = list.Insert(2, 99);

        // Assert
        Assert.Equal(6, list.Count);
        Assert.Equal(99, list[2]);
        Assert.Equal(2, list[3]);
    }

    [Fact]
    public void ImmutableList_RemoveFromEnd_ShouldRemoveElements()
    {
        // Arrange
        var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

        // Act
        var tempList = list;
        while (tempList.Count > 0)
        {
            tempList = tempList.RemoveAt(tempList.Count - 1);
        }

        // Assert
        Assert.Equal(0, tempList.Count);
        Assert.Equal(TestSize, list.Count); // Original should be unchanged
    }

    [Fact]
    public void ImmutableList_RemoveFromBeginning_ShouldRemoveElements()
    {
        // Arrange
        var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

        // Act
        var tempList = list;
        while (tempList.Count > 0)
        {
            tempList = tempList.RemoveAt(0);
        }

        // Assert
        Assert.Equal(0, tempList.Count);
        Assert.Equal(TestSize, list.Count); // Original should be unchanged
    }

    [Fact]
    public void ImmutableList_RemoveFromMiddle_ShouldRemoveElements()
    {
        // Arrange
        var list = ImmutableList<int>.Empty.AddRange(new[] { 0, 1, 2, 3, 4 });

        // Act
        var newList = list.RemoveAt(2);

        // Assert
        Assert.Equal(4, newList.Count);
        Assert.Equal(3, newList[2]);
        Assert.DoesNotContain(2, newList);
        Assert.Equal(5, list.Count); // Original should be unchanged
    }

    [Fact]
    public void ImmutableList_Search_ShouldFindElement()
    {
        // Arrange
        var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));
        var searchValue = TestSize / 2;

        // Act
        var index = list.IndexOf(searchValue);

        // Assert
        Assert.Equal(searchValue, index);
        Assert.Equal(searchValue, list[index]);
    }

    [Fact]
    public void ImmutableList_GetByIndex_ShouldReturnCorrectElement()
    {
        // Arrange
        var list = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, TestSize));

        // Act & Assert
        for (int i = 0; i < TestSize; i++)
        {
            Assert.Equal(i, list[i]);
        }
    }

    [Fact]
    public void ImmutableList_Immutability_ShouldBePreserved()
    {
        // Arrange
        var original = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, 10));

        // Act
        var modified = original.Add(99);
        var removed = original.RemoveAt(0);

        // Assert
        Assert.Equal(10, original.Count);
        Assert.Equal(11, modified.Count);
        Assert.Equal(9, removed.Count);
        Assert.DoesNotContain(99, original);
        Assert.Contains(0, original);
        Assert.DoesNotContain(0, removed);
    }

    #endregion
}
