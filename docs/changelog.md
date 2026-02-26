# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [2.1.0] - 2026-02-27

### Added

- **Split PDF** — Split a PDF at specified page numbers into multiple output files
- **Extract Pages** — Extract specific pages from a PDF using range syntax (e.g., `1-3,5,8-10`)
- **Rotate Pages** — Rotate all pages in a PDF by 90, 180, or 270 degrees
- `ParsePageRange` helper for flexible page range input
- `InputDialogForm` — Reusable text input dialog for Split and Extract operations
- `RotationDialogForm` — Angle selection dialog for Rotate operation
- 21 new unit tests covering Split, Extract, Rotate, and ParsePageRange

### Changed

- UI redesigned: compact 37px buttons, separator between merge and tool operations
- Form height increased to accommodate new operation buttons
- Button states update dynamically based on file list count

## [2.0.0] - 2026-02-27

### Added

- Extracted `PdfMergeService` for clean separation of concerns
- Unit test project with NUnit tests
- MkDocs Material theme documentation website
- Doxygen API documentation configuration
- GitHub Actions CI/CD pipelines (build, test, release, docs)
- Professional English README with badges
- CONTRIBUTING.md, CODE_OF_CONDUCT.md
- .editorconfig for consistent code style
- GitHub issue templates (bug report, feature request)

### Changed

- Reorganized project structure with `src/` and `tests/` directories
- Renamed `MainFrm` to `MainForm` for consistency
- Fixed namespace from `PDF571` to `PdfMerger`
- Improved error handling with proper exception types
- Added `using` statements for disposable PDF document objects
- Sequential merge no longer adds unwanted watermark page

### Fixed

- "Decleration" typo corrected to "Declaration"
- Missing input validation in merge methods
- Resource disposal issues with PdfDocument objects

## [1.0.0] - 2021-12-30

### Added

- Initial release
- Sequential PDF merge
- ADF odd/even page merge for single-side scanners
- Thesis update feature (signature and declaration page insertion)
- Drag-and-drop file support
- WiX MSI installer
