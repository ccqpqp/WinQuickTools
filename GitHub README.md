# WinQuickTools

Lightweight Windows utility for quickly toggling hidden system settings.

🔽 Download (Prebuilt EXE):
https://winquicktools-site.web.app/

## Features
- File extension toggle
- Hidden file toggle
- Explorer restart
- Path copy
- DNS flush
- 

## Build

This project can be built directly from source.

Requirements:
- .NET 8 SDK

Build:

dotnet build

Publish single EXE:


dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
