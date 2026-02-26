# Contributing to PDF Merger

Thank you for your interest in contributing to PDF Merger! This document provides guidelines and instructions for contributing.

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in [Issues](https://github.com/ucoruh/pdf-merger/issues)
2. If not, create a new issue using the bug report template
3. Include steps to reproduce, expected behavior, and actual behavior
4. Add screenshots if applicable

### Suggesting Features

1. Check existing [Issues](https://github.com/ucoruh/pdf-merger/issues) for similar suggestions
2. Create a new issue using the feature request template
3. Describe the feature and its use case clearly

### Pull Requests

1. Fork the repository
2. Create a feature branch from `main`: `git checkout -b feature/your-feature-name`
3. Make your changes following the coding standards below
4. Add or update unit tests as needed
5. Ensure all tests pass
6. Commit with clear, descriptive messages
7. Push to your fork and submit a pull request

## Development Setup

### Prerequisites

- Visual Studio 2022 Community Edition or later
- .NET Framework 4.8 SDK
- WiX Toolset v3.11+ (for installer project)

### Building

```bash
# Restore NuGet packages
nuget restore PdfMerger.sln

# Build the solution
msbuild PdfMerger.sln /p:Configuration=Release
```

### Running Tests

```bash
dotnet test tests/PdfMerger.Tests/PdfMerger.Tests.csproj
```

## Coding Standards

- Follow standard C# naming conventions
- Use XML documentation comments on all public members
- Keep methods focused and under 50 lines where possible
- Separate UI logic from business logic
- Use `using` statements for disposable objects
- Add unit tests for new functionality

## Code of Conduct

Please note that this project has a [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## Questions?

Feel free to open an issue for any questions about contributing.
