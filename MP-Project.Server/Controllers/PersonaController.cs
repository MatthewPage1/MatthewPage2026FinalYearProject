using Microsoft.AspNetCore.Mvc;
using MP_Project.BlazorClient.Services;
using MP_Project.Shared;

[ApiController]
[Route("api/[controller]")]
public class PersonasController : ControllerBase
{
	private readonly PersonaService _personaService;

	public PersonasController(PersonaService personaService)
	{
		_personaService = personaService;
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var personas = await _personaService.GetPersonas();
		return Ok(personas);
	}
}