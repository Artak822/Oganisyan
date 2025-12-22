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
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public string SerializeToJson(Person person)
        {
            return JsonSerializer.Serialize(person, _options);
        }

        public Person DeserializeFromJson(string json)
        {
            return JsonSerializer.Deserialize<Person>(json, _options) 
                ?? throw new JsonException("Не удалось десериализовать Person из JSON");
        }

        public void SaveToFile(Person person, string filePath)
        {
            string json = SerializeToJson(person);
            File.WriteAllText(filePath, json);
        }

        public Person LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            string json = File.ReadAllText(filePath);
            return DeserializeFromJson(json);
        }

        public async Task SaveToFileAsync(Person person, string filePath)
        {
            string json = SerializeToJson(person);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<Person> LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            string json = await File.ReadAllTextAsync(filePath);
            return DeserializeFromJson(json);
        }

        public void SaveListToFile(List<Person> people, string filePath)
        {
            string json = JsonSerializer.Serialize(people, _options);
            File.WriteAllText(filePath, json);
        }

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

