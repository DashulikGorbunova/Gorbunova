namespace Lab02.Models;

public class CollectionOperationResults
{
    public string CollectionName { get; set; } = string.Empty;
    public double AddToEnd { get; set; }
    public double AddToStart { get; set; }
    public double AddToMiddle { get; set; }
    public double RemoveFromStart { get; set; }
    public double RemoveFromEnd { get; set; }
    public double RemoveFromMiddle { get; set; }
    public double Search { get; set; }
    public double GetByIndex { get; set; }
}
