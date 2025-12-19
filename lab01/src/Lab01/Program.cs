using Lab01;

var person = new Person
{
    FirstName = "Bob",
    LastName = "Bobson",
    Age = 21,
    Email = "bob.bobson@gmail.com",
    Password = "Bob0809",
    PhoneNumber = "+7 966 456 33 32",
    Id = Guid.NewGuid().ToString(),
    BirthDate = new DateTime(2000, 1, 1)
};

Console.WriteLine($"Full name: {person.FullName}");
Console.WriteLine($"Is adult: {person.IsAdult}");

var serializer = new PersonSerializer();
var json = serializer.SerializeToJson(person);

Console.WriteLine("JSON:");
Console.WriteLine(json);


