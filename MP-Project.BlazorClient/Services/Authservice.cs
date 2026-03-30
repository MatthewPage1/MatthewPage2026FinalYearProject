public class AuthService
{
	public bool IsLoggedIn { get; private set; }
	public int UserId { get; private set; }

	public void Login(int userId)
	{
		IsLoggedIn = true;
		UserId = userId;
		Console.WriteLine($"User {userId} logged in.");
	}

	public void Logout()
	{
		IsLoggedIn = false;
		UserId = 0;
	}
}