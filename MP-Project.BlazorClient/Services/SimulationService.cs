using System.Text.Json;
using MP_Project.Shared;

public class SimulationService
{
	private readonly IHttpClientFactory _clientFactory;
	private CancellationTokenSource? _cts;
	private Task? _simulationTask;

	public bool IsRunning => _simulationTask != null && !_simulationTask.IsCompleted;

	public SimulationService(IHttpClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
	}

	public async Task StartSimulation()
	{
		if (IsRunning)
			return;

		_cts = new CancellationTokenSource();
		var token = _cts.Token;

		_simulationTask = Task.Run(async () =>
		{
			var http = _clientFactory.CreateClient("API");

			var personas = await http.GetFromJsonAsync<List<Persona>>("api/personas")
						   ?? new List<Persona>();

			var random = new Random();

			while (!token.IsCancellationRequested)
			{
				var persona = personas[random.Next(personas.Count)];

				foreach (var item in persona.ShoppingList)
				{
					await http.PutAsync(
						$"api/products/{item.ProductId}/decrease-stock?quantity={item.Quantity}",
						null
					);
				}

				await Task.Delay(2000, token); // simulate time between customers
			}

		}, token);
	}

	public void StopSimulation()
	{
		_cts?.Cancel();
	}
}