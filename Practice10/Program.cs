using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


enum WeatherType
{
    Sunny,
    Cloudy,
    Rainy,
    Snowy,
    Foggy
}

class WeatherEntry
{
    public DateTime Date { get; set; }
    public int Temperature { get; set; }
    public WeatherType Weather { get; set; }

    public WeatherEntry(DateTime date, int temperature, WeatherType weather)
    {
        Date = date;
        Temperature = temperature;
        Weather = weather;
    }

    public override string ToString()
    {
        string weatherStr = Weather switch
        {
            WeatherType.Sunny => "солнечно",
            WeatherType.Cloudy => "облачно",
            WeatherType.Rainy => "дождь",
            WeatherType.Snowy => "снег",
            WeatherType.Foggy => "туман",
            _ => Weather.ToString()
        };
        
        return $"Дата: {Date:dd-MM-yy}\nТемпература: {Temperature}\nПогода: {weatherStr}";
    }
}

class WeatherDiary
{
    private List<WeatherEntry> entries;
    private const string FilePath = "wether.txt";

    public WeatherDiary()
    {
        entries = new List<WeatherEntry>();
    }


    public void SaveEntires()
    {
        try
        {
            using (StreamWriter writher = new StreamWriter(FilePath))
            {
                foreach (var entry in entries)
                {
                    writher.WriteLine($"{entry.Date:dd-MM-yy}|{entry.Temperature}|{entry.Weather}");
                }
                Console.WriteLine("Записи сохранены в файл");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении записей: {ex.Message}");
        }
    }


    public void AddEntry(DateTime date, int temperature, WeatherType weather)
    {
        entries.Add(new WeatherEntry(date, temperature, weather));
        Console.WriteLine("\nЗапись добавлена!");
        SaveEntires();
    }

    public double GetAverageTemperatureForWeek()
    {
        if (entries.Count == 0) return 0;

        var lastWeek = entries
            .Where(e => e.Date >= DateTime.Now.AddDays(-7))
            .ToList();

        if (lastWeek.Count == 0) return 0;

        return lastWeek.Average(e => e.Temperature);
    }

    public int GetSunnyDaysCount()
    {
        return entries.Count(e => e.Weather == WeatherType.Sunny);
    }

    public int GetHighestTemperature()
    {
        if (entries.Count == 0) return 0;
        return entries.Max(e => e.Temperature);
    }

    public int GetLowestTemperature()
    {
        if (entries.Count == 0) return 0;
        return entries.Min(e => e.Temperature);
    }

    public void DisplayAllEntries()
    {
        if (entries.Count == 0)
        {
            Console.WriteLine("\nЗаписей пока нет.");
            return;
        }

        Console.WriteLine("\n=== Все записи ===");
        foreach (var entry in entries.OrderBy(e => e.Date))
        {
            Console.WriteLine(entry);
            Console.WriteLine();
        }
    }

    public void DisplayStatistics()
    {
        Console.WriteLine("\n=== Статистика ===");
        Console.WriteLine($"Средняя температура за неделю: {GetAverageTemperatureForWeek():F2}°C");
        Console.WriteLine($"Количество солнечных дней: {GetSunnyDaysCount()}");
        Console.WriteLine($"Самая высокая температура: {GetHighestTemperature()}°C");
        Console.WriteLine($"Самая низкая температура: {GetLowestTemperature()}°C");
    }
}

class Program
{
    static void Main(string[] args)
    {
        
        WeatherDiary diary = new WeatherDiary();

        while (true)
        {
            Console.WriteLine("\n=== Дневник погоды ===");
            Console.WriteLine("1. Добавить запись");
            Console.WriteLine("2. Показать все записи");
            Console.WriteLine("3. Показать статистику");
            Console.WriteLine("4. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    AddEntry(diary);
                    break;
                case "2":
                    diary.DisplayAllEntries();
                    break;
                case "3":
                    diary.DisplayStatistics();
                    break;
                case "4":
                    Console.WriteLine("До свидания!");
                    diary.SaveEntires();
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    static void AddEntry(WeatherDiary diary)
    {
        Console.Write("Введите дату (dd-MM-yy) или нажмите Enter для сегодня: ");
        string dateInput = Console.ReadLine() ?? "";
        DateTime date;

        if (string.IsNullOrWhiteSpace(dateInput))
        {
            date = DateTime.Now;
        }
        else
        {
            if (!DateTime.TryParse(dateInput, out date))
            {
                Console.WriteLine("Неверный формат даты. Используется сегодняшняя дата.");
                date = DateTime.Now;
            }
        }

        Console.Write("Введите температуру: ");
        if (!int.TryParse(Console.ReadLine(), out int temperature))
        {
            Console.WriteLine("Неверный формат температуры. Запись не добавлена.");
            return;
        }

        Console.WriteLine("Выберите погоду:");
        Console.WriteLine("1. Солнечно");
        Console.WriteLine("2. Облачно");
        Console.WriteLine("3. Дождь");
        Console.WriteLine("4. Снег");
        Console.WriteLine("5. Туман");
        Console.Write("Ваш выбор: ");
        
        string choice = Console.ReadLine() ?? "";
        WeatherType weather = choice switch
        {
            "1" => WeatherType.Sunny,
            "2" => WeatherType.Cloudy,
            "3" => WeatherType.Rainy,
            "4" => WeatherType.Snowy,
            "5" => WeatherType.Foggy,
            _ => WeatherType.Cloudy 
        };
        diary.AddEntry(date, temperature, weather);
    }
}
