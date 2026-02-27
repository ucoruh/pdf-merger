# Development

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/) with the .NET desktop development workload)
- (Optional) [Inno Setup 6](https://jrsoftware.org/isinfo.php) for building the installer

## Building

### Using Visual Studio

1. Open `PdfMerger.sln`
2. Build the solution (`Ctrl+Shift+B`)

### Using Command Line

```bash
dotnet restore PdfMerger.sln
dotnet build PdfMerger.sln --configuration Release
```

## Architecture

### Project Structure

```text
pdf-merger/
  src/
    PdfMerger/              # Main WinForms application (.NET 8)
      Services/
        PdfMergeService.cs  # Core PDF merging logic (static methods)
      MainForm.cs           # UI event handlers
      MainForm.Designer.cs  # Auto-generated UI layout
      Program.cs            # Application entry point
  tests/
    PdfMerger.Tests/        # NUnit unit tests
  installer/
    PdfMergerSetup.iss      # Inno Setup installer script
```

### Design Principles

- **Separation of Concerns:** PDF merge logic is in `PdfMergeService` (testable, no UI dependencies). The `MainForm` handles only UI interactions.
- **Error Handling:** The service layer throws typed exceptions. The UI layer catches and displays user-friendly messages.
- **Disposable Resources:** All `PdfDocument` objects are wrapped in `using` statements to ensure proper cleanup.

### Key Classes

| Class | Purpose |
|-------|---------|
| `PdfMergeService` | Static class with merge, split, extract, and rotate methods |
| `MainForm` | WinForms UI with drag-drop, file management, and operation buttons |
| `InputDialogForm` | Reusable text input dialog (used by Split and Extract) |
| `RotationDialogForm` | Angle selection dialog (used by Rotate) |
| `Program` | Application entry point |

## Testing

### Running Tests

```bash
dotnet test PdfMerger.sln --configuration Release
```

### Test Coverage

Tests cover all six operations (37 tests total):

- Sequential merge with varying file counts
- ADF merge with equal and unequal page counts
- Thesis update with correct and incorrect file counts
- Split PDF with various split points and edge cases
- Extract pages with ranges, single pages, and boundary conditions
- Rotate pages with 90/180/270 degrees and invalid angles
- ParsePageRange with mixed ranges and error cases
- Edge cases: null input, missing files, wrong file counts, out-of-range pages

## CI/CD

GitHub Actions workflows handle:

- **Build & Test:** Runs on every push and pull request
- **Release:** Creates GitHub releases with build artifacts on version tags
- **Documentation:** Deploys MkDocs site to GitHub Pages
