using Lab2;

namespace Lab2;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Лабораторная работа 2: Работа с коллекциями ===\n");

        var benchmark = new PerformanceBenchmark();
        var results = benchmark.RunAllBenchmarks();

        Console.WriteLine("\n=== Результаты замеров производительности ===\n");
        PrintResults(results);
    }

    static void PrintResults(List<PerformanceBenchmark.BenchmarkResult> results)
    {
        var grouped = results.GroupBy(r => r.Operation);

        foreach (var group in grouped)
        {
            Console.WriteLine($"Операция: {group.Key}");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine($"{"Коллекция",-20} {"Среднее (мс)",-15} {"Мин (мс)",-15} {"Макс (мс)",-15}");
            Console.WriteLine(new string('-', 80));

            foreach (var result in group.OrderBy(r => r.AverageTimeMs))
            {
                Console.WriteLine($"{result.CollectionType,-20} {result.AverageTimeMs,-15:F2} {result.MinTimeMs,-15:F2} {result.MaxTimeMs,-15:F2}");
            }

            Console.WriteLine();
        }
    }

}
