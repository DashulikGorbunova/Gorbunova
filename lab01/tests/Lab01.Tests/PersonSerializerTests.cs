using System.IO;
using Lab01;
using Xunit;

namespace Lab01.Tests;

public class PersonSerializerTests
{
    [Fact]
    public void SerializeAndDeserialize_Person_Works()
    {
        var serializer = new PersonSerializer();

        var p = new Person
        {
            FirstName = "Bob",
            LastName = "Bobson",
            Age = 21,
            Email = "bob.bobson@gmail.com",
            Password = "secret",
            Id = "123",
            PhoneNumber = "+7 966 456 33 32",
            BirthDate = new System.DateTime(2003, 1, 1)
        };

        var json = serializer.SerializeToJson(p);
        var restored = serializer.DeserializeFromJson(json);

        Assert.Equal(p.FirstName, restored.FirstName);
        Assert.Equal(p.LastName, restored.LastName);
        Assert.Equal(p.Age, restored.Age);
        Assert.Equal(p.Email, restored.Email);
        Assert.Equal(p.Id, restored.Id);
        Assert.Equal(p.PhoneNumber, restored.PhoneNumber);
        Assert.Equal(p.BirthDate.Date, restored.BirthDate.Date);
        Assert.Equal(string.Empty, restored.Password);
    }

    [Fact]
    public void SaveAndLoad_File_Works()
    {
        var serializer = new PersonSerializer();
        var tempFile = Path.GetTempFileName();

        try
        {
            var p = new Person
            {
                FirstName = "Artak",
                LastName = "Gevorgyan",
                Age = 30,
                Email = "artak.gevorgyan@gmail.com"
            };

            serializer.SaveToFile(p, tempFile);

            var loaded = serializer.LoadFromFile(tempFile);

            Assert.Equal(p.FirstName, loaded.FirstName);
            Assert.Equal(p.LastName, loaded.LastName);
            Assert.Equal(p.Age, loaded.Age);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}


