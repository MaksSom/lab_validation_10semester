using Moq;
using FluentAssertions;
using InventoryApi.Models;
using InventoryApi.Repositories;
using InventoryApi.Services;
using Xunit;

namespace InventoryApi.Tests;

public class InventoryServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly InventoryService _sut; // System Under Test

    public InventoryServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _sut = new InventoryService(_productRepoMock.Object);
    }

    [Fact]
    public async Task OrderProduct_ShouldReturnTrue_WhenRepositorySucceeds()
    {
        var productId = "prod_123";
        var amount = 5;

        _productRepoMock.Setup(repo => repo.TryDecrementQuantityAsync(productId, amount))
            .ReturnsAsync(true);

        var result = await _sut.OrderProductAsync(productId, amount);

        result.Should().BeTrue();

        // Перевіряємо, що сервіс викликав саме атомарний метод репозиторію
        _productRepoMock.Verify(repo => repo.TryDecrementQuantityAsync(productId, amount), Times.Once);
        
        // Перевіряємо, що старий небезпечний метод UpdateAsync НЕ викликався
        _productRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task RestockProduct_ShouldIncreaseQuantity_WhenProductExists()
    {
        var productId = "restock_id";
        var addedAmount = 50;

        // Налаштовуємо Mock на успішне збільшення кількості
        _productRepoMock.Setup(repo => repo.IncrementQuantityAsync(productId, addedAmount))
            .ReturnsAsync(true);

        // Викликаємо метод, якого ще не існує в сервісі
        var result = await _sut.RestockProductAsync(productId, addedAmount);

        result.Should().BeTrue();
        _productRepoMock.Verify(repo => repo.IncrementQuantityAsync(productId, addedAmount), Times.Once);
    }

    [Theory]
    [InlineData("id_1", 10, true)]
    [InlineData("id_2", 0, false)]
    [InlineData("id_3", -5, false)]
    public async Task OrderProduct_ShouldHandleVariousAmounts(string id, int amount, bool expectedResult)
    {
        _productRepoMock.Setup(repo => repo.TryDecrementQuantityAsync(id, amount))
            .ReturnsAsync(expectedResult);

        var result = await _sut.OrderProductAsync(id, amount);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task OrderProduct_ShouldReturnFalse_WhenRepositoryFails()
    {
        var productId = "69f841b934698cb9941ce984";
        _productRepoMock.Setup(repo => repo.TryDecrementQuantityAsync(productId, It.IsAny<int>()))
            .ReturnsAsync(false);

        var result = await _sut.OrderProductAsync(productId, 100);

        result.Should().BeFalse();
    }
}