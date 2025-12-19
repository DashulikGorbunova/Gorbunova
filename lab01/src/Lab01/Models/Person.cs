using System.Text.Json.Serialization;

namespace Lab01;
public class Person
{
   
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public int Age
    {
        get => _age;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Age не может быть отрицательным");
            }

            _age = value;
        }
    }

    
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

   
    [JsonPropertyName("personId")]
    public string Id { get; set; } = string.Empty;


    [JsonInclude]
    private DateTime _birthDate;


    [JsonIgnore]
    public DateTime BirthDate
    {
        get => _birthDate;
        set => _birthDate = value;
    }

    private string _email = string.Empty;

    public string Email
    {
        get => _email;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            {
                throw new ArgumentException("Некорректный email", nameof(value));
            }

            _email = value;
        }
    }


    [JsonPropertyName("phone")]
    public string PhoneNumber { get; set; } = string.Empty;


    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}".Trim();

    [JsonIgnore]
    public bool IsAdult => Age >= 18;

    private int _age;
}


