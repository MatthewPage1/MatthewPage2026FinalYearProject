using System.Text.Json;
using MP_Project.Shared;

namespace MP_Project.BlazorClient.Services
{
	public class PersonaService
	{
		public async Task<List<Persona>> GetPersonas()
		{
			var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Personas.json");

			var json = await File.ReadAllTextAsync(path);

			return JsonSerializer.Deserialize<List<Persona>>(json);
		}
	}
}
