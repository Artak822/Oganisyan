using System.Collections;
using System.Collections.Generic;

namespace Lab3.Collections;

public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private class Bucket
    {
        public KeyValuePair<TKey, TValue>? Item { get; set; }
        public Bucket? Next { get; set; }
    }

    private Bucket[] _buckets;
    private int _count;
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;

    public SimpleDictionary()
    {
        _buckets = new Bucket[DefaultCapacity];
        _count = 0;
    }

    public SimpleDictionary(int capacity)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _buckets = new Bucket[capacity == 0 ? DefaultCapacity : capacity];
        _count = 0;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue? value))
                return value;
            throw new KeyNotFoundException();
        }
        set
        {
            AddOrUpdate(key, value);
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            foreach (var pair in this)
            {
                keys.Add(pair.Key);
            }
            return keys;
        }
    }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>();
            foreach (var pair in this)
            {
                values.Add(pair.Value);
            }
            return values;
        }
    }

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    public int Count => _count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        
        if (ContainsKey(key))
            throw new ArgumentException("Key already exists");

        AddOrUpdate(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(_buckets, 0, _buckets.Length);
        _count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue? value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        
        return TryGetValue(key, out _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < _count)
            throw new ArgumentException("Array too small");

        int currentIndex = arrayIndex;
        foreach (var pair in this)
        {
            array[currentIndex++] = pair;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var bucket in _buckets)
        {
            var current = bucket;
            while (current != null && current.Item.HasValue)
            {
                yield return current.Item.Value;
                current = current.Next;
            }
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

        int bucketIndex = GetBucketIndex(key);
        Bucket? bucket = _buckets[bucketIndex];

        if (bucket == null || !bucket.Item.HasValue)
            return false;

        // Проверяем первый элемент в цепочке
        if (bucket.Item.Value.Key.Equals(key))
        {
            _buckets[bucketIndex] = bucket.Next;
            _count--;
            return true;
        }

        // Ищем в остальной части цепочки
        Bucket? previous = bucket;
        Bucket? current = bucket.Next;
        
        while (current != null && current.Item.HasValue)
        {
            if (current.Item.Value.Key.Equals(key))
            {
                previous.Next = current.Next;
                _count--;
                return true;
            }
            previous = current;
            current = current.Next;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
        {
            return Remove(item.Key);
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int bucketIndex = GetBucketIndex(key);
        Bucket? bucket = _buckets[bucketIndex];

        while (bucket != null && bucket.Item.HasValue)
        {
            if (bucket.Item.Value.Key.Equals(key))
            {
                value = bucket.Item.Value.Value;
                return true;
            }
            bucket = bucket.Next;
        }

        value = default(TValue)!;
        return false;
    }

    private void AddOrUpdate(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        // Проверяем необходимость рехеширования
        if ((double)_count / _buckets.Length >= LoadFactor)
        {
            Resize();
        }

        int bucketIndex = GetBucketIndex(key);
        Bucket? bucket = _buckets[bucketIndex];

        // Если корзина пустая, создаем первый элемент
        if (bucket == null || !bucket.Item.HasValue)
        {
            _buckets[bucketIndex] = new Bucket 
            { 
                Item = new KeyValuePair<TKey, TValue>(key, value) 
            };
            _count++;
            return;
        }

        // Ищем ключ в цепочке или добавляем в конец
        Bucket? current = bucket;
        while (current != null && current.Item.HasValue)
        {
            if (current.Item.Value.Key.Equals(key))
            {
                // Обновляем существующий ключ
                current.Item = new KeyValuePair<TKey, TValue>(key, value);
                return;
            }
            
            if (current.Next == null)
            {
                // Добавляем в конец цепочки
                current.Next = new Bucket 
                { 
                    Item = new KeyValuePair<TKey, TValue>(key, value) 
                };
                _count++;
                return;
            }
            
            current = current.Next;
        }
    }

    private int GetBucketIndex(TKey key)
    {
        int hashCode = key.GetHashCode();
        return Math.Abs(hashCode) % _buckets.Length;
    }

    private void Resize()
    {
        Bucket[] oldBuckets = _buckets;
        _buckets = new Bucket[_buckets.Length * 2];
        _count = 0;

        // Перехешируем все элементы
        foreach (Bucket? bucket in oldBuckets)
        {
            Bucket? current = bucket;
            while (current != null && current.Item.HasValue)
            {
                KeyValuePair<TKey, TValue> pair = current.Item.Value;
                AddOrUpdate(pair.Key, pair.Value);
                current = current.Next;
            }
        }
    }
}

