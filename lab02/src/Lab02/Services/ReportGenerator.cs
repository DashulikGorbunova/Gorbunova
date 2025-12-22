using Lab02.Models;

namespace Lab02.Services;

public class ReportGenerator
{
    public void PrintReport(BenchmarkResult results)
    {
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("Отчет о производительности коллекций");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        PrintCollectionResults(results.ListResults);
        PrintCollectionResults(results.LinkedListResults);
        PrintCollectionResults(results.QueueResults);
        PrintCollectionResults(results.StackResults);
        PrintCollectionResults(results.ImmutableListResults);

        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("Анализ и выводы");
        Console.WriteLine("=".PadRight(80, '='));
        PrintAnalysis(results);
    }

    private void PrintCollectionResults(CollectionOperationResults results)
    {
        Console.WriteLine($"--- {results.CollectionName} ---");
        Console.WriteLine($"Добавление в конец:        {FormatTime(results.AddToEnd)}");
        if (results.AddToStart >= 0)
            Console.WriteLine($"Добавление в начало:       {FormatTime(results.AddToStart)}");
        if (results.AddToMiddle >= 0)
            Console.WriteLine($"Добавление в середину:    {FormatTime(results.AddToMiddle)}");
        if (results.RemoveFromStart >= 0)
            Console.WriteLine($"Удаление из начала:       {FormatTime(results.RemoveFromStart)}");
        if (results.RemoveFromEnd >= 0)
            Console.WriteLine($"Удаление из конца:        {FormatTime(results.RemoveFromEnd)}");
        if (results.RemoveFromMiddle >= 0)
            Console.WriteLine($"Удаление из середины:    {FormatTime(results.RemoveFromMiddle)}");
        Console.WriteLine($"Поиск элемента:           {FormatTime(results.Search)}");
        if (results.GetByIndex >= 0)
            Console.WriteLine($"Получение по индексу:     {FormatTime(results.GetByIndex)}");
        Console.WriteLine();
    }

    private string FormatTime(double milliseconds)
    {
        if (milliseconds < 0)
            return "N/A";
        return $"{milliseconds:F2} мс";
    }

    private void PrintAnalysis(BenchmarkResult results)
    {
        Console.WriteLine("1. List<T>:");
        Console.WriteLine("   - Быстрое добавление в конец (O(1) амортизированное)");
        Console.WriteLine("   - Медленное добавление/удаление в начало/середину (O(n))");
        Console.WriteLine("   - Быстрый доступ по индексу (O(1))");
        Console.WriteLine("   - Поиск O(n)");
        Console.WriteLine();

        Console.WriteLine("2. LinkedList<T>:");
        Console.WriteLine("   - Быстрое добавление/удаление в начало/конец (O(1))");
        Console.WriteLine("   - Медленное добавление/удаление в середину (O(n) для поиска позиции)");
        Console.WriteLine("   - Нет доступа по индексу");
        Console.WriteLine("   - Поиск O(n)");
        Console.WriteLine();

        Console.WriteLine("3. Queue<T>:");
        Console.WriteLine("   - Оптимизирован для FIFO операций");
        Console.WriteLine("   - Быстрое добавление в конец и удаление из начала (O(1))");
        Console.WriteLine("   - Не поддерживает операции в середине");
        Console.WriteLine();

        Console.WriteLine("4. Stack<T>:");
        Console.WriteLine("   - Оптимизирован для LIFO операций");
        Console.WriteLine("   - Быстрое добавление и удаление из конца (O(1))");
        Console.WriteLine("   - Не поддерживает операции в середине");
        Console.WriteLine();

        Console.WriteLine("5. ImmutableList<T>:");
        Console.WriteLine("   - Неизменяемая коллекция, создает новую при изменении");
        Console.WriteLine("   - Медленнее всех операций из-за создания копий");
        Console.WriteLine("   - Потокобезопасна по умолчанию");
        Console.WriteLine("   - Подходит для многопоточных сценариев");
        Console.WriteLine();
    }
}
