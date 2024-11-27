using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PacmanConsoleController;

public class ControlServiceBuilder
{
	private Dictionary<string, string> _configs = new();
	private HttpClient? _client;

	public ControlService Build()
	{
		if (_client != null)
		{
			return new ControlService(_client);
		}
		else
		{
			throw new Exception("Http client has not been created!");
		}
	}

	public ControlServiceBuilder AddHttpClient()
	{
		if (_configs["apiKey"] != null && _configs["baseAddress"] != null)
		{
			_client = new HttpClient
			{
				BaseAddress = new Uri(_configs["baseAddress"]),
				DefaultRequestHeaders =
				{
					{ "X-API-Key", _configs["apiKey"] }
				}
			};
		}
		else
		{
			throw new Exception("apiKey or baseAddress have not been set!");
		}

		return this;
	}

	public ControlServiceBuilder AddConfigs()
	{
		bool setNewApiKey = true;
		bool setNewBaseAddress = true;
		string path = Path.Combine(Environment.CurrentDirectory, "appSettings.json");

		if (File.Exists(path))
		{
			var builder = new ConfigurationBuilder().AddJsonFile(path, false, false);
			IConfiguration config = builder.Build();

			if (PromptSetNew("apiKey", config))
			{
				Console.Write("\nEnter your API key: ");
				string newApiKey = Console.ReadLine()!;
				_configs["apiKey"] = newApiKey;
			}
			else
			{
				_configs["apiKey"] = config.GetSection("pacmanConfig")["apiKey"]!;
			}

			if (PromptSetNew("baseAddress", config))
			{
				Console.Write("\nEnter your base address: ");
				string newBaseAddress = Console.ReadLine()!;
				_configs["baseAddress"] = newBaseAddress;
			}
			else
			{
				_configs["baseAddress"] = config.GetSection("pacmanConfig")["baseAddress"]!;
			}
		}
		else
		{
			Console.Clear();
			Console.Write("\nEnter your API key: ");
			string newApiKey = Console.ReadLine()!;
			_configs["apiKey"] = newApiKey;

			Console.Clear();
			Console.Write("\nEnter your base address: ");
			string newBaseAddress = Console.ReadLine()!;
			_configs["baseAddress"] = newBaseAddress;
		}

		if (setNewApiKey == true || setNewBaseAddress == true || !File.Exists(path)) // write to config file if needed
		{
			WriteConfigsToFile(_configs, path);
		}

		Console.Clear();
		return this;
	}

	private bool PromptSetNew(string fieldKey, IConfiguration config)
	{
		bool? setNewValue = null;

		string? value = config.GetSection("pacmanConfig")[fieldKey];
		if (value == null || value == string.Empty) setNewValue = true;

		while (setNewValue == null)
		{
			Console.Clear();
			Console.Write($"\n{fieldKey} found in configuration:\n{value}\n\nDo you want to enter a new {fieldKey}? [Y/N] ");
			var key = Console.ReadKey(intercept: true).Key;
			setNewValue = key switch
			{
				ConsoleKey.Y => true,
				ConsoleKey.N => false,
				_ => null
			};
		}

		Console.Clear();
		return (bool)setNewValue;
	}

	private void WriteConfigsToFile(Dictionary<string, string> configValues, string path)
	{
		// Create the final object
		var jsonObject = new
		{
			pacmanConfig = configValues
		};

		// Serialize the object to JSON
		var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
		string jsonString = JsonSerializer.Serialize(jsonObject, jsonOptions);

		// Write the JSON to the specified file
		File.WriteAllText(path, jsonString);
	}
}

