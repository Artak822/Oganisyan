using System.Collections;

namespace Lab3.Collections;

public class SimpleList : IEnumerable, ICollection, IList
{
    private object[] _items;
    private int _count;
    private const int DefaultCapacity = 4;

    public SimpleList()
    {
        _items = new object[DefaultCapacity];
        _count = 0;
    }

    public SimpleList(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));
        
        _items = new object[capacity];
        _count = 0;
    }

    public int Count => _count;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public object? this[int index]
    {
        get
        {
            ValidateIndex(index);
            return _items[index];
        }
        set
        {
            ValidateIndex(index);
            _items[index] = value;
        }
    }

    public int Add(object? value)
    {
        EnsureCapacity();
        _items[_count] = value;
        return _count++;
    }

    public void Clear()
    {
        Array.Clear(_items, 0, _count);
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

        Array.Copy(_items, 0, array, index, _count);
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }

    public int IndexOf(object? value)
    {
        for (int i = 0; i < _count; i++)
        {
            if (Equals(_items[i], value))
                return i;
        }
        return -1;
    }

    public void Insert(int index, object? value)
    {
        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        EnsureCapacity();

        if (index < _count)
        {
            Array.Copy(_items, index, _items, index + 1, _count - index);
        }

        _items[index] = value;
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

        _count--;
        if (index < _count)
        {
            Array.Copy(_items, index + 1, _items, index, _count - index);
        }
        _items[_count] = null!;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));
    }

    private void EnsureCapacity()
    {
        if (_count >= _items.Length)
        {
            Resize();
        }
    }

    private void Resize()
    {
        int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
        object[] newItems = new object[newCapacity];
        Array.Copy(_items, newItems, _count);
        _items = newItems;
    }
}

