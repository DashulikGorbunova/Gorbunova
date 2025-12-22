namespace Lab02.Models;

public class BenchmarkResult
{
    public CollectionOperationResults ListResults { get; set; } = new();
    public CollectionOperationResults LinkedListResults { get; set; } = new();
    public CollectionOperationResults QueueResults { get; set; } = new();
    public CollectionOperationResults StackResults { get; set; } = new();
    public CollectionOperationResults ImmutableListResults { get; set; } = new();
}
