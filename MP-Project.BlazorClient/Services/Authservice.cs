
using System.ComponentModel;

public class AuthService
{
	public event Action? OnLoginStateChanged;
	public bool IsLoggedIn { get; private set; }
	public int UserId { get; private set; }
	public string DisplayName { get; private set; }

	public void Login(int userId, string displayName)
	{
		IsLoggedIn = true;
		UserId = userId;
		DisplayName = displayName;
		Console.WriteLine("LOGIN EVENT FIRED");
		OnLoginStateChanged?.Invoke();
		Console.WriteLine($"User {userId} logged in.");
	}

	public void Logout()
	{
		IsLoggedIn = false;
		UserId = 0;
		OnLoginStateChanged?.Invoke();
	}
}