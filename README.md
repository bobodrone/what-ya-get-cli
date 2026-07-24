# what-ya-get-cli

You can read more about the main project here: [github.com/bobodrone/what-ya-get](https://github.com/bobodrone/what-ya-get)

A tiny .NET console app that picks a random item from the [Pennache](https://pennache.art) items API and prints it in the terminal as a framed card with a scannable QR code linking to that item.

```
┌────────────────── What Ya Get ──────────────────┐
│                                                 │
│                    3 pieces                     │
│                   Item Name Here                │
│                                                 │
│                 ▄▄▄▄▄▄▄  ▄ ▄▄▄▄▄▄▄              │
│                 █ ▄▄▄ █ ▀█▀ █ ▄▄▄ █             │
│                 █ ███ █ ▄▄  █ ███ █             │
│                 ▀▀▀▀▀▀▀ ▀ ▀ ▀▀▀▀▀▀▀             │
│                                                 │
└─────────────────────────────────────────────────┘
```

The QR code encodes `https://what-ya-get.pennache.art/?wyg=<item id>`.

## How it works

Everything lives in [Program.cs](Program.cs):

1. `GET https://api.pennache.art/api/items` and deserialize the JSON into `Item(Name, Nth, Unit, Art, Id)` records.
2. Shuffle the list and take the first item.
3. Build the item URL and generate a QR matrix with [QRCoder](https://github.com/codebude/QRCoder).
4. Render the matrix with half-block characters (`▀`, `▄`, `█`) so each terminal row holds two QR rows — this keeps the code square and scannable — and draw the whole card with [Spectre.Console](https://spectreconsole.net/).

If the API is unreachable, the app prints a red `Failed to reach API` message and exits.

## Requirements

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) or newer (the app uses the .NET 10 LINQ `Shuffle()` operator)
- A terminal with Unicode and truecolor/ANSI support — the QR code is drawn with block characters on a white background

## Running

```bash
dotnet run
```

## Building

Debug build:

```bash
dotnet build
```

Self-contained single-file binaries (this is how the checked-in [dist/](dist/) folder was produced — one directory per runtime identifier):

```bash
dotnet publish -c Release -r linux-x64  --self-contained -p:PublishSingleFile=true -o dist/linux-x64
dotnet publish -c Release -r win-x64    --self-contained -p:PublishSingleFile=true -o dist/win-x64
dotnet publish -c Release -r osx-x64    --self-contained -p:PublishSingleFile=true -o dist/osx-x64
dotnet publish -c Release -r osx-arm64  --self-contained -p:PublishSingleFile=true -o dist/osx-arm64
```

Each output is a single ~75 MB executable with no runtime prerequisites; run it directly:

```bash
./dist/linux-x64/what-ya-get-cli
```

## Configuration

There is no config file or CLI argument handling. The API endpoint is a constant at the top of [Program.cs:7](Program.cs#L7) — a commented-out `http://localhost:3000/api/items` line sits below it for pointing the CLI at a local API during development.

## Dependencies

| Package | Version | Purpose |
| --- | --- | --- |
| [QRCoder](https://www.nuget.org/packages/QRCoder) | 1.8.0 | QR code matrix generation |
| [Spectre.Console](https://www.nuget.org/packages/Spectre.Console) | 0.57.2 | Panels, markup, and layout |
