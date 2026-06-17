using InventoryApi.Models;

namespace InventoryApi.Services;

public interface IInventoryService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(string id);
    Task CreateProductAsync(Product product);
    Task<bool> OrderProductAsync(string productId, int amount);
    Task<bool> UpdateProductAsync(string id, string name, int quantity, decimal price);
    Task DeleteProductAsync(string id);
}