# Atlas QuickPinner

Quickly pin your workspace- or favourite folders and open them quickly and easily!
A lightweight, always-on-top, fully resizeable and scaleable with automatic button addition/removal with memory, Windows sidebar for instant access to your active folders, instead of Windows' terrible Pin feature. No more digging through File Explorer — pin your folders once, open them with a single click.

<img width="239" height="334" alt="Skjermbilde 2026-04-02 134224" src="https://github.com/user-attachments/assets/610048b0-c2a6-4427-91e2-025c13f43792" />


![Windows 10/11](https://img.shields.io/badge/Windows-10%2F11-0078D6?logo=windows) ![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet) ![License: MIT](https://img.shields.io/badge/License-MIT-green)

## What It Does

QuickPinner is a compact floating panel that sticks to the edge of your screen. Each row ("bar") holds one pinned folder — select it once, then open it instantly anytime. The window resizes dynamically: stretch it taller to get more bars, shrink it to get fewer.

## Features

- **Always on top** — stays visible while you work in other apps
- **Native dark title bar** — uses the Windows DWM API for a clean Windows 11 look
- **Unlimited folder bars** — auto-adjusts to window height (60px per bar)
- **One-click folder access** — Select, Open, or Clear any pinned folder
- **Drag anywhere** — click and drag the window body to reposition
- **Auto-save** — window size, position, and all pinned folders persist across sessions
- **Zero dependencies** — ships as a single self-contained `.exe`

### NOTE! It's a large single binary, because .NET 9 is embedded into it.

## Quick Start

```
build.bat        # compile (produces publish\AtlasQuickPinner.exe)
Launch.bat       # run
```

1. Drag the window to a screen edge
2. Click **Select** on any bar to pin a folder
3. Click **Open** to jump straight to it in File Explorer
4. Click **Clear** (red) to unpin

## Building from Source

### Requirements

| | Minimum |
|---|---|
| **OS** | Windows 10 build 1809+ |
| **SDK** | .NET 9 SDK (build only) |
| **Runtime** | None — self-contained build bundles .NET |

### Self-contained release build (recommended)

```batch
build.bat
```

Produces `publish\AtlasQuickPinner.exe` (~160 MB) — a single-file executable with the .NET 9 runtime embedded. Runs on any Windows 10/11 machine with nothing else installed.

### Debug build

```batch
dotnet build QuickPinner.csproj
```

Output: `bin\Debug\net9.0-windows\AtlasQuickPinner.dll` (requires .NET 9 Runtime).

### Clean

```batch
clean.bat
```

Removes `bin/`, `obj/`, and `publish/` directories.

## Configuration

All state is saved to `config.json` in the same directory as the executable:

```json
{
  "size": [220, 325],
  "location": [0, 100],
  "paths": ["C:\\Projects", "D:\\Downloads", null, null, null]
}
```

| Field | Description |
|---|---|
| `size` | Window width and height in pixels |
| `location` | Window X and Y screen position |
| `paths` | Array of pinned folder paths (`null` = empty bar) |

The config is written automatically when you close the app or change a pin. You can also edit it by hand.

## Project Structure

```
├── QuickPinnerForm.cs   # All UI and logic (~525 lines)
├── Program.cs           # Entry point
├── QuickPinner.csproj   # .NET 9 WinForms project
├── config.json          # Saved state (auto-generated)
├── build.bat            # Self-contained publish script
├── clean.bat            # Artifact cleanup
└── Launch.bat           # Quick launcher
```

## Technical Notes

- **Dark title bar**: P/Invoke into `dwmapi.dll` using `DwmSetWindowAttribute` with `DWMWA_USE_IMMERSIVE_DARK_MODE` (attribute 19). Works best on Windows 11; functional on Windows 10 1809+.
- **Dynamic layout**: On resize, the form calculates `Math.Max(1, (Height - 35) / 60)` to decide how many bars to show, then adds or removes controls accordingly.
- **Folder display**: Shows only the leaf folder name via `Path.GetFileName()`, with special handling for drive roots like `C:\`.
- **Window style**: `SizableToolWindow` border gives a minimal title bar while keeping resize handles.

## License

[MIT](https://opensource.org/licenses/MIT)
