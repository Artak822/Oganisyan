using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static List<int> list = new List<int>();
    static Mutex mutex = new Mutex();
    
    static void AddData()
    {
        for (int i = 0; i < 10; i++)
        {
            mutex.WaitOne();
            list.Add(i);
            mutex.ReleaseMutex();
        }
    }
    
    static void Main()
    {
        Thread t1 = new Thread(AddData);
        Thread t2 = new Thread(AddData);
        
        t1.Start();
        t2.Start();
        
        t1.Join();
        t2.Join();
        
        Console.WriteLine($"Элементов: {list.Count}");
    }
}