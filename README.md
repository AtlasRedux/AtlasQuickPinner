# Atlas QuickPinner

A tiny always-on-top Windows utility for quick folder access with a native dark title bar.

## Features

- **Always on top**: Floats above other windows
- **Dark title bar**: Native Windows dark mode via DWM API (`DWMWA_USE_IMMERSIVE_DARK_MODE`)
- **Vertical layout**: Unlimited vertical bars, auto-adjusting based on window height
- **Two actions per bar**:
  - **Select**: Choose a folder to pin
  - **Open**: Open the pinned folder in File Explorer
- **Compact**: Fits to the side of your screen
- **Draggable**: Click anywhere on the window to drag
- **Persistent**: Saves your pinned folders automatically (unlimited bars)
- **Resizable**: Auto-adjusts the number of bars based on window height

## Quick Start

1. **Build** (if needed): Run `build.bat` to compile
2. **Launch**: Run `Launch.bat` to start
3. **Position**: Drag the window to your desired location
4. **Pin folders**: Click **Select** on any bar to choose a folder
5. **Quick access**: Click **Open** to quickly access that folder

## Configuration

Folders are stored in `config.json` next to the executable:

```
<app_directory>\config.json
```

The config file stores:
- Window size and position
- Your pinned folder paths (unlimited)

## Build

### Single-file self-contained build (recommended for distribution):

```batch
build.bat
```

This produces a standalone executable in the `publish` folder that includes the .NET runtime. No external dependencies needed.

### Debug build:

```batch
dotnet build QuickPinner.csproj
```

Output: `bin\Debug\net9.0-windows\AtlasQuickPinner.dll`

### Clean:

```batch
clean.bat
```

Or manually:
```batch
dotnet clean
```

## Requirements

- **For building**: .NET 9 SDK
- **For running (debug)**: .NET 9 Runtime
- **For running (self-contained)**: Windows 10/11 (no .NET needed)

## Technical Details

- **Framework**: .NET 9 / WinForms
- **Language**: C#
- **Dark title bar**: Windows DWM API (`DwmSetWindowAttribute`)
- **Self-contained build**: Single-file executable (~160MB) with embedded .NET runtime
- **Unlimited bars**: Auto-adjusts based on window height
- **Source size**: ~20KB

## Project Structure

```
AtlasQuickPinnerGHRelease/
├── QuickPinner.csproj      # Project file
├── Program.cs              # Entry point
├── QuickPinnerForm.cs      # Main form (dark title bar logic)
├── build.bat               # Build script (self-contained)
├── clean.bat               # Clean script
├── Launch.bat              # Quick launch
├── config.json             # Config template
├── README.md               # This file
└── .gitignore              # Git ignore rules
```

## License

MIT License
