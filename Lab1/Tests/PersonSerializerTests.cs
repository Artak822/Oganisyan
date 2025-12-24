using System.Text.Json;
using Lab1.Models;
using Lab1.Services;

namespace Lab1.Tests
{
    public class PersonSerializerTests
    {
        private readonly PersonSerializer _serializer;
        private readonly string _testDirectory;

        public PersonSerializerTests()
        {
            _serializer = new PersonSerializer();
            _testDirectory = Path.Combine(Path.GetTempPath(), "Lab1Tests");
            
            if (!Directory.Exists(_testDirectory))
            {
                Directory.CreateDirectory(_testDirectory);
            }
        }

        private Person CreateTestPerson()
        {
            return new Person
            {
                FirstName = "Artak",
                LastName = "Oganisyan",
                Age = 18,
                Password = "secret123",
                Id = "12345",
                BirthDate = new DateTime(1999, 1, 15),
                Email = "artak@example.com",
                PhoneNumber = "+7-999-123-45-67"
            };
        }

        private void CleanupTestFiles()
        {
            if (Directory.Exists(_testDirectory))
            {
                var files = Directory.GetFiles(_testDirectory);
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }

        public void TestSerializeToJson()
        {
            Console.WriteLine("Тест 1: Сериализация в JSON");
            
            var person = CreateTestPerson();
            string json = _serializer.SerializeToJson(person);

            bool hasFirstName = json.Contains("Иван") || json.Contains("\\u0418\\u0432\\u0430\\u043D") || json.Contains("\\u0418\\u0432\\u0430\\u043d");
            Assert(hasFirstName, $"JSON должен содержать FirstName. JSON: {json.Substring(0, Math.Min(200, json.Length))}");
            
            bool hasLastName = json.Contains("Иванов") || json.Contains("\\u0418\\u0432\\u0430\\u043D\\u043E\\u0432") || json.Contains("\\u0418\\u0432\\u0430\\u043d\\u043e\\u0432");
            Assert(hasLastName, "JSON должен содержать LastName");
            Assert(json.Contains("25"), "JSON должен содержать Age");
            Assert(json.Contains("personId"), "JSON должен содержать 'personId' вместо 'Id'");
            Assert(json.Contains("phone"), "JSON должен содержать 'phone' вместо 'PhoneNumber'");
            Assert(!json.Contains("Password"), "JSON НЕ должен содержать Password");
            Assert(!json.Contains("secret123"), "JSON НЕ должен содержать значение Password");

            Console.WriteLine("Тест пройден\n");
        }

        public void TestDeserializeFromJson()
        {
            Console.WriteLine("Тест 2: Десериализация из JSON");
            
            var originalPerson = CreateTestPerson();
            string json = _serializer.SerializeToJson(originalPerson);
            Person deserializedPerson = _serializer.DeserializeFromJson(json);

            Assert(deserializedPerson.FirstName == originalPerson.FirstName, "FirstName должен совпадать");
            Assert(deserializedPerson.LastName == originalPerson.LastName, "LastName должен совпадать");
            Assert(deserializedPerson.Age == originalPerson.Age, "Age должен совпадать");
            Assert(deserializedPerson.Id == originalPerson.Id, "Id должен совпадать");
            Assert(deserializedPerson.Email == originalPerson.Email, "Email должен совпадать");
            Assert(deserializedPerson.PhoneNumber == originalPerson.PhoneNumber, "PhoneNumber должен совпадать");
            Assert(deserializedPerson.BirthDate == originalPerson.BirthDate, "BirthDate должен совпадать");

            Console.WriteLine("Тест пройден\n");
        }

        public void TestSaveToFile()
        {
            Console.WriteLine("Тест 3: Сохранение в файл (синхронно)");
            
            var person = CreateTestPerson();
            string filePath = Path.Combine(_testDirectory, "test_save.json");

            try
            {
                _serializer.SaveToFile(person, filePath);
                Assert(File.Exists(filePath), "Файл должен быть создан");

                string fileContent = File.ReadAllText(filePath);
                Assert(fileContent.Contains("Иван"), "Файл должен содержать данные Person");

                Console.WriteLine("Тест пройден\n");
            }
            finally
            {
                CleanupTestFiles();
            }
        }

        public void TestLoadFromFile()
        {
            Console.WriteLine("Тест 4: Загрузка из файла (синхронно)");
            
            var originalPerson = CreateTestPerson();
            string filePath = Path.Combine(_testDirectory, "test_load.json");

            try
            {
                _serializer.SaveToFile(originalPerson, filePath);
                Person loadedPerson = _serializer.LoadFromFile(filePath);

                Assert(loadedPerson.FirstName == originalPerson.FirstName, "FirstName должен совпадать");
                Assert(loadedPerson.LastName == originalPerson.LastName, "LastName должен совпадать");
                Assert(loadedPerson.Age == originalPerson.Age, "Age должен совпадать");

                Console.WriteLine("Тест пройден\n");
            }
            finally
            {
                CleanupTestFiles();
            }
        }

        public void TestLoadFromFileNotFound()
        {
            Console.WriteLine("Тест 5: Загрузка из несуществующего файла");
            
            string filePath = Path.Combine(_testDirectory, "nonexistent.json");

            try
            {
                _serializer.LoadFromFile(filePath);
                Assert(false, "Должно быть выброшено исключение FileNotFoundException");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Тест пройден (исключение выброшено корректно)\n");
            }
            catch (Exception ex)
            {
                Assert(false, $"Выброшено неправильное исключение: {ex.GetType().Name}");
            }
        }

        public async Task TestSaveToFileAsync()
        {
            Console.WriteLine("Тест 6: Сохранение в файл (асинхронно)");
            
            var person = CreateTestPerson();
            string filePath = Path.Combine(_testDirectory, "test_save_async.json");

            try
            {
                await _serializer.SaveToFileAsync(person, filePath);
                Assert(File.Exists(filePath), "Файл должен быть создан");

                string fileContent = await File.ReadAllTextAsync(filePath);
                Assert(fileContent.Contains("Иван"), "Файл должен содержать данные Person");

                Console.WriteLine("Тест пройден\n");
            }
            finally
            {
                CleanupTestFiles();
            }
        }

        public async Task TestLoadFromFileAsync()
        {
            Console.WriteLine("Тест 7: Загрузка из файла (асинхронно)");
            
            var originalPerson = CreateTestPerson();
            string filePath = Path.Combine(_testDirectory, "test_load_async.json");

            try
            {
                await _serializer.SaveToFileAsync(originalPerson, filePath);
                Person loadedPerson = await _serializer.LoadFromFileAsync(filePath);

                Assert(loadedPerson.FirstName == originalPerson.FirstName, "FirstName должен совпадать");
                Assert(loadedPerson.Age == originalPerson.Age, "Age должен совпадать");

                Console.WriteLine("Тест пройден\n");
            }
            finally
            {
                CleanupTestFiles();
            }
        }

        public void TestSaveListToFile()
        {
            Console.WriteLine("Тест 8: Сохранение списка в файл");
            
            var people = new List<Person>
            {
                CreateTestPerson(),
                new Person
                {
                    FirstName = "Мария",
                    LastName = "Петрова",
                    Age = 30,
                    Password = "pass456",
                    Id = "67890",
                    BirthDate = new DateTime(1994, 5, 20),
                    Email = "maria@example.com",
                    PhoneNumber = "+7-999-987-65-43"
                }
            };

            string filePath = Path.Combine(_testDirectory, "test_list.json");

            try
            {
                _serializer.SaveListToFile(people, filePath);
                Assert(File.Exists(filePath), "Файл должен быть создан");

                string fileContent = File.ReadAllText(filePath);
                Assert(fileContent.Contains("Иван"), "Файл должен содержать первого Person");
                Assert(fileContent.Contains("Мария"), "Файл должен содержать второго Person");

                Console.WriteLine("Тест пройден\n");
            }
            finally
            {
                CleanupTestFiles();
            }
        }

        public void TestLoadListFromFile()
        {
            Console.WriteLine("Тест 9: Загрузка списка из файла");
            
            var originalPeople = new List<Person>
            {
                CreateTestPerson(),
                new Person
                {
                    FirstName = "Алексей",
                    LastName = "Сидоров",
                    Age = 22,
                    Password = "pass789",
                    Id = "11111",
                    BirthDate = new DateTime(2002, 3, 10),
                    Email = "alex@example.com",
                    PhoneNumber = "+7-999-111-22-33"
                }
            };

            string filePath = Path.Combine(_testDirectory, "test_list_load.json");

            try
            {
                _serializer.SaveListToFile(originalPeople, filePath);
                List<Person> loadedPeople = _serializer.LoadListFromFile(filePath);

                Assert(loadedPeople.Count == 2, "Должно быть загружено 2 Person");
                Assert(loadedPeople[0].FirstName == originalPeople[0].FirstName, "Первый Person должен совпадать");
                Assert(loadedPeople[1].FirstName == originalPeople[1].FirstName, "Второй Person должен совпадать");

                Console.WriteLine("Тест пройден\n");
            }
            finally
            {
                CleanupTestFiles();
            }
        }

        public void TestPasswordJsonIgnore()
        {
            Console.WriteLine("Тест 10: Проверка JsonIgnore для Password");
            
            var person = CreateTestPerson();
            person.Password = "super_secret_password";
            string json = _serializer.SerializeToJson(person);

            Assert(!json.Contains("Password"), "JSON не должен содержать поле Password");
            Assert(!json.Contains("super_secret_password"), "JSON не должен содержать значение Password");

            Person deserialized = _serializer.DeserializeFromJson(json);
            Assert(deserialized.Password == string.Empty, "Password должен быть пустым после десериализации");

            Console.WriteLine("Тест пройден\n");
        }

        public void TestJsonPropertyName()
        {
            Console.WriteLine("Тест 11: Проверка JsonPropertyName");
            
            var person = CreateTestPerson();
            person.Id = "test-id-123";
            person.PhoneNumber = "+7-999-999-99-99";
            string json = _serializer.SerializeToJson(person);

            Assert(json.Contains("personId"), "JSON должен содержать 'personId'");
            Assert(json.Contains("phone"), "JSON должен содержать 'phone'");
            Assert(!json.Contains("\"Id\""), "JSON не должен содержать 'Id'");
            Assert(!json.Contains("\"PhoneNumber\""), "JSON не должен содержать 'PhoneNumber'");

            Person deserialized = _serializer.DeserializeFromJson(json);
            Assert(deserialized.Id == "test-id-123", "Id должен корректно десериализоваться");
            Assert(deserialized.PhoneNumber == "+7-999-999-99-99", "PhoneNumber должен корректно десериализоваться");

            Console.WriteLine("Тест пройден\n");
        }

        private void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception($"Тест не пройден: {message}");
            }
        }

        public static async Task RunAllTests()
        {
            Console.WriteLine("=== Запуск тестов PersonSerializer ===\n");
            
            var tests = new PersonSerializerTests();
            
            try
            {
                tests.TestSerializeToJson();
                tests.TestDeserializeFromJson();
                tests.TestSaveToFile();
                tests.TestLoadFromFile();
                tests.TestLoadFromFileNotFound();
                await tests.TestSaveToFileAsync();
                await tests.TestLoadFromFileAsync();
                tests.TestSaveListToFile();
                tests.TestLoadListFromFile();
                tests.TestPasswordJsonIgnore();
                tests.TestJsonPropertyName();

                Console.WriteLine("=== Все тесты успешно пройдены! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
            }
        }
    }
}

