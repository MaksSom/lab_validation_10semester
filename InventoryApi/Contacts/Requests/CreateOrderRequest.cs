namespace InventoryApi.Contracts.Requests;
public class CreateOrderRequest
{
public string ProductId {get; set;} = default!;
public int Amount {get; set;}
}