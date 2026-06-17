using InventoryApi.Models;
using InventoryApi.Repositories;

namespace InventoryApi.Services;

public class InventoryService : IInventoryService
{
    private readonly IProductRepository _productRepository;

    public InventoryService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync() =>
        await _productRepository.GetAllAsync();

    public async Task<Product?> GetProductByIdAsync(string id) =>
        await _productRepository.GetByIdAsync(id);

    public async Task CreateProductAsync(Product product) =>
        await _productRepository.CreateAsync(product);

    public async Task<bool> OrderProductAsync(string productId, int amount)
    {
        if (amount <= 0) return false;

        return await _productRepository.TryDecrementQuantityAsync(productId, amount);
    }

    public async Task<bool> UpdateProductAsync(string id, string name, int quantity, decimal price)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return false;

        product.Name = name;
        product.Quantity = quantity;
        product.Price = price;

        await _productRepository.UpdateAsync(id, product);
        return true;
    }

    public async Task DeleteProductAsync(string id) 
    {
        await _productRepository.DeleteAsync(id);
    }
}