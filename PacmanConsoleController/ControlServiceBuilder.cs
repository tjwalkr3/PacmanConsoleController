using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace PacmanConsoleController;

public class ControlServiceBuilder
{
	private readonly string baseUrl = "http://localhost:5041/";
	private string? _apiKey;
	private HttpClient? _client;

	public ControlService Build()
	{
		if (_client != null )
		{
			return new ControlService(_client);
		}
		else
		{
			throw new Exception("Http client has not been created!");
		}
	}

	public void AddHttpClient()
	{
		if (_apiKey != null)
		{
			_client = new HttpClient
			{
				BaseAddress = new Uri(baseUrl),
				DefaultRequestHeaders =
				{
					{ "X-API-Key", _apiKey }
				}
			};
		}
		else
		{
			throw new Exception("Api key has not been set!");
		}
	}

	public void AddApiKey()
	{
		var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false, false);
		var config = builder.Build();

		string? apiKey = config["testKey"];

		bool? setNewApiKey = null;
		if (apiKey == null || apiKey == string.Empty) setNewApiKey = true;

		while (setNewApiKey == null)
		{
			Console.Write($"\nAPI key found in configuration:\n{apiKey}\n\nDo you want to enter a new API key? [Y/N] ");
			var key = Console.ReadKey(intercept: true).Key;
			setNewApiKey = key switch
			{
				ConsoleKey.Y or ConsoleKey.UpArrow => true,
				ConsoleKey.N or ConsoleKey.LeftArrow => false,
				_ => null
			};
		}

		if (apiKey == null || apiKey == string.Empty || setNewApiKey == true)
		{
			Console.Write("\nEnter your API key: ");
			string newApiKey = Console.ReadLine()!;

			config["testkey"] = newApiKey;
			apiKey = newApiKey;
		}

		_apiKey = apiKey;
	}
}
