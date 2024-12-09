using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PacmanConsoleController;

public class ControlService(HttpClient client)
{
	private int? currentGame = null;

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

	public async Task<bool> JoinGame()
	{
		bool selected = false;
		List<GameDTO>? games = await client.GetFromJsonAsync<List<GameDTO>>("game");
		List<int> activeGames = new List<int>();

		while (!selected && games != null && games.Count != 0)
		{
			Console.Clear();
			Console.WriteLine("Running games:\n---------------------------------------");
			foreach (GameDTO game in games)
			{
				if (!game.isRunning && !game.hasEnded)
				{
					activeGames.Add(game.gameId);
					Console.WriteLine($"ID: {game.gameId}, Map #: {game.mapId}, Players: {game}");
				}
			}

			Console.Write("\n\n Choose a game: ");
			string? choice = Console.ReadLine();
			bool parsed = int.TryParse(choice, out int numChoice);
			HttpResponseMessage? response = null;
			if (parsed && activeGames.Contains(numChoice))
			{
				response = await client.PostAsJsonAsync($"game/add/{numChoice}", new RequestTupleCoords { X = 1, Y = 1});
			}

			if (response != null && response.IsSuccessStatusCode)
			{
				selected = true;
			} 
		}

		Console.Clear();
		return selected;
	}
}
