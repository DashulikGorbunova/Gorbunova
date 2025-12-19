using System.Text;
using System.Text.Json;

namespace Lab01;
public class PersonSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string SerializeToJson(Person person)
    {
        try
        {
            return JsonSerializer.Serialize(person, _options);
        }
        catch (Exception ex)
        {
            Logger.LogError("Ошибка при сериализации Person в JSON", ex);
            throw;
        }
    }

    public Person DeserializeFromJson(string json)
    {
        try
        {
            var person = JsonSerializer.Deserialize<Person>(json, _options);
            if (person == null)
            {
                throw new InvalidOperationException("Не удалось десериализовать Person из JSON");
            }

            return person;
        }
        catch (Exception ex)
        {
            Logger.LogError("Ошибка при десериализации Person из JSON", ex);
            throw;
        }
    }

    public void SaveToFile(Person person, string filePath)
    {
        try
        {
            var json = SerializeToJson(person);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при сохранении Person в файл '{filePath}'", ex);
            throw;
        }
    }

    public Person LoadFromFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден", filePath);
            }

            var json = File.ReadAllText(filePath, Encoding.UTF8);
            return DeserializeFromJson(json);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при загрузке Person из файла '{filePath}'", ex);
            throw;
        }
    }

    public async Task SaveToFileAsync(Person person, string filePath)
    {
        try
        {
            var json = SerializeToJson(person);
            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при асинхронном сохранении Person в файл '{filePath}'", ex);
            throw;
        }
    }

    public async Task<Person> LoadFromFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден", filePath);
            }

            var json = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            return DeserializeFromJson(json);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при асинхронной загрузке Person из файла '{filePath}'", ex);
            throw;
        }
    }

    public void SaveListToFile(List<Person> people, string filePath)
    {
        try
        {
            var json = JsonSerializer.Serialize(people, _options);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при сохранении списка Person в файл '{filePath}'", ex);
            throw;
        }
    }

    public List<Person> LoadListFromFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден", filePath);
            }

            var json = File.ReadAllText(filePath, Encoding.UTF8);
            var people = JsonSerializer.Deserialize<List<Person>>(json, _options);
            if (people == null)
            {
                throw new InvalidOperationException("Не удалось десериализовать список Person из JSON");
            }

            return people;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при загрузке списка Person из файла '{filePath}'", ex);
            throw;
        }
    }
}


