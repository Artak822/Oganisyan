using Lab1.Tests;

namespace Lab1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await PersonSerializerTests.RunAllTests();
        }
    }
}

