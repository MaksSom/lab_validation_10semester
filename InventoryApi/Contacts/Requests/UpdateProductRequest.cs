namespace InventoryApi.Contracts.Requests;

public class UpdateProductRequest
{
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}