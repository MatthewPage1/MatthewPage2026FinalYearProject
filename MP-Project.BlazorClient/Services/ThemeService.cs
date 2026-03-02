namespace MP_Project.BlazorClient.Services
{
	public class ThemeService
	{
		private string _currentTheme = "light";

		public string CurrentTheme => _currentTheme;

		public event Action? OnThemeChanged;

		public async Task SetTheme(string theme)
		{
			if (_currentTheme == theme)
				return;

			_currentTheme = theme;

			OnThemeChanged?.Invoke();

			await Task.Delay(1);
		}
	}
}