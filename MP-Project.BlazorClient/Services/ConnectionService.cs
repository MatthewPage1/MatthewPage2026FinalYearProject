public class ConnectionService
{
	private string? _connection;

	public void SetConnection(string conn)
	{
		_connection = conn;
	}

	public string? GetConnection()
	{
		return _connection;
	}

	public void Clear()
	{
		_connection = null;
	}
}