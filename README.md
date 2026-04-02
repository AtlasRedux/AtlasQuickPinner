# Atlas QuickPinner

A tiny always-on-top Windows utility for quick folder access with a dark title bar.

## Features

- **Always on top**: Floats above other windows
- **Dark title bar**: Native Windows dark mode integration via DWM API
- **Vertical layout**: 5 adjustable bars for your favorite folders
- **Two actions per bar**:
  - **Select**: Choose a folder to pin
  - **Open**: Open the pinned folder in File Explorer
- **Compact**: Fits to the side of your screen
- **Draggable**: Click anywhere to drag
- **Persistent**: Saves your pinned folders automatically
- **Resizable**: Auto-adjusts the number of bars based on height

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

## Build

### Single-file self-contained build (recommended for distribution):
```batch
build.bat
```

This produces a standalone executable in the `publish` folder that includes the .NET runtime.

### Debug build:
```batch
dotnet build
```

### Clean:
```batch
clean.bat
```

## Requirements

- Windows 10/11
- .NET 9 SDK (for building)
- .NET 9 Runtime (for non-self-contained builds)

## Technical Details

- Built with **.NET 9** and **WinForms**
- Uses Windows DWM API (`DWMWA_USE_IMMERSIVE_DARK_MODE`) for native dark title bar
- Single-file self-contained executable (~160MB) or portable debug build
- Source code: ~8KB

## License

MIT License
