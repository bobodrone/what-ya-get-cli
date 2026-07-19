using QRCoder;
using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections;

var apiUrl = "https://api.pennache.art/api/items";
//var apiUrl = "http://localhost:3000/api/items";

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

    if (items?.Count > 0)
    {
        var item = items.Shuffle().First();
        var text = $"https://what-ya-get.pennache.art/?wyg={item.Id}";

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

        var rows = new Rows(
            new Markup($"[red]{item.Nth}[/] [white]{item.Unit}[/]").Justify(Justify.Center),
            new Markup($"[yellow]{item.Name}[/]").Justify(Justify.Center),
            new Text(""),
            RenderQr(qrCodeData.ModuleMatrix),
            new Text(""),
            new Markup($"[yellow]Video: {item.Art}[/]").Justify(Justify.Center)
        );

        var left = new Panel(rows)
            .Header("What Ya Get", Justify.Center)
            .BorderColor(Color.Green)
            .Padding(4, 2);

        AnsiConsole.Write(left);
    }
}
catch (HttpRequestException ex)
{
    AnsiConsole.MarkupLine($"[red]Failed to reach API:[/] {ex.Message}");
}

static IRenderable RenderQr(List<BitArray> moduleMatrix)
{
    const bool white = false;
    const bool black = true;

    var topWhiteBottomWhite = " ";
    var topWhiteBottomBlack = "▄";
    var topBlackBottomWhite = "▀";
    var topBlackBottomBlack = "█";

    var rows = new List<BitArray>(moduleMatrix);

    if (rows.Count % 2 != 0)
        rows.Add(new BitArray(rows[0].Count));

    var lines = new List<IRenderable>();

    for (int row = 0; row < rows.Count; row += 2)
    {
        var line = new System.Text.StringBuilder();

        for (int col = 0; col < rows[row].Count; col++)
        {
            var top = rows[row][col];
            var bottom = rows[row + 1][col];

            if (top == white && bottom == white)
                line.Append(topWhiteBottomWhite);
            else if (top == white && bottom == black)
                line.Append(topWhiteBottomBlack);
            else if (top == black && bottom == white)
                line.Append(topBlackBottomWhite);
            else
                line.Append(topBlackBottomBlack);
        }

        lines.Add(
            new Markup($"[black on white]{EscapeMarkup(line.ToString())}[/]")
                .Justify(Justify.Center));
    }

    return new Rows(lines);
}

static string EscapeMarkup(string text) =>
    text.Replace("[", "[[").Replace("]", "]]");


// A record to represent the JSON shape returned by the API
record Item(string Name, decimal Nth, string Unit, string Art, int Id);