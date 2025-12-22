using System.Collections;

public class BidirectionalLinkedList : IEnumerable, IList
{
    private class Node
    {
        public object? Value { get; set; }
        public Node? Next { get; set; }
        public Node? Previous { get; set; }
    }

    private Node? _head;
    private Node? _tail;
    private int _count;

    public int Count => _count;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public object? this[int index]
    {
        get
        {
            var node = GetNodeAt(index);
            return node.Value;
        }
        set
        {
            var node = GetNodeAt(index);
            node.Value = value;
        }
    }

    public int Add(object? value)
    {
        var newNode = new Node { Value = value };

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
        return _count - 1;
    }

    public void Clear()
    {
        _head = null;
        _tail = null;
        _count = 0;
    }

    public bool Contains(object? value)
    {
        return IndexOf(value) >= 0;
    }

    public void CopyTo(Array array, int index)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (array.Length - index < _count)
            throw new ArgumentException("Array too small");

        int currentIndex = index;
        Node? current = _head;
        while (current != null)
        {
            array.SetValue(current.Value, currentIndex++);
            current = current.Next;
        }
    }

    public IEnumerator GetEnumerator()
    {
        var current = _head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    public int IndexOf(object? value)
    {
        int index = 0;
        var current = _head;
        while (current != null)
        {
            if (Equals(current.Value, value))
                return index;
            current = current.Next;
            index++;
        }
        return -1;
    }

    public void Insert(int index, object? value)
    {
        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Node newNode = new Node { Value = value };

        if (index == 0)
        {
            // Вставка в начало
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
            // Вставка в конец
            _tail!.Next = newNode;
            newNode.Previous = _tail;
            _tail = newNode;
        }
        else
        {
            // Вставка в середину
            Node node = GetNodeAt(index);
            newNode.Next = node;
            newNode.Previous = node.Previous;
            node.Previous!.Next = newNode;
            node.Previous = newNode;
        }

        _count++;
    }

    public void Remove(object? value)
    {
        int index = IndexOf(value);
        if (index >= 0)
        {
            RemoveAt(index);
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Node node = GetNodeAt(index);

        // Обновляем ссылку предыдущего узла
        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        else
        {
            _head = node.Next;
        }

        // Обновляем ссылку следующего узла
        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
        else
        {
            _tail = node.Previous;
        }

        _count--;
    }

    private Node GetNodeAt(int index)
    {
        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        // Оптимизация: выбираем направление обхода
        Node? current;
        if (index < _count / 2)
        {
            // Идем с начала
            current = _head;
            for (int i = 0; i < index; i++)
            {
                current = current!.Next;
            }
        }
        else
        {
            // Идем с конца
            current = _tail;
            for (int i = _count - 1; i > index; i--)
            {
                current = current!.Previous;
            }
        }

        return current!;
    }
}

