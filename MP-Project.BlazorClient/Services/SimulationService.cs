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

	private HashSet<int> processedTransactionIds = new();

	public SimulationService(IHttpClientFactory factory)
	{
		ClientFactory = factory;
	}

	public async Task LoadBalanceAsync(int userId)
	{
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

		NotifyStateChanged();
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

			await LoadBalanceAsync(userId);

			int dayDelay = secondsPerDay * 1000;

			for (int day = 1; day <= days; day++)
			{
				currentDay++;
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
							await http.PutAsync(stockUrl, null);
						}
						catch (Exception ex)
						{
							Console.WriteLine($"STOCK ERROR: {ex.Message}");
						}
					}
				}

				var checkInUrl = QueryHelpers.AddQueryString(
					"api/SupplierTransactions/checkInPendingDeliveries",
					new Dictionary<string, string?>
					{
						["currentUserId"] = userId.ToString()
					});

				var processUrl = QueryHelpers.AddQueryString(
					"api/SupplierTransactions/processDeliveries",
					new Dictionary<string, string?>
					{
						["currentUserId"] = userId.ToString()
					});

				await http.PostAsync(processUrl, null);

				var purchaseUrl = QueryHelpers.AddQueryString(
					"api/suppliertransactions",
					new Dictionary<string, string?>
					{
						["currentUserId"] = userId.ToString()
					});

				var transactions = await http.GetFromJsonAsync<List<PurchaseDto>>(purchaseUrl);

				var newTransactions = transactions?
					.Where(t => t.CheckedIn && !processedTransactionIds.Contains(t.TransactionID))
					.ToList() ?? new List<PurchaseDto>();

				var newCosts = newTransactions.Sum(t => t.TotalPrice);

				foreach (var t in newTransactions)
				{
					processedTransactionIds.Add(t.TransactionID);
				}

				if (newCosts > 0)
				{
					await ApplyCostAsync(newCosts, userId);
				}

				var dayQuery = new Dictionary<string, string?>
				{
					["day"] = day.ToString()
				};

				var deliveryUrl = QueryHelpers.AddQueryString(
					"api/suppliertransactions/delivery-cost-today",
					dayQuery
				);

				var deliveryCost = await http.GetFromJsonAsync<decimal>(deliveryUrl);

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
			IsRunning = false;
			NotifyStateChanged();
		}
	}

	public void Stop()
	{
		if (_cts != null && !_cts.IsCancellationRequested)
			_cts.Cancel();

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
					SaleDate = DateTime.Now,
					UserId = userId
				});
			}
		}

		return sales;
	}

	private int currentDay = 0;
	private decimal currentBalance = 0;

	private async Task RecordDayAsync(decimal revenue, decimal costs, int userId)
	{
		var http = ClientFactory.CreateClient("API");

		var dayToUse = currentDay == 0 ? 1 : currentDay;

		var query = new Dictionary<string, string?>
		{
			["userId"] = userId.ToString(),
			["day"] = dayToUse.ToString()
		};

		var url = QueryHelpers.AddQueryString(
			"api/simulation/history/by-user-day",
			query
		);

		SimulationHistory? existing = null;

		try
		{
			existing = await http.GetFromJsonAsync<SimulationHistory>(url);
		}
		catch { }

		if (existing != null)
		{
			existing.Revenue += revenue;
			existing.Costs += costs;
			existing.Balance += (revenue - costs);
			existing.Timestamp = DateTime.Now;

			await http.PutAsJsonAsync("api/simulation/history", existing);

			currentBalance = existing.Balance;
		}
		else
		{
			currentDay = dayToUse;
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

			await http.PostAsJsonAsync("api/simulation/history", record);
		}

		NotifyStateChanged();
	}

	public async Task ApplyCostAsync(decimal amount, int userId)
	{
		var http = ClientFactory.CreateClient("API");

		var dayToUse = currentDay == 0 ? 1 : currentDay;

		var query = new Dictionary<string, string?>
		{
			["userId"] = userId.ToString(),
			["day"] = dayToUse.ToString()
		};

		var url = QueryHelpers.AddQueryString(
			"api/simulation/history/by-user-day",
			query
		);

		SimulationHistory? existing = null;

		try
		{
			existing = await http.GetFromJsonAsync<SimulationHistory>(url);
		}
		catch { }

		if (existing != null)
		{
			existing.Costs += amount;
			existing.Balance -= amount;

			await http.PutAsJsonAsync("api/simulation/history", existing);

			currentBalance = existing.Balance;
		}
		else
		{
			currentBalance -= amount;

			var record = new SimulationHistory
			{
				Day = dayToUse,
				Balance = currentBalance,
				Revenue = 0,
				Costs = amount,
				Timestamp = DateTime.Now,
				UserId = userId
			};

			await http.PostAsJsonAsync("api/simulation/history", record);
		}

		NotifyStateChanged();
	}
}