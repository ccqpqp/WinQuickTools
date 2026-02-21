## Build

This project can be built directly from source.

Requirements:
- .NET 8 SDK

Build:

dotnet build

Publish single EXE:

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true