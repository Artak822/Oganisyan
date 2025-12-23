using System;
using System.Threading;
using System.Threading.Tasks;

class DiningPhilosophersWithDeadlock
{
    private static readonly object[] Forks = new object[5];
    private static readonly Thread[] Philosophers = new Thread[5];
    
    public static async Task Run()
    {
        for (int i = 0; i < 5; i++)
        {
            Forks[i] = new object();
        }
        
        for (int i = 0; i < 5; i++)
        {
            int id = i;
            Philosophers[i] = new Thread(() => Philosopher(id));
            Philosophers[i].Start();
        }
        
        await Task.Delay(3000);
        
        foreach (var philosopher in Philosophers)
        {
            philosopher.Interrupt();
        }
    }
    
    static void Philosopher(int id)
    {
        int leftFork = id;
        int rightFork = (id + 1) % 5;
        
        while (true)
        {
            try
            {
                Console.WriteLine($"Философ {id} думает...");
                Thread.Sleep(Random.Shared.Next(500, 1000));
                
                Console.WriteLine($"Философ {id} хочет есть");
                
                lock (Forks[leftFork])
                {
                    Console.WriteLine($"Философ {id} взял левую вилку {leftFork}");
                    Thread.Sleep(100);
                    
                    lock (Forks[rightFork])
                    {
                        Console.WriteLine($"Философ {id} взял правую вилку {rightFork} и ест");
                        Thread.Sleep(Random.Shared.Next(500, 1000));
                        Console.WriteLine($"Философ {id} закончил есть");
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                break;
            }
        }
    }
}

