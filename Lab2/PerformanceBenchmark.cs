using System.Collections.Immutable;
using System.Diagnostics;

namespace Lab2;

public class PerformanceBenchmark
{
    private const int CollectionSize = 100_000;
    private const int WarmupIterations = 3;
    private const int MeasurementIterations = 5;

    public class BenchmarkResult
    {
        public string CollectionType { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public double AverageTimeMs { get; set; }
        public double MinTimeMs { get; set; }
        public double MaxTimeMs { get; set; }
    }

    public List<BenchmarkResult> RunAllBenchmarks()
    {
        var results = new List<BenchmarkResult>();

        Console.WriteLine("Запуск бенчмарков производительности...");
        Console.WriteLine($"Размер коллекций: {CollectionSize:N0} элементов");
        Console.WriteLine($"Количество итераций: {MeasurementIterations}");
        Console.WriteLine();

        // List benchmarks
        results.AddRange(BenchmarkList());
        results.AddRange(BenchmarkLinkedList());
        results.AddRange(BenchmarkQueue());
        results.AddRange(BenchmarkStack());
        results.AddRange(BenchmarkImmutableList());

        return results;
    }

    private List<BenchmarkResult> BenchmarkList()
    {
        var results = new List<BenchmarkResult>();
        Console.WriteLine("Тестирование List<T>...");

        // Добавление в конец
        results.Add(Measure(() =>
        {
            var list = new List<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.Add(i);
            }
        }, "List", "Добавление в конец"));

        // Добавление в начало
        results.Add(Measure(() =>
        {
            var list = new List<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.Insert(0, i);
            }
        }, "List", "Добавление в начало"));

        // Добавление в середину
        results.Add(Measure(() =>
        {
            var list = new List<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.Insert(list.Count / 2, i);
            }
        }, "List", "Добавление в середину"));

        // Удаление из конца
        var listForRemoval = new List<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempList = new List<int>(listForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveAt(tempList.Count - 1);
            }
        }, "List", "Удаление из конца"));

        // Удаление из начала
        results.Add(Measure(() =>
        {
            var tempList = new List<int>(listForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveAt(0);
            }
        }, "List", "Удаление из начала"));

        // Удаление из середины
        results.Add(Measure(() =>
        {
            var tempList = new List<int>(listForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveAt(tempList.Count / 2);
            }
        }, "List", "Удаление из середины"));

        // Поиск элемента
        var listForSearch = new List<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                listForSearch.IndexOf(i % CollectionSize);
            }
        }, "List", "Поиск элемента"));

        // Получение по индексу
        results.Add(Measure(() =>
        {
            int sum = 0;
            for (int i = 0; i < 100000; i++)
            {
                sum += listForSearch[i % CollectionSize];
            }
        }, "List", "Получение по индексу"));

        return results;
    }

    private List<BenchmarkResult> BenchmarkLinkedList()
    {
        var results = new List<BenchmarkResult>();
        Console.WriteLine("Тестирование LinkedList<T>...");

        // Добавление в конец
        results.Add(Measure(() =>
        {
            var list = new LinkedList<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.AddLast(i);
            }
        }, "LinkedList", "Добавление в конец"));

        // Добавление в начало
        results.Add(Measure(() =>
        {
            var list = new LinkedList<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.AddFirst(i);
            }
        }, "LinkedList", "Добавление в начало"));

        // Добавление в середину
        results.Add(Measure(() =>
        {
            var list = new LinkedList<int>();
            var middle = list.AddFirst(0);
            for (int i = 1; i < CollectionSize; i++)
            {
                if (i % 2 == 0)
                {
                    list.AddAfter(middle, i);
                }
                else
                {
                    list.AddBefore(middle, i);
                }
            }
        }, "LinkedList", "Добавление в середину"));

        // Удаление из конца
        var linkedListForRemoval = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempList = new LinkedList<int>(linkedListForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveLast();
            }
        }, "LinkedList", "Удаление из конца"));

        // Удаление из начала
        results.Add(Measure(() =>
        {
            var tempList = new LinkedList<int>(linkedListForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveFirst();
            }
        }, "LinkedList", "Удаление из начала"));

        // Удаление из середины
        results.Add(Measure(() =>
        {
            var tempList = new LinkedList<int>(linkedListForRemoval);
            while (tempList.Count > 0)
            {
                var node = tempList.First;
                for (int i = 0; i < tempList.Count / 2 && node != null; i++)
                {
                    node = node.Next;
                }
                if (node != null)
                {
                    tempList.Remove(node);
                }
            }
        }, "LinkedList", "Удаление из середины"));

        // Поиск элемента
        var linkedListForSearch = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                linkedListForSearch.Find(i % CollectionSize);
            }
        }, "LinkedList", "Поиск элемента"));

        // Получение по индексу (неэффективно для LinkedList)
        results.Add(Measure(() =>
        {
            for (int idx = 0; idx < 100; idx++)
            {
                var node = linkedListForSearch.First;
                for (int i = 0; i < (idx * 100) % CollectionSize && node != null; i++)
                {
                    node = node.Next;
                }
            }
        }, "LinkedList", "Получение по индексу"));

        return results;
    }

    private List<BenchmarkResult> BenchmarkQueue()
    {
        var results = new List<BenchmarkResult>();
        Console.WriteLine("Тестирование Queue<T>...");

        // Добавление в конец (Enqueue)
        results.Add(Measure(() =>
        {
            var queue = new Queue<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                queue.Enqueue(i);
            }
        }, "Queue", "Добавление в конец (Enqueue)"));

        // Удаление из начала (Dequeue)
        var queueForRemoval = new Queue<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempQueue = new Queue<int>(queueForRemoval);
            while (tempQueue.Count > 0)
            {
                tempQueue.Dequeue();
            }
        }, "Queue", "Удаление из начала (Dequeue)"));

        // Поиск элемента
        var queueForSearch = new Queue<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                queueForSearch.Contains(i % CollectionSize);
            }
        }, "Queue", "Поиск элемента"));

        return results;
    }

    private List<BenchmarkResult> BenchmarkStack()
    {
        var results = new List<BenchmarkResult>();
        Console.WriteLine("Тестирование Stack<T>...");

        // Добавление в конец (Push)
        results.Add(Measure(() =>
        {
            var stack = new Stack<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                stack.Push(i);
            }
        }, "Stack", "Добавление в конец (Push)"));

        // Удаление из конца (Pop)
        var stackForRemoval = new Stack<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempStack = new Stack<int>(stackForRemoval);
            while (tempStack.Count > 0)
            {
                tempStack.Pop();
            }
        }, "Stack", "Удаление из конца (Pop)"));

        // Поиск элемента
        var stackForSearch = new Stack<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                stackForSearch.Contains(i % CollectionSize);
            }
        }, "Stack", "Поиск элемента"));

        return results;
    }

    private List<BenchmarkResult> BenchmarkImmutableList()
    {
        var results = new List<BenchmarkResult>();
        Console.WriteLine("Тестирование ImmutableList<T>...");

        // Добавление в конец
        results.Add(Measure(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int i = 0; i < CollectionSize; i++)
            {
                list = list.Add(i);
            }
        }, "ImmutableList", "Добавление в конец"));

        // Добавление в начало
        results.Add(Measure(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int i = 0; i < CollectionSize; i++)
            {
                list = list.Insert(0, i);
            }
        }, "ImmutableList", "Добавление в начало"));

        // Добавление в середину
        results.Add(Measure(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int i = 0; i < CollectionSize; i++)
            {
                list = list.Insert(list.Count / 2, i);
            }
        }, "ImmutableList", "Добавление в середину"));

        // Удаление из конца
        var immutableListForRemoval = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempList = immutableListForRemoval;
            while (tempList.Count > 0)
            {
                tempList = tempList.RemoveAt(tempList.Count - 1);
            }
        }, "ImmutableList", "Удаление из конца"));

        // Удаление из начала
        results.Add(Measure(() =>
        {
            var tempList = immutableListForRemoval;
            while (tempList.Count > 0)
            {
                tempList = tempList.RemoveAt(0);
            }
        }, "ImmutableList", "Удаление из начала"));

        // Удаление из середины
        results.Add(Measure(() =>
        {
            var tempList = immutableListForRemoval;
            while (tempList.Count > 0)
            {
                tempList = tempList.RemoveAt(tempList.Count / 2);
            }
        }, "ImmutableList", "Удаление из середины"));

        // Поиск элемента
        var immutableListForSearch = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                immutableListForSearch.IndexOf(i % CollectionSize);
            }
        }, "ImmutableList", "Поиск элемента"));

        // Получение по индексу
        results.Add(Measure(() =>
        {
            int sum = 0;
            for (int i = 0; i < 100000; i++)
            {
                sum += immutableListForSearch[i % CollectionSize];
            }
        }, "ImmutableList", "Получение по индексу"));

        return results;
    }

    private BenchmarkResult Measure(Action action, string collectionType, string operation)
    {
        // Прогрев
        for (int i = 0; i < WarmupIterations; i++)
        {
            action();
        }

        var times = new List<double>();

        for (int i = 0; i < MeasurementIterations; i++)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return new BenchmarkResult
        {
            CollectionType = collectionType,
            Operation = operation,
            AverageTimeMs = times.Average(),
            MinTimeMs = times.Min(),
            MaxTimeMs = times.Max()
        };
    }
}

