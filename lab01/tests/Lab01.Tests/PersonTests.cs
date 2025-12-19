using System;
using Lab01;
using Xunit;

namespace Lab01.Tests;

public class PersonTests
{
    [Fact]
    public void FullName_ReturnsConcatenation()
    {
        var p = new Person
        {
            FirstName = "Bob",
            LastName = "Bobson",
            Age = 21,
            Email = "bob.bobson@gmail.com"
        };

        Assert.Equal("Bob Bobson", p.FullName);
    }

    [Theory]
    [InlineData(17, false)]
    [InlineData(18, true)]
    [InlineData(25, true)]
    public void IsAdult_WorksCorrectly(int age, bool expected)
    {
        var p = new Person
        {
            FirstName = "Artak",
            LastName = "Gevorgyan",
            Age = age,
            Email = "artak.gevorgyan@gmail.com"
        };

        Assert.Equal(expected, p.IsAdult);
    }

    [Fact]
    public void Email_Invalid_Throws()
    {
        var p = new Person();

        Assert.Throws<ArgumentException>(() => p.Email = "wrong-email");
    }
}


