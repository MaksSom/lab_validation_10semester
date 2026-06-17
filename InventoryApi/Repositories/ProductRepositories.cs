using Microsoft.Extensions.Options;
using MongoDB.Driver;
using InventoryApi.Models;
using InventoryApi.Settings;

namespace InventoryApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductRepository(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _productsCollection = mongoDatabase.GetCollection<Product>(mongoDbSettings.Value.ProductsCollectionName);
    }

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _productsCollection.Find(x => !x.IsDeleted).ToListAsync();

    public async Task<Product?> GetByIdAsync(string id) =>
        await _productsCollection.Find(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();

    public async Task CreateAsync(Product product) =>
        await _productsCollection.InsertOneAsync(product);

    public async Task UpdateAsync(string id, Product product)
    {
        var update = Builders<Product>.Update
            .Set(x => x.Name, product.Name)
            .Set(x => x.Price, product.Price)
            .Set(x => x.Quantity, product.Quantity); 

        await _productsCollection.UpdateOneAsync(x => x.Id == id, update);
    }

    public async Task<bool> IncrementQuantityAsync(string id, int amount)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        var update = Builders<Product>.Update.Inc(p => p.Quantity, amount);

        var result = await _productsCollection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> TryDecrementQuantityAsync(string id, int amount)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(x => x.Id, id),
            Builders<Product>.Filter.Eq(x => x.IsDeleted, false),
            Builders<Product>.Filter.Gte(x => x.Quantity, amount)
        );

        var update = Builders<Product>.Update.Inc(x => x.Quantity, -amount);

        var result = await _productsCollection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<Product>.Update.Set(x => x.IsDeleted, true);
        await _productsCollection.UpdateOneAsync(x => x.Id == id, update);
    }
}