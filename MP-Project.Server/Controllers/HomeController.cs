using Microsoft.AspNetCore.Mvc;

namespace MP_Project.Server.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
