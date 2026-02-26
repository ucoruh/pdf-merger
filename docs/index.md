# PDF Merger

Welcome to the PDF Merger documentation.

PDF Merger is a Windows desktop application for merging PDF files with specialized support for **single-side ADF (Automatic Document Feeder) scanners**.

## Key Features

- **Sequential Merge** - Combine multiple PDFs into one document
- **ADF Odd/Even Merge** - Interleave odd and even pages from single-side scanner output
- **Thesis Update** - Insert signature and declaration pages into thesis documents

## Quick Links

- [Getting Started](getting-started.md) - Installation and first use
- [User Guide](user-guide.md) - Detailed usage instructions
- [Development](development.md) - Build from source, architecture, testing
- [API Reference](api-reference.md) - PdfMergeService API documentation
- [Contributing](contributing.md) - How to contribute
- [Changelog](changelog.md) - Version history

## Technology Stack

| Component | Technology |
|-----------|-----------|
| Language | C# (.NET Framework 4.8) |
| UI Framework | Windows Forms |
| PDF Library | [PDFsharp](http://www.pdfsharp.net/) 1.50 |
| Installer | WiX Toolset 3.11 |
| Testing | NUnit 3 |
| CI/CD | GitHub Actions |
| Documentation | MkDocs Material |
