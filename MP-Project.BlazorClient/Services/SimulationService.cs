using System.IO;
using System.Text.Json;
using System.Transactions;
using Microsoft.AspNetCore.WebUtilities;
using MP_Project.Shared;
using static System.Net.WebRequestMethods;

public class SimulationService
{
	private readonly IHttpClientFactory ClientFactory;
	private readonly Random _rand = new();
	private CancellationTokenSource? _cts;

	public List<Sale> Sales { get; private set; } = new();
	public bool IsRunning { get; private set; }

	public SimulationService(IHttpClientFactory factory)
	{
		ClientFactory = factory;
	}

	public async Task RunSimulationAsync(
		int days,
		int customersPerDay,
		int secondsPerDay,
		List<Persona> personas,
		List<Product> products)
	{
		var simId = Guid.NewGuid();

		if (IsRunning)
		{
			Console.WriteLine("Simulation already running - restarting...");
			Stop();
		}

		_cts = new CancellationTokenSource();
		IsRunning = true;

		try
		{
			if (personas == null || personas.Count == 0)
			{
				return;
			}

			Console.WriteLine($"Personas inside sim: {personas.Count}");
			Console.WriteLine($"Products inside sim: {products?.Count}");

			var http = ClientFactory.CreateClient("API");
			var history = await http.GetFromJsonAsync<List<SimulationHistory>>(
				"api/simulation/history"
			);

			if (history != null && history.Any())
			{
				currentDay = history.Max(x => x.Day);
				currentBalance = history.OrderBy(x => x.Day).Last().Balance;

				Console.WriteLine($"RESUMING FROM DAY {currentDay} | Balance: {currentBalance}");
			}

			for (int day = 1; day <= days; day++)
			{
				decimal dailyRevenue = 0;
				decimal dailyCosts = 0;

				int delayPerCustomer = customersPerDay > 0
					? (secondsPerDay * 1000) / customersPerDay
					: 1000;

				for (int c = 0; c < customersPerDay; c++)
				{

					if (_cts.Token.IsCancellationRequested)
					{
						return;
					}
					var persona = personas[3];//[_rand.Next(personas.Count)];
					Console.WriteLine($"Persona: {persona.Name}");
					var sales = GenerateSales(persona, products);

					Sales.AddRange(sales);

					foreach (var sale in sales)
					{
						dailyRevenue += sale.TotalPrice;

						try
						{
							var url1 = $"api/products/{sale.ProductID}/decrease-stock?quantity={sale.Quantity}";

							Console.WriteLine($"CALLING: {url1}");

							var response = await http.PutAsync(url1, null);


							if (!response.IsSuccessStatusCode)
							{
								var error = await response.Content.ReadAsStringAsync();
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine($"POST ERROR: {ex.Message}");
						}
					}
					await Task.Delay(delayPerCustomer, _cts.Token);
				}

				var query = new Dictionary<string, string?>()
				{
					["day"] = day.ToString()
				};

				var url = QueryHelpers.AddQueryString(
					"api/suppliertransactions/delivery-cost-today",
					query
				);

				Console.WriteLine($"day value: {day}");
				Console.WriteLine($"Query = {query}");
				Console.WriteLine($"URL = {url}");

				var deliveryCost = await http.GetFromJsonAsync<decimal>(url);
				dailyCosts += deliveryCost;

				await RecordDayAsync(dailyRevenue, dailyCosts);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"SIM ERROR: {ex}");
		}
		finally
		{
			Console.WriteLine("SIMULATION FINISHED");
			IsRunning = false;
		}
	}

	public void Stop()
	{
		if (_cts != null && !_cts.IsCancellationRequested)
		{
			_cts.Cancel();
		}
		Console.WriteLine("SIMULATION STOPPED");
		IsRunning = false;
	}

	private List<Sale> GenerateSales(Persona persona, List<Product> products)
	{
		var sales = new List<Sale>();

		foreach (var item in persona.ShoppingList)
		{
			if (_rand.NextDouble() > 0.8)
				continue;

			var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
			if (product == null)
				continue;

			int quantity = Math.Max(1, item.Quantity + _rand.Next(-1, 2));

			if (_rand.NextDouble() < 0.1)
				quantity *= 2;

			sales.Add(new Sale
			{
				ProductID = product.ProductId,
				Quantity = quantity,
				SellingPrice = product.SellingPrice,
				TotalPrice = product.SellingPrice * quantity,
				SaleDate = DateTime.Now
			});
		}

		if (!sales.Any())
		{
			var fallback = persona.ShoppingList[_rand.Next(persona.ShoppingList.Count)];
			var product = products.FirstOrDefault(p => p.ProductId == fallback.ProductId);

			if (product != null)
			{
				sales.Add(new Sale
				{
					ProductID = product.ProductId,
					Quantity = 1,
					SellingPrice = product.SellingPrice,
					TotalPrice = product.SellingPrice,
					SaleDate = DateTime.Now
				});
			}
		}

		return sales;
	}

	private int currentDay = 0;
	private decimal currentBalance = 0;

	private async Task RecordDayAsync(decimal revenue, decimal costs)
	{
		currentDay++;
		Console.WriteLine($"RECORDING DAY {currentDay} | Balance: {currentBalance}");

		currentBalance += (revenue - costs);

		var record = new SimulationHistory
		{
			Day = currentDay,
			Balance = currentBalance,
			Revenue = revenue,
			Costs = costs,
			Timestamp = DateTime.Now
		};

		var http = ClientFactory.CreateClient("API");
		var response = await http.PostAsJsonAsync(
		"api/simulation/history", 
		record
		);

		if (!response.IsSuccessStatusCode)
		{
			var error = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"POST FAILED: {error}");
			throw new Exception(error);
		}

	}

}