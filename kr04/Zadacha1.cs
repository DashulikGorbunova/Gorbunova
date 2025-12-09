
using System.Reflection;

class Student
{
    private string name = "Боб";
    private int age = 18;
}

class Program
{
    static void Main()
    {
        Student student = new Student();
        
        Type type = student.GetType();
        
        Console.WriteLine("Первоночальное значение");
        
        FieldInfo[] fields = type.GetFields(
            BindingFlags.NonPublic | 
            BindingFlags.Instance);  
        
        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(student);
            Console.WriteLine($"{field.Name} = {value}");
        }
        
        Console.WriteLine("\nИзменение значений");
        
        foreach (FieldInfo field in fields)
        {
            if (field.Name == "name")
            {
                field.SetValue(student, "Дениска");
                Console.WriteLine("Имя изменено на: Дениска");
            }
            else if (field.Name == "age")
            {
                field.SetValue(student, 21);
                Console.WriteLine("Возраст изменен на: 21");
            }
        }
        
        Console.WriteLine("\nОбновленные значения");
        
        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(student);
            Console.WriteLine($"{field.Name} = {value}");
        }
    }
}