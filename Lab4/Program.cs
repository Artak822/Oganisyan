using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Лабораторная работа 4: Синхронизация потоков\n");
        
        Console.WriteLine("1. Обедающие философы (с deadlock)");
        await DiningPhilosophersWithDeadlock.Run();
        
        Console.WriteLine("\n2. Обедающие философы (без deadlock)");
        await DiningPhilosophersFixed.Run();
        
        Console.WriteLine("\n3. Спящий парикмахер");
        await SleepingBarber.Run();
        
        Console.WriteLine("\n4. Производитель-Потребитель");
        await ProducerConsumer.Run();
    }
}
