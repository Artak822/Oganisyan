using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class SleepingBarber
{
    private static readonly SemaphoreSlim Customers = new SemaphoreSlim(0);
    private static readonly SemaphoreSlim BarberReady = new SemaphoreSlim(0);
    private static readonly Mutex Mutex = new Mutex();
    private static readonly Queue<int> WaitingRoom = new Queue<int>();
    private static readonly int MaxSeats = 3;
    private static bool BarberSleeping = true;
    
    public static async Task Run()
    {
        Thread barberThread = new Thread(Barber);
        barberThread.Start();
        
        Thread[] customerThreads = new Thread[10];
        for (int i = 0; i < 10; i++)
        {
            int customerId = i;
            customerThreads[i] = new Thread(() => Customer(customerId));
            customerThreads[i].Start();
            await Task.Delay(Random.Shared.Next(200, 500));
        }
        
        await Task.Delay(5000);
        
        barberThread.Interrupt();
    }
    
    static void Barber()
    {
        while (true)
        {
            try
            {
                if (WaitingRoom.Count == 0)
                {
                    BarberSleeping = true;
                    Console.WriteLine("Парикмахер спит");
                }
                
                Customers.Wait();
                
                BarberSleeping = false;
                int customerId;
                
                Mutex.WaitOne();
                customerId = WaitingRoom.Dequeue();
                Mutex.ReleaseMutex();
                
                Console.WriteLine($"Парикмахер стрижет клиента {customerId}");
                Thread.Sleep(Random.Shared.Next(1000, 2000));
                Console.WriteLine($"Парикмахер закончил стричь клиента {customerId}");
                
                BarberReady.Release();
            }
            catch (ThreadInterruptedException)
            {
                break;
            }
        }
    }
    
    static void Customer(int id)
    {
        Mutex.WaitOne();
        
        if (WaitingRoom.Count < MaxSeats)
        {
            WaitingRoom.Enqueue(id);
            Console.WriteLine($"Клиент {id} сел в очередь. Мест занято: {WaitingRoom.Count}/{MaxSeats}");
            Mutex.ReleaseMutex();
            
            Customers.Release();
            
            BarberReady.Wait();
            Console.WriteLine($"Клиент {id} уходит");
        }
        else
        {
            Mutex.ReleaseMutex();
            Console.WriteLine($"Клиент {id} ушел - нет мест");
        }
    }
}

