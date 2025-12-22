using System.Collections.Immutable;
using System.Diagnostics;

namespace Lab2;

    public class PerformanceBenchmark
    {
        private const int CollectionSize = 100000;
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

            Console.WriteLine("Запуск бенчмарков...");
            Console.WriteLine($"Размер: {CollectionSize} элементов");
            Console.WriteLine();

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

        results.Add(Measure(() =>
        {
            var list = new List<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.Add(i);
            }
        }, "List", "Добавление в конец"));

        results.Add(Measure(() =>
        {
            var list = new List<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.Insert(0, i);
            }
        }, "List", "Добавление в начало"));

        results.Add(Measure(() =>
        {
            var list = new List<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.Insert(list.Count / 2, i);
            }
        }, "List", "Добавление в середину"));

        var listForRemoval = new List<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempList = new List<int>(listForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveAt(tempList.Count - 1);
            }
        }, "List", "Удаление из конца"));

        results.Add(Measure(() =>
        {
            var tempList = new List<int>(listForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveAt(0);
            }
        }, "List", "Удаление из начала"));

        results.Add(Measure(() =>
        {
            var tempList = new List<int>(listForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveAt(tempList.Count / 2);
            }
        }, "List", "Удаление из середины"));

        var listForSearch = new List<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                listForSearch.IndexOf(i % CollectionSize);
            }
        }, "List", "Поиск элемента"));

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

        results.Add(Measure(() =>
        {
            var list = new LinkedList<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.AddLast(i);
            }
        }, "LinkedList", "Добавление в конец"));

        results.Add(Measure(() =>
        {
            var list = new LinkedList<int>();
            for (int i = 0; i < CollectionSize; i++)
            {
                list.AddFirst(i);
            }
        }, "LinkedList", "Добавление в начало"));

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

        var linkedListForRemoval = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempList = new LinkedList<int>(linkedListForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveLast();
            }
        }, "LinkedList", "Удаление из конца"));

        results.Add(Measure(() =>
        {
            var tempList = new LinkedList<int>(linkedListForRemoval);
            while (tempList.Count > 0)
            {
                tempList.RemoveFirst();
            }
        }, "LinkedList", "Удаление из начала"));

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

        var linkedListForSearch = new LinkedList<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                linkedListForSearch.Find(i % CollectionSize);
            }
        }, "LinkedList", "Поиск элемента"));

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

        var queueForRemoval = new Queue<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempQueue = new Queue<int>(queueForRemoval);
            while (tempQueue.Count > 0)
            {
                tempQueue.Dequeue();
            }
        }, "Queue", "Удаление из начала (Dequeue)"));

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

        var stackForRemoval = new Stack<int>(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempStack = new Stack<int>(stackForRemoval);
            while (tempStack.Count > 0)
            {
                tempStack.Pop();
            }
        }, "Stack", "Удаление из конца (Pop)"));

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

        results.Add(Measure(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int i = 0; i < CollectionSize; i++)
            {
                list = list.Add(i);
            }
        }, "ImmutableList", "Добавление в конец"));

        results.Add(Measure(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int i = 0; i < CollectionSize; i++)
            {
                list = list.Insert(0, i);
            }
        }, "ImmutableList", "Добавление в начало"));

        results.Add(Measure(() =>
        {
            var list = ImmutableList<int>.Empty;
            for (int i = 0; i < CollectionSize; i++)
            {
                list = list.Insert(list.Count / 2, i);
            }
        }, "ImmutableList", "Добавление в середину"));

        var immutableListForRemoval = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            var tempList = immutableListForRemoval;
            while (tempList.Count > 0)
            {
                tempList = tempList.RemoveAt(tempList.Count - 1);
            }
        }, "ImmutableList", "Удаление из конца"));

        results.Add(Measure(() =>
        {
            var tempList = immutableListForRemoval;
            while (tempList.Count > 0)
            {
                tempList = tempList.RemoveAt(0);
            }
        }, "ImmutableList", "Удаление из начала"));

        results.Add(Measure(() =>
        {
            var tempList = immutableListForRemoval;
            while (tempList.Count > 0)
            {
                tempList = tempList.RemoveAt(tempList.Count / 2);
            }
        }, "ImmutableList", "Удаление из середины"));

        var immutableListForSearch = ImmutableList<int>.Empty.AddRange(Enumerable.Range(0, CollectionSize));
        results.Add(Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                immutableListForSearch.IndexOf(i % CollectionSize);
            }
        }, "ImmutableList", "Поиск элемента"));

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

