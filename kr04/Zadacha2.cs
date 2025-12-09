using System.Reflection;

class Son { public short Value; }
class Lon { public long Value; }

class Program
{
    static void Main()
    {
        Console.WriteLine("Объекты");
        
        object[] allobject = new object[6];
        
        for (int i = 0; i < 3; i++)
        {
            Son s = new Son();
            s.Value = (short)(i * 100);
            allobject[i] = s;
        }
        
        for (int i = 0; i < 3; i++)
        {
            Lon l = new Lon();
            l.Value = i * 100000L;
            allobject[i + 3] = l;
        }
        Console.WriteLine("Все созданные объекты:");
        for (int i = 0; i < allobject.Length; i++)
        {
            if (allobject[i] is Son)
                Console.WriteLine($"Объект {i+1}: Son = {((Son)allobject[i]).Value}");
            else
                Console.WriteLine($"Объект {i+1}: Lon = {((Lon)allobject[i]).Value}");
        }
    }
}