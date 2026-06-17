using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using InventoryApi.Contracts.Requests;
using FluentAssertions;
using Xunit;

namespace InventoryApi.Tests;

public class InventoryWebTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public InventoryWebTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // Приклад 1: Перевірка успішного отримання всього списку (200 OK)
    [Fact]
    public async Task GetProducts_ReturnsSuccessAndJson()
    {
        var response = await _client.GetAsync("/api/inventory");

        response.EnsureSuccessStatusCode(); 
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    // Приклад 2: Перевірка роботи валідації вхідних даних (400 Bad Request)
    [Fact]
    public async Task OrderProduct_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        var request = new CreateOrderRequest 
        { 
            ProductId = "65f1a2b3c4d5e6f7a8b9c0d1", 
            Amount = -10 // Невалідне значення
        };

        var response = await _client.PostAsJsonAsync("/api/inventory/order", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Приклад 3: Перевірка реакції на неіснуючий ресурс (404 Not Found)
    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenIdDoesNotExist()
    {
        var fakeId = "000000000000000000000000";
        var response = await _client.GetAsync($"/api/inventory/{fakeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}