# PDF Merger

![PDF Merger Logo](assets/logo.png)

[![Build and Test](https://github.com/ucoruh/pdf-merger/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/ucoruh/pdf-merger/actions/workflows/build-and-test.yml)
[![Latest Release](https://img.shields.io/github/v/release/ucoruh/pdf-merger)](https://github.com/ucoruh/pdf-merger/releases/latest)
[![Documentation](https://img.shields.io/badge/docs-MkDocs-blue)](https://ucoruh.github.io/pdf-merger/)

A Windows desktop application for merging PDF files with specialized support for **single-side ADF (Automatic Document Feeder) scanners**. Built with C# WinForms and [PDFsharp](http://www.pdfsharp.net/).

## Features

### Sequential Merge

Combine multiple PDF files into a single document. Pages are appended sequentially in the order you specify.

### ADF Odd/Even Page Merge

Designed for scanners that only scan one side at a time. Provide two PDFs (odd pages and even pages), and the tool interleaves them into a properly ordered double-sided document.

### Thesis Update

Insert signed signature and declaration pages into your thesis PDF at the correct positions (pages 3 and 4).

## Screenshots

| Main Window | Adding Files |
|-------------|--------------|
| ![Main Window](assets/2021-12-30-23-00-47-image.png) | ![Adding Files](assets/2021-12-30-23-05-18-image.png) |

## Installation

### Download a Release

Download the latest release from the [Releases](https://github.com/ucoruh/pdf-merger/releases) page.

### Build from Source

1. Install [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/) with the **.NET desktop development** workload
2. Clone the repository:

   ```bash
   git clone https://github.com/ucoruh/pdf-merger.git
   ```

3. Open `PdfMerger.sln` in Visual Studio
4. Restore NuGet packages and build the solution

## Usage

### Adding PDF Files

- **Drag and drop** PDF files into the white list area
- **Or** click the **Add** button to browse and select files (multi-select supported)

### Merge Sequentially

1. Add 2 or more PDF files
2. Reorder files using **Move Up** / **Move Down** buttons
3. Click **Merge Sequential**
4. Choose the output file location

### Merge ADF Odd/Even

1. Add exactly 2 PDF files:
   - First file: odd-numbered pages
   - Second file: even-numbered pages
2. Click **Merge ADF Odd/Even Mix**
3. Choose the output file location

### Update Thesis

1. Add exactly 3 PDF files in this order:
   - Thesis document
   - Signatures page
   - Declaration page
2. Click **Thesis Update**
3. Choose the output file location

## Development

### Prerequisites

- Visual Studio 2022 Community Edition or later
- .NET Framework 4.8 SDK
- (Optional) WiX Toolset v3.11+ for building the installer

### Project Structure

```text
pdf-merger/
  src/
    PdfMerger/              # Main WinForms application
      Services/             # PDF merge business logic
      MainForm.cs           # UI code
      Program.cs            # Entry point
    PdfMergerSetup/         # WiX installer project
  tests/
    PdfMerger.Tests/        # NUnit unit tests
  docs/                     # MkDocs documentation source
  doxygen/                  # Doxygen configuration
  .github/workflows/        # CI/CD pipelines
```

### Build Commands

```bash
nuget restore PdfMerger.sln
msbuild PdfMerger.sln /p:Configuration=Release
```

### Run Tests

```bash
dotnet test tests/PdfMerger.Tests/PdfMerger.Tests.csproj
```

## Documentation

- [Online Documentation](https://ucoruh.github.io/pdf-merger/) (MkDocs)
- [API Reference](https://ucoruh.github.io/pdf-merger/api-reference/)

## Contributing

Contributions are welcome! Please read the [Contributing Guide](CONTRIBUTING.md) for details.

## Author

**Ugur CORUH** - [GitHub](https://github.com/ucoruh)
