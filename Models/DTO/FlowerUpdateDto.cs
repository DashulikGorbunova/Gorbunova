namespace WebApplication1.Models.DTO;

public class FlowerUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Color { get; set; }
    public string? Season { get; set; }
    public string? ImageUrl { get; set; }
    public int? CategoryId { get; set; }
    public bool IsAvailable { get; set; }
}

