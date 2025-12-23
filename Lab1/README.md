# Лабораторная работа 1

Проект демонстрирует работу с сериализацией объектов в JSON, управление файловыми ресурсами и реализацию паттерна IDisposable в C#.

## Классы проекта

### Person

Модель данных, представляющая информацию о человеке. Использует различные JSON-атрибуты для управления сериализацией.

Свойства:
- FirstName, LastName, Age - основные поля
- Password - игнорируется при сериализации через JsonIgnore
- Id - сериализуется как "personId" через JsonPropertyName
- BirthDate - дата рождения, доступ через свойство
- Email - с валидацией наличия символа '@'
- PhoneNumber - сериализуется как "phone"

Вычисляемые свойства:
- FullName - полное имя (FirstName + LastName)
- IsAdult - проверка совершеннолетия (Age >= 18)

### PersonSerializer

Класс для работы с сериализацией объектов Person в JSON.

Методы:
- SerializeToJson - сериализация в JSON-строку
- DeserializeFromJson - десериализация из JSON-строки
- SaveToFile - синхронное сохранение в файл
- LoadFromFile - синхронная загрузка из файла
- SaveToFileAsync - асинхронное сохранение в файл
- LoadFromFileAsync - асинхронная загрузка из файла
- SaveListToFile - сохранение списка объектов
- LoadListFromFile - загрузка списка объектов

Использует System.Text.Json для сериализации, настроен для корректной работы с кириллицей.

### FileResourceManager

Класс для работы с файлами с реализацией IDisposable и управлением ресурсами.

Методы:
- OpenForWriting - открывает файл для записи
- OpenForReading - открывает файл для чтения
- WriteLine - записывает строку в файл
- ReadAllText - читает весь файл
- AppendText - добавляет текст в конец файла
- GetFileInfo - возвращает информацию о файле
- Dispose - освобождает ресурсы

Реализует интерфейс IDisposable, правильно управляет ресурсами.

### PersonSerializerTests

Класс содержит набор тестов для проверки функциональности PersonSerializer.

Тесты проверяют сериализацию, десериализацию, работу с файлами (синхронно и асинхронно), работу со списками, JsonIgnore для Password, JsonPropertyName для Id и PhoneNumber.

## Запуск проекта

Требуется .NET 9.0 SDK или выше.

Команды для работы с проектом:
```
dotnet restore
dotnet build
dotnet run
```

## Пример JSON-вывода

При сериализации объекта Person получается следующий JSON:

```json
{
  "FirstName": "Artak",
  "LastName": "Oganisyan",
  "Age": 18,
  "personId": "12345secret",
  "_birthDate": "2007-12-16:T00:00:00",
  "Email": "artak@example.com",
  "phone": "+7-999-123-45-67"
}
```
