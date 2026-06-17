using Microsoft.AspNetCore.Mvc;
using InventoryApi.Models;
using InventoryApi.Services;
using InventoryApi.Contracts.Requests;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get() =>
        Ok(await _inventoryService.GetAllProductsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var product = await _inventoryService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Product product)
    {
        await _inventoryService.CreateProductAsync(product);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPost("order")]
    public async Task<IActionResult> Order([FromBody] CreateOrderRequest request)
    {
        var success = await _inventoryService.OrderProductAsync(request.ProductId, request.Amount);
        return success ? Ok() : BadRequest("Order failed");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateProductRequest request)
    {
        var updated = await _inventoryService.UpdateProductAsync(
            id, 
            request.Name, 
            request.Quantity, 
            request.Price
        );
        
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var product = await _inventoryService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound($"Товар з ID {id} не знайдено.");
        }

        await _inventoryService.DeleteProductAsync(id);
        
        return NoContent(); 
    }
}
