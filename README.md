# GarnetDesktop

A lightweight WPF desktop application for managing multiple instances of Microsoft Garnet cache servers as Windows Services.

GarnetDesktop provides a simple GUI to create, configure, install, start, stop, and monitor local Garnet server instances without manually working with command-line tools or Windows Service commands.

## Features

- Create and manage multiple Garnet instances
- Install/uninstall Garnet instances as Windows Services
- Start / stop / restart services
- Configure ports, authentication, memory, persistence, and storage settings
- View instance status
- Store per-instance configuration
- Simple WPF-based desktop UI
- MIT Licensed

## Requirements

- Windows 10 / 11
- .NET Desktop Runtime
- Garnet server binaries
- Administrator privileges (required for Windows Service operations)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/rmin001/GarnetDesktop.git
cd GarnetDesktop
```

### 2. Build the project

Open the solution in Visual Studio and build:

```bash
Build -> Build Solution
```

Or use the .NET CLI:

```bash
dotnet build
```

### 3. Run the application

```bash
dotnet run
```

## Typical Workflow

1. Create a new Garnet instance
2. Select the Garnet executable path
3. Configure server options
4. Install as Windows Service
5. Start the instance
6. Monitor status from the dashboard

## Screenshots

_Add screenshots here_

## Roadmap

- Logs viewer
- Real-time metrics
- Backup / restore
- Export / import configuration
- Tray icon support
- Auto-start management
- Remote instance management

## License

This project is licensed under the MIT License.

## Related Projects

- Microsoft Garnet: https://microsoft.github.io/garnet/
- Garnet GitHub Repository: https://github.com/microsoft/garnet
