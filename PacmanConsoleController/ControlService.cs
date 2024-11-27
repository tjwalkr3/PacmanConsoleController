using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PacmanConsoleController;

public class ControlService(HttpClient client)
{
	// Possible (Correct) Direction Inputs
	// UP:    "up",    "u", "↑", "arrowup"
	// DOWN:  "down",  "d", "↓", "arrowdown"
	// LEFT:  "left",  "l", "←", "arrowleft"
	// RIGHT: "right", "r", "→", "arrowright"
	public async Task<HttpResponseMessage?> MovePlayerAsync(string? direction)
	{
		try
		{
			return await client.PatchAsJsonAsync($"Player/redirect", direction);
		}
		catch (Exception ex)
		{
			Console.Write($"\n{ex.Message}");
			return null;
		}
	}
}
