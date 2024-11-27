using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace PacmanConsoleController;

internal class Program
{
	private static async Task Main()
	{
		// Create an instance of ControlService
		var builder = new ControlServiceBuilder();
		builder.AddApiKey();
		builder.AddHttpClient();
		ControlService control = builder.Build();

		Console.WriteLine("Use WASD or Arrow keys to move the player (Press 'Q' to quit).");

		// Loop to listen for key presses
		while (true)
		{
			// Wait for a key press
			var key = Console.ReadKey(intercept: true).Key;

			// Logic to determine movement direction based on key press
			string? direction = key switch
			{
				ConsoleKey.W or ConsoleKey.UpArrow => "up",
				ConsoleKey.A or ConsoleKey.LeftArrow => "left",
				ConsoleKey.S or ConsoleKey.DownArrow => "down",
				ConsoleKey.D or ConsoleKey.RightArrow => "right",
				ConsoleKey.Q => null,
				_ => string.Empty
			};

			if (direction == null) return;

			if (direction != string.Empty)
			{
				HttpResponseMessage? response = await control.MovePlayerAsync(direction);

				if (response == null)
				{
					Console.Write(" Press q to quit.");
				}
				else if (response.IsSuccessStatusCode)
				{
					Console.WriteLine($"You moved {direction}.");
				}
				else
				{
					Console.WriteLine("Invalid API Key or User ID, press q to quit.");
				}
			}
		}
	}
}


