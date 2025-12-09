using System;
using System.Reflection;

// Класс с закрытыми полями
class Student
{
    private string name = "Боб";
    private int age = 18;
}

class Program
{
    static void Main()
    {
        // Создаем объект Student
        Student student = new Student();
        
        // Получаем тип объекта
        Type type = student.GetType();
        
        Console.WriteLine("Первоночальное значение");
        
        // Получаем все закрытые поля
        FieldInfo[] fields = type.GetFields(
            BindingFlags.NonPublic | // непубличные поля
            BindingFlags.Instance);   // поля экземпляра (не статические)
        
        // Выводим значения закрытых полей
        foreach (FieldInfo field in fields)
        {
            // Получаем значение поля
            object value = field.GetValue(student);
            Console.WriteLine($"{field.Name} = {value}");
        }
        
        Console.WriteLine("\nИзменение значений");
        
        // Меняем значения полей
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
        
        // Выводим обновленные значения
        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(student);
            Console.WriteLine($"{field.Name} = {value}");
        }
    }
}