using Microsoft.AspNetCore.WebUtilities;
using MP_Project.Shared;

public class SimulationService
{
	private readonly IHttpClientFactory ClientFactory;
	private readonly Random _rand = new();
	private CancellationTokenSource? _cts;

	public List<Sale> Sales { get; private set; } = new();
	public bool IsRunning { get; private set; }

	public decimal CurrentBalance => currentBalance;

	public event Action? OnChange;
	private void NotifyStateChanged() => OnChange?.Invoke();

	public SimulationService(IHttpClientFactory factory)
	{
		ClientFactory = factory;
	}

	public async Task RunSimulationAsync(
		int days,
		int customersPerDay,
		int secondsPerDay,
		List<Persona> personas,
		List<Product> products,
		int userId)
	{
		if (IsRunning)
		{
			Console.WriteLine("Simulation already running - restarting...");
			Stop();
		}

		_cts = new CancellationTokenSource();
		IsRunning = true;
		NotifyStateChanged();

		try
		{
			if (personas == null || personas.Count == 0)
				return;

			var http = ClientFactory.CreateClient("API");

			var query = new Dictionary<string, string?>
			{
				["currentUserId"] = userId.ToString()
			};

			var url = QueryHelpers.AddQueryString(
				"api/simulation/history/byuser",
				query
			);

			var history = await http.GetFromJsonAsync<List<SimulationHistory>>(url);

			if (history != null && history.Any())
			{
				currentDay = history.Max(x => x.Day);
				currentBalance = history.OrderBy(x => x.Day).Last().Balance;
			}
			else
			{
				currentDay = 0;
				currentBalance = 0;
			}

			int dayDelay = secondsPerDay * 1000;

			for (int day = 1; day <= days; day++)
			{
				decimal dailyRevenue = 0;
				decimal dailyCosts = 0;

				for (int c = 0; c < customersPerDay; c++)
				{
					if (_cts.Token.IsCancellationRequested)
						return;

					var persona = personas[_rand.Next(personas.Count)];
					var sales = GenerateSales(persona, products, userId);

					Sales.AddRange(sales);

					foreach (var sale in sales)
					{
						dailyRevenue += sale.TotalPrice;

						try
						{
							var stockUrl = $"api/products/{sale.ProductID}/decrease-stock?quantity={sale.Quantity}&day={day}&currentUserId={userId}";
							var response = await http.PutAsync(stockUrl, null);

							if (!response.IsSuccessStatusCode)
							{
								var error = await response.Content.ReadAsStringAsync();
								Console.WriteLine(error);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine($"STOCK ERROR: {ex.Message}");
						}
					}
				}

				var dayQuery = new Dictionary<string, string?>
				{
					["day"] = day.ToString()
				};

				var dayUrl = QueryHelpers.AddQueryString(
					"api/suppliertransactions/delivery-cost-today",
					dayQuery
				);

				var deliveryCost = await http.GetFromJsonAsync<decimal>(dayUrl);
				dailyCosts += deliveryCost;

				await RecordDayAsync(dailyRevenue, dailyCosts, userId);

				NotifyStateChanged();

				await Task.Delay(dayDelay, _cts.Token);
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
			NotifyStateChanged();
		}
	}

	public void Stop()
	{
		if (_cts != null && !_cts.IsCancellationRequested)
			_cts.Cancel();

		Console.WriteLine("SIMULATION STOPPED");
		IsRunning = false;
		NotifyStateChanged();
	}

	private List<Sale> GenerateSales(Persona persona, List<Product> products, int userId)
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
				SaleDate = DateTime.Now,
				UserId = userId
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

	private async Task RecordDayAsync(decimal revenue, decimal costs, int userId)
	{
		currentDay++;
		currentBalance += (revenue - costs);

		var record = new SimulationHistory
		{
			Day = currentDay,
			Balance = currentBalance,
			Revenue = revenue,
			Costs = costs,
			Timestamp = DateTime.Now,
			UserId = userId
		};

		var http = ClientFactory.CreateClient("API");

		var response = await http.PostAsJsonAsync(
			"api/simulation/history",
			record
		);

		if (!response.IsSuccessStatusCode)
		{
			var error = await response.Content.ReadAsStringAsync();
			throw new Exception(error);
		}
	}
}