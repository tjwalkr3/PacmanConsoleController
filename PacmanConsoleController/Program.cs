namespace PacmanConsoleController;

internal class Program
{
	private static async Task Main(string[] args)
	{
		// Prompt the user for API key and user ID
		Console.Write("Enter your API key: ");
		string apiKey = Console.ReadLine()!;

		Console.Write("Enter your user ID: ");
		string userId = Console.ReadLine()!;

		// Create an instance of ControlService
		var controlService = new ControlService(apiKey, userId);

		Console.WriteLine("Use WASD or Arrow keys to move the player (Press 'Q' to quit).");

		// Loop to listen for key presses
		while (true)
		{
			// Wait for a key press
			var key = Console.ReadKey(intercept: true).Key;

			// Logic to determine movement direction based on key press
			string direction = string.Empty;
			switch (key)
			{
				case ConsoleKey.W:
				case ConsoleKey.UpArrow:
					direction = "up";
					break;
				case ConsoleKey.A:
				case ConsoleKey.LeftArrow:
					direction = "left";
					break;
				case ConsoleKey.S:
				case ConsoleKey.DownArrow:
					direction = "down";
					break;
				case ConsoleKey.D:
				case ConsoleKey.RightArrow:
					direction = "right";
					break;
				case ConsoleKey.Q:
					Console.WriteLine("Exiting...");
					return;
				default:
					continue;
			}

			// Call the MovePlayerAsync method with the direction
			HttpResponseMessage? response = await controlService.MovePlayerAsync(direction);

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


