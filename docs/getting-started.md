# Getting Started

## Download

[:material-download: Download Latest Release](https://github.com/ucoruh/pdf-merger/releases/latest){ .md-button .md-button--primary }
[:material-github: View on GitHub](https://github.com/ucoruh/pdf-merger){ .md-button }

---

## System Requirements

- Windows 10 or later (64-bit)
- No additional runtime required (self-contained executable)

## Installation

### Option 1: Installer (Recommended)

1. Go to the [Releases](https://github.com/ucoruh/pdf-merger/releases/latest) page
2. Download `PdfMergerSetup-vX.X.X.exe`
3. Run the installer and follow the wizard

### Option 2: Portable ZIP

1. Go to the [Releases](https://github.com/ucoruh/pdf-merger/releases/latest) page
2. Download `PdfMerger-vX.X.X.zip`
3. Extract the archive to a folder of your choice
4. Run `PdfMerger.exe`

!!! tip
    No installation required. Just extract and run.

### Option 3: Build from Source

1. Install the [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Clone the repository:

    ```bash
    git clone https://github.com/ucoruh/pdf-merger.git
    cd pdf-merger
    ```

3. Build and run:

    ```bash
    dotnet run --project src/PdfMerger/PdfMerger.csproj
    ```

## First Use

1. Launch the application
2. Add PDF files using drag-and-drop or the **Add** button
3. Select the appropriate operation:
    - **Merge Sequential** for combining multiple PDFs
    - **Merge ADF Odd/Even Mix** for interleaving scanner pages
    - **Thesis Update** for inserting signed pages into a thesis
    - **Split PDF** for splitting a PDF into multiple files
    - **Extract Pages** for extracting specific pages
    - **Rotate Pages** for rotating all pages by 90°, 180°, or 270°
4. Choose the output file location and the result will open automatically

![Main Window](assets/screenshots/2021-12-30-23-00-47-image.png)
