# Ludico

**A universal game launcher and downloader for Linux, built with C#, .NET 6, and Gtk#.**

Ludico is a high-performance, universal platform for game launching. It is written in C# and uses Gtk# to create a native GTK 3 interface. The project starts by supporting plugins from [Project GLD](https://github.com/Y0URD34TH/Project-GLD), with future plans to integrate sources from [Hydra](https://github.com/hydralauncher/hydra).

## Why Ludico?

The landscape of open-source game launchers has become fragmented, leading to multiple, incompatible plugin formats. Ludico aims to solve this by unifying the most popular plugin and source formats into a single, powerful application, providing a streamlined experience for both gamers and developers on Linux.

## Features

  - **Native GTK 3 Interface:** Built with Gtk# to provide a stable, fast, and classic interface that integrates well with the Linux desktop.
  - **Project GLD Compatibility:** Implements the plugin specification from Project GLD.
  - **Future-Proof Design:** Planned support for Hydra's "source" format to create a truly unified launcher.

## Implementation Status

The immediate development focus is on the core UI and achieving full compatibility with the Project GLD plugin format.

### Project GLD Plugin Status

The table below tracks the implementation status for **Project GLD's** methods:

| Method        | Status     | Description                                     |
|---------------|------------|-------------------------------------------------|
| `Client`      | 🟡 Partial | Missing functions for Loading, Unloading and Save Paths |
| `JsonWrapper` | ✅ Implemented | Handles basic JSON-based plugin definitions.    |
| `Game`        | ✅ Implemented | Manages game-specific data and actions.         |
| `Game Library`| ❌ Not Planned | For managing the user's library of games.       |
| `Menu`        | ❌ Not Planned | For creating custom menu entries.               |
| `HTTP`        | ✅ Implemented | Allows plugins to make HTTP requests.           |
| `Utils`       | ❌ Planned | General utility functions for plugins.          |
| `Communication`| 🟡 Partial | Plugin-C# communication is being written  |
| `File`        | ❌ Planned | For reading and writing files.                  |
| `Settings`    | ❌ Not Planned | Allows plugins to modify Ludico's settings   |
| `Download`    | ❌ Planned | A robust system for managing game downloads.    |
| `Notifications`| ✅ Implemented | Provides native desktop notifications.          |
| `SteamAPI`    | ❌ Not Planned | Integration with the Steam API.                 |
| `Zip`         | ❌ Planned | For handling compressed archive files.          |
| `GLDConsole`  | ❌ Not Planned | Access to the Ludico logs can be had by running it via the terminal |
| `DLL`         | ❌ Not Planned | Support for DLL injection into apps |

### Future: Hydra Source Support

Once the core UI and GLD implementation are stable, work will begin on integrating support for Hydra's "source" system.

## Getting Started

### Prerequisites

  * [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
  * GTK 3 Runtime Libraries

### Building and Running

```bash
# Clone the repository
git clone https://github.com/argonptg/Ludico.git

# Navigate to the project directory
cd Ludico

# Restore dependencies and build the project
dotnet build

# Run the application
dotnet run
```

## How to Contribute

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

You can contribute in many ways:

  * **Implement a Feature:** Help complete the GLD method implementation or, in the future, help build the Hydra source support.
  * **Fix a Bug:** Browse the issues tab and help squash some bugs.
  * **Improve Documentation:** Even a small grammar fix in this README is a valuable contribution.
  * **Submit Ideas:** Have an idea to make Ludico better? Open an issue to share it.

### Development Workflow

1.  Fork the Project.
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`).
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`).
4.  Push to the Branch (`git push origin feature/AmazingFeature`).
5.  Open a Pull Request.

## License

This project is licensed under the **Mozilla Public License v2.0**. See the `LICENSE` file for more details.
