using System.Text;
using System.Net.Http.Json; 
using NBomber.CSharp;

using var httpClient = new HttpClient();

var scenario = Scenario.Create("load_test", async context =>
{
    try
    {
        var payload = new { productId = "69f83381ff28f51c34337af4", amount = 1 };
        
        var response = await httpClient.PostAsJsonAsync("http://localhost:5020/api/inventory/order", payload);

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            return Response.Ok(statusCode: "400"); 
        }

        return response.IsSuccessStatusCode 
        ? Response.Ok() 
        : Response.Fail(message: $"Real Error: {response.StatusCode}");
        }
    catch (Exception ex)
    {
        // Передаємо текст помилки як string
        return Response.Fail(message: ex.Message);
    }
})
.WithWarmUpDuration(TimeSpan.FromSeconds(5))
.WithLoadSimulations(
    Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
);

// 3. Запуск
NBomberRunner
    .RegisterScenarios(scenario)
    .Run();