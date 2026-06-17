using InventoryApi.Repositories;
using InventoryApi.Services;

namespace InventoryApi.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Реєструємо репозиторій. Scoped - створюється один на кожен HTTP-запит
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryService, InventoryService>();
        return services;
    }
}