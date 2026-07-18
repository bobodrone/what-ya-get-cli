using System.Text.Json;
using Spectre.Console;

var apiUrl = "https://api.pennache.art/api/items";

using var client = new HttpClient();

try
{
    var response = await client.GetAsync(apiUrl);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync();

    var items = JsonSerializer.Deserialize<List<Item>>(json, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    var table = new Table();
    table.AddColumn("ID");
    table.AddColumn("Name");
    table.AddColumn("Price");

    foreach (var item in items!)
    {
        table.AddRow(item.Id.ToString(), item.Name, item.Price.ToString("C"));
    }

    AnsiConsole.Write(table);
}
catch (HttpRequestException ex)
{
    AnsiConsole.MarkupLine($"[red]Failed to reach API:[/] {ex.Message}");
}

// A record to represent the JSON shape returned by the API
record Item(int Id, string Name, decimal Price);