using System.Net.Http.Json;
using MP_Project.Shared;

namespace MP_Project.BlazorClient.Services
{
	public class SupplierTransactionService
	{
		private readonly HttpClient _http;

		public SupplierTransactionService(HttpClient http)
		{
			_http = http;
		}

		public async Task AddSupplierTransaction(Supplier transaction)
		{
			var response = await _http.PostAsJsonAsync(
				"api/SupplierTransactions/addSupplierTransaction",
				transaction
			);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to add supplier transaction");
			}
		}
	}
}