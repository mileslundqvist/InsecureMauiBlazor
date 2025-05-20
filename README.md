# Intentional Vulnerable .NET MAUI Blazor Hybrid Application (IVMAUIB)

**Author: Miles Lundqvist**
**University: Umeå University**
**Date: May 20, 2025**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

## Overview

This application, **IVMAUIB** (Intentional Vulnerable .NET MAUI Blazor Hybrid), was developed as a benchmark application for a Master's thesis focused on assessing the detection capabilities of community Static Application Security Testing (SAST) tools against .NET MAUI Blazor Hybrid vulnerabilities.

The application intentionally implements a variety of common security vulnerabilities across different categories relevant to modern mobile and hybrid applications.

**Technologies Used:**
* .NET MAUI (using .NET 9)
* Blazor Hybrid
* C#
* HTML, CSS, JavaScript (within Blazor components and JS interop)

---

## ⚠️ SECURITY WARNING ⚠️

**This application is intentionally vulnerable and designed for research and educational purposes ONLY.**

* **DO NOT** use any part of this code in a production environment.
* **DO NOT** deploy this application to publicly accessible servers or devices where it could be exploited.
* The vulnerabilities are implemented to be easily discoverable and exploitable for testing SAST tools.
* Use this application in isolated and controlled environments.

---

## Vulnerability Categories Demonstrated

This application showcases vulnerabilities in the following areas:

1.  **Authentication & Session Management (`AuthVulnerabilities.razor`)**
2.  **Code Quality & Injection Flaws (`CodeVulnerabilities.razor`)**
3.  **Cryptographic Failures (`CryptoVulnerabilities.razor`)**
4.  **JavaScript-Native Interoperability (`InteropVulnerabilities.razor`)**
5.  **Network Security (`NetworkVulnerabilities.razor`)**
6.  **Platform Interaction & Permissions (`PlatformVulnerabilities.razor`)**
7.  **Insecure Data Storage (`StorageVulnerabilities.razor`)**

Explicit comments in the format `// VULNERABILITY: [Description]` are used throughout the C# code (`.razor` @code blocks and `.cs` service files) to mark the location and nature of intended vulnerabilities.

---

## Setup and Installation

### Prerequisites

* **.NET SDK:** Version 9
* **.NET MAUI Workload:** Ensure the MAUI workload is installed for your .NET SDK.
    ```bash
    dotnet workload install maui
    ```
* **IDE (Recommended):**
    * Visual Studio 2022 with the .NET Multi-platform App UI development workload installed.
* **Target Platform Setup:**
    * **Android:** Android SDK, Emulator, or a physical device with developer mode enabled.
    * **Windows:** Windows 10/11 with Developer Mode enabled.

### Building and Running the Application

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/mileslundqvist/InsecureMauiBlazor.git
    cd InsecureMauiBlazor
    ```
2.  **Open the solution:**
    * Open `InsecureMauiBlazor.sln` (or your solution file name) in Visual Studio.
3.  **Restore dependencies:**
    * Dependencies should restore automatically. If not, right-click the solution in Visual Studio and select "Restore NuGet Packages," or run:
        ```bash
        dotnet restore
        ```
4.  **Select a target framework and device/emulator:**
    * In Visual Studio, choose a target framework (e.g., `net9.0-android`) and a corresponding deployment target (e.g., an Android Emulator).
5.  **Run the application:**
    * Press F5 or click the "Start" button in Visual Studio.

---

## How to Use for SAST Tool Assessment

This application is designed to be scanned by SAST tools.
1.  Ensure the SAST tool you are testing supports .NET MAUI, C#, and Blazor if possible.
2.  Point the SAST tool to this codebase.
3.  Analyze the SAST tool's output, comparing detected vulnerabilities against the intentionally implemented ones (marked with `// VULNERABILITY:` comments and detailed in the section below).
4.  The goal is to measure how many known, intentionally introduced vulnerabilities your chosen SAST tool can identify.

---

## Project Structure (Brief)

* `/InsecureMauiBlazor.csproj`: Main project file.
* `/MauiProgram.cs`: App initialization, service registration.
* `/Pages/`: Blazor components/pages demonstrating vulnerabilities.
* `/Services/`: Interface definitions and their insecure implementations.
    * `IAuthService.cs`, `InsecureAuthService.cs`
    * `ICryptoService.cs`, `InsecureCryptoService.cs`
    * `IDataStorageService.cs`, `InsecureDataStorageService.cs`
    * `INetworkService.cs`, `InsecureNetworkService.cs` 
* `/wwwroot/`: Static web assets including `index.html` for the Blazor Hybrid setup.
    * May contain custom JS for interop demos.
* `/Platforms/`: Platform-specific startup code (Android, iOS, etc.).
    * WebView configurations might be found here.
* `NativeFeatures.cs`: Dummy class for mocked native commands

---

## Author

* Miles Lundqvist

This application was developed as part of a Master's thesis at Umeå University.

---

## Disclaimer

This software is provided "as is", without warranty of any kind, express or implied. The author is not responsible for any misuse of this software or any damage caused by it. It is intended solely for educational and research purposes in controlled environments.
