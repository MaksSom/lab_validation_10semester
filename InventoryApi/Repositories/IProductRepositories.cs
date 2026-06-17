using InventoryApi.Models;

namespace InventoryApi.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(string id);
    Task CreateAsync(Product product);
    Task UpdateAsync(string id, Product product);
    Task<bool> TryDecrementQuantityAsync(string id, int amount);
    Task DeleteAsync(string id);
    Task<bool> IncrementQuantityAsync(string id, int amount);
}