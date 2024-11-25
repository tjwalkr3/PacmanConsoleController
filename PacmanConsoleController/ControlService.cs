using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace PacmanConsoleController;

public class ControlService
{
	private string baseUrl = "http://localhost:5041/";
	private HttpClient _client;

	public ControlService(string apiKey, string userId)
	{
		_client = new HttpClient
		{
			BaseAddress = new Uri(baseUrl),
			DefaultRequestHeaders =
			{
				{ "X-API-Key", apiKey },
				{ "X-API-User", userId }
			}
		};
	}

	// Possible (Correct) Direction Inputs
	// UP:    "up",    "u", "↑", "arrowup"
	// DOWN:  "down",  "d", "↓", "arrowdown"
	// LEFT:  "left",  "l", "←", "arrowleft"
	// RIGHT: "right", "r", "→", "arrowright"
	public async Task<HttpResponseMessage?> MovePlayerAsync(string direction)
	{
		try
		{
			return await _client.PatchAsJsonAsync($"Player/redirect", direction);
		}
		catch (Exception ex)
		{
			Console.Write($"\n{ex.Message}");
			return null;
		}
	}
}
