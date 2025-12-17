using System.Text.Json.Serialization;

namespace Lab1
{
    public class Person
    {
        // Обычные свойства
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }

        // Password - не должно попадать в JSON
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        // Id - в JSON как "personId"
        [JsonPropertyName("personId")]
        public string Id { get; set; } = string.Empty;

        // _birthDate - private поле с JsonInclude, доступ через свойство BirthDate
        [JsonInclude]
        private DateTime _birthDate;

        public DateTime BirthDate
        {
            get => _birthDate;
            set => _birthDate = value;
        }

        // Email - с валидацией в setter
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (!value.Contains('@'))
                {
                    throw new ArgumentException("Email должен содержать символ '@'");
                }
                _email = value;
            }
        }

        // PhoneNumber - в JSON как "phone"
        [JsonPropertyName("phone")]
        public string PhoneNumber { get; set; } = string.Empty;

        // FullName - только для чтения (конкатенация FirstName + LastName)
        public string FullName => $"{FirstName} {LastName}";

        // IsAdult - только для чтения (Age >= 18)
        public bool IsAdult => Age >= 18;
    }
}
