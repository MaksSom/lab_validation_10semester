using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using InventoryApi.Contracts.Requests;
using InventoryApi.Models;
using FluentAssertions;
using Xunit;

namespace InventoryApi.Tests;

public class ProductLifecycleTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductLifecycleTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Product_ShouldGoThroughFullLifecycle()
    {
        // КРОК 1: Створення
        var newProduct = new Product { Name = "Gaming Laptop", Quantity = 10, Price = 1500m };
        var createResponse = await _client.PostAsJsonAsync("/api/inventory", newProduct);
        
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<Product>();
        var id = createdProduct!.Id;
        id.Should().NotBeNullOrEmpty();

        // КРОК 2: Оновлення ціни
        var updateRequest = new UpdateProductRequest { Name = "Gaming Laptop Pro", Quantity = 8, Price = 1700m };
        var updateResponse = await _client.PutAsJsonAsync($"/api/inventory/{id}", updateRequest);
        
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // КРОК 3: Замовлення
        var orderRequest = new CreateOrderRequest { ProductId = id!, Amount = 2 };
        var orderResponse = await _client.PostAsJsonAsync("/api/inventory/order", orderRequest);
        
        orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Перевіряємо, чи стан в базі змінився
        var getResponse = await _client.GetAsync($"/api/inventory/{id}");
        var productAfterOrder = await getResponse.Content.ReadFromJsonAsync<Product>();
        productAfterOrder!.Quantity.Should().Be(6);

        // КРОК 4: Видалення
        var deleteResponse = await _client.DeleteAsync($"/api/inventory/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // КРОК 5: Перевірка видалення
        var finalGetResponse = await _client.GetAsync($"/api/inventory/{id}");
        finalGetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}