using System.IO;
using Lab01;
using Xunit;

namespace Lab01.Tests;

public class FileResourceManagerTests
{
    [Fact]
    public void WriteLine_And_ReadAllText_Work()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            using (var manager = new FileResourceManager(tempFile, FileMode.Create))
            {
                manager.OpenForWriting();
                manager.WriteLine("Hello");
            }

            using (var manager = new FileResourceManager(tempFile, FileMode.Open))
            {
                manager.OpenForReading();
                var text = manager.ReadAllText();
                Assert.Contains("Hello", text);
            }
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void AppendText_AppendsToFile()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            File.WriteAllText(tempFile, "start");

            using (var manager = new FileResourceManager(tempFile, FileMode.Append))
            {
                manager.AppendText(" + more");
            }

            var content = File.ReadAllText(tempFile);
            Assert.Contains("start", content);
            Assert.Contains("more", content);
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


