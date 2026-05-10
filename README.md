[English](README.md) | [简体中文](README.zh-CN.md)

---

# ChildGuard (Eyesight Protector) 🛡️👀

[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://microsoft.com/windows)
[![Language](https://img.shields.io/badge/Language-C%23-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Version](https://img.shields.io/badge/Version-v1.2-orange.svg)]()

**ChildGuard** is a professional Windows desktop application designed to protect children's eyesight. It runs silently in the background, monitors continuous computer usage, and enforces mandatory breaks with a full-screen lock to ensure children leave their seats and rest their eyes.

## ✨ Key Features

* **⏳ Smart Mandatory Rest**: Automatically triggers a full-screen, animated lock screen when usage time is up, preventing further operation until the rest period ends.
* **⚠️ Early Warning System**: Provides a countdown overlay (supports dragging) and custom audio alerts before the lock screen initiates.
* **🔒 Parental Control Panel**: All core settings (usage time, rest duration, passwords) are protected by a parental administrative password.
* **⌨️ Stealth Mode & Global Hotkeys**: Operates without a tray icon for true "silent" protection. Users can invoke the control panel using a customizable global hotkey.
* **🌍 Multi-language Support (i18n)**: Automatically detects the OS language and switches between **English (en_US)** and **Chinese (zh_CN)**.
* **🎨 Immersive Dark UI**: Features custom-drawn WinForms components, including rounded password boxes and dark-themed menus for a modern aesthetic.

## 🚀 Getting Started

### 1. Default Credentials
After running the application for the first time, use these defaults to access the control panel:
* **Default Hotkey**: `Ctrl + Alt + S`
* **Default Password**: `123456`

### 2. System Requirements
* **OS**: Windows 7 / 10 / 11
* **Runtime**: .NET Framework 4.5 or higher (pre-installed on most Windows systems)

### 3. Usage
1. Download the `ChildGuard.exe` from the latest Release.
2. Double-click to run. The program will enter background monitoring mode immediately.
3. Press `Ctrl + Alt + S` to verify your identity and customize your rules.

## 🛠️ Compilation (Command Line)

As a single-file C# project, you can compile it directly using the built-in Windows C# compiler without any heavy IDE like Visual Studio.

Open CMD and run the following command:
```cmd
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe /out:ChildGuard.exe "ChildGuard v1.2.cs"
```

## 📜 Credits & License

* **Author**: Lawyer Xu (大许律师)
* **Version**: v1.2
* **Copyright**: Copyright © 2026

Dedicated to protecting the healthy growth of children.