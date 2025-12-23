using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

class ProducerConsumer
{
    private static readonly BlockingCollection<int> Buffer = new BlockingCollection<int>(boundedCapacity: 5);
    private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
    
    public static async Task Run()
    {
        Task producer1 = Task.Run(() => Producer(1));
        Task producer2 = Task.Run(() => Producer(2));
        Task consumer1 = Task.Run(() => Consumer(1));
        Task consumer2 = Task.Run(() => Consumer(2));
        
        await Task.Delay(5000);
        
        Cts.Cancel();
        Buffer.CompleteAdding();
        
        await Task.WhenAll(producer1, producer2, consumer1, consumer2);
    }
    
    static void Producer(int id)
    {
        int item = 0;
        while (!Cts.Token.IsCancellationRequested)
        {
            try
            {
                item++;
                Buffer.Add(item, Cts.Token);
                Console.WriteLine($"Производитель {id} добавил товар {item}");
                Thread.Sleep(Random.Shared.Next(300, 800));
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
        Console.WriteLine($"Производитель {id} завершил работу");
    }
    
    static void Consumer(int id)
    {
        foreach (var item in Buffer.GetConsumingEnumerable())
        {
            Console.WriteLine($"Потребитель {id} забрал товар {item}");
            Thread.Sleep(Random.Shared.Next(500, 1000));
        }
        Console.WriteLine($"Потребитель {id} завершил работу");
    }
}

