using System.Text.Json;

namespace Lab1
{
    public class PersonSerializer
    {
        private readonly JsonSerializerOptions _options;

        public PersonSerializer()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Не экранировать Unicode
            };
        }

        // 1. Сериализация в строку
        public string SerializeToJson(Person person)
        {
            return JsonSerializer.Serialize(person, _options);
        }

        // 2. Десериализация из строки
        public Person DeserializeFromJson(string json)
        {
            return JsonSerializer.Deserialize<Person>(json, _options) 
                ?? throw new JsonException("Не удалось десериализовать Person из JSON");
        }

        // 3. Сохранение в файл (синхронно)
        public void SaveToFile(Person person, string filePath)
        {
            string json = SerializeToJson(person);
            File.WriteAllText(filePath, json);
        }

        // 4. Загрузка из файла (синхронно)
        public Person LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            string json = File.ReadAllText(filePath);
            return DeserializeFromJson(json);
        }

        // 5. Сохранение в файл (асинхронно)
        public async Task SaveToFileAsync(Person person, string filePath)
        {
            string json = SerializeToJson(person);
            await File.WriteAllTextAsync(filePath, json);
        }

        // 6. Загрузка из файла (асинхронно)
        public async Task<Person> LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            string json = await File.ReadAllTextAsync(filePath);
            return DeserializeFromJson(json);
        }

        // 7. Экспорт нескольких объектов в файл
        public void SaveListToFile(List<Person> people, string filePath)
        {
            string json = JsonSerializer.Serialize(people, _options);
            File.WriteAllText(filePath, json);
        }

        // 8. Импорт из файла
        public List<Person> LoadListFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Person>>(json, _options) 
                ?? throw new JsonException("Не удалось десериализовать список Person из JSON");
        }
    }
}

