using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace PacmanConsoleController;

public class ControlServiceBuilder
{
	private readonly Dictionary<string, string> _configs = [];
	private readonly List<string> _fieldKeys = ["apiKey", "baseAddress"];
	private readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
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
		string path = Path.Combine(Environment.CurrentDirectory, "appSettings.json");

		if (File.Exists(path))
		{
			var builder = new ConfigurationBuilder().AddJsonFile(path, false, false);
			IConfiguration config = builder.Build();

			foreach (string fieldKey in _fieldKeys)
			{
				if (PromptSetNew(fieldKey, config))
				{
					PromptValue(fieldKey);
				}
				else
				{
					_configs[fieldKey] = config.GetSection("pacmanConfig")[fieldKey]!;
				}
			}
		}
		else
		{
			foreach (string fieldKey in _fieldKeys)
			{
				PromptValue(fieldKey);
			}
		}

		WriteConfigsToFile(_configs, path);
		Console.Clear();
		return this;
	}

	private void PromptValue(string fieldKey)
	{
		Console.Clear();
		Console.Write($"\nEnter your {fieldKey}: ");
		string newFieldKey = Console.ReadLine()!;
		_configs[fieldKey] = newFieldKey;
	}

	private static bool PromptSetNew(string fieldKey, IConfiguration config)
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
		var jsonObject = new { pacmanConfig = configValues };
		string jsonString = JsonSerializer.Serialize(jsonObject, jsonOptions);
		File.WriteAllText(path, jsonString);
	}
}

