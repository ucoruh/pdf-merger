# API Reference

## PdfMergeService

**Namespace:** `PdfMerger.Services`

Static class providing PDF merge, split, extract, and rotate operations.

---

### MergeSequential

```csharp
public static void MergeSequential(string outputFile, string[] files)
```

Merges multiple PDF files sequentially into a single output file. Pages from each input file are appended in order.

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `outputFile` | `string` | Path for the merged output PDF file |
| `files` | `string[]` | Array of input PDF file paths (minimum 2) |

**Exceptions:**

| Exception | Condition |
|-----------|-----------|
| `ArgumentException` | Fewer than 2 files provided |
| `FileNotFoundException` | An input file does not exist |

---

### MergeAdfScanning

```csharp
public static void MergeAdfScanning(string outputFile, string[] files)
```

Merges two PDF files by interleaving odd and even pages. Even pages are inserted in reverse order, matching the output of single-side ADF scanners.

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `outputFile` | `string` | Path for the merged output PDF file |
| `files` | `string[]` | Array of exactly 2 PDF file paths: [0]=odd, [1]=even |

**Exceptions:**

| Exception | Condition |
|-----------|-----------|
| `ArgumentException` | Not exactly 2 files, or page counts don't match |
| `FileNotFoundException` | An input file does not exist |

---

### ThesisUpdate

```csharp
public static void ThesisUpdate(string outputFile, string[] files)
```

Updates a thesis PDF by inserting signature and declaration pages at positions 3 and 4.

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `outputFile` | `string` | Path for the updated thesis output PDF file |
| `files` | `string[]` | Array of exactly 3 PDF paths: [0]=thesis, [1]=signatures, [2]=declaration |

**Exceptions:**

| Exception | Condition |
|-----------|-----------|
| `ArgumentException` | Not exactly 3 files provided |
| `FileNotFoundException` | An input file does not exist |

---

### SplitPdf

```csharp
public static string[] SplitPdf(string inputFile, string outputFolder, int[] splitAfterPages)
```

Splits a PDF file into multiple parts at specified page boundaries. For example, splitting a 10-page PDF at pages 3 and 7 produces three files: pages 1-3, 4-7, and 8-10.

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `inputFile` | `string` | Path to the input PDF file |
| `outputFolder` | `string` | Directory for the split output files |
| `splitAfterPages` | `int[]` | 1-based page numbers to split after |

**Returns:** `string[]` — Array of created output file paths.

**Exceptions:**

| Exception | Condition |
|-----------|-----------|
| `ArgumentException` | Null/empty file path, empty split points, or page numbers out of range |
| `FileNotFoundException` | Input file does not exist |

---

### ExtractPages

```csharp
public static void ExtractPages(string inputFile, string outputFile, int[] pageNumbers)
```

Extracts specified pages from a PDF into a new document. Page numbers are 1-based.

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `inputFile` | `string` | Path to the input PDF file |
| `outputFile` | `string` | Path for the extracted output PDF file |
| `pageNumbers` | `int[]` | 1-based page numbers to extract |

**Exceptions:**

| Exception | Condition |
|-----------|-----------|
| `ArgumentException` | Null/empty file path, empty page list, or page numbers out of range |
| `FileNotFoundException` | Input file does not exist |

---

### RotatePages

```csharp
public static void RotatePages(string inputFile, string outputFile, int angle)
```

Rotates all pages in a PDF by the specified angle.

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `inputFile` | `string` | Path to the input PDF file |
| `outputFile` | `string` | Path for the rotated output PDF file |
| `angle` | `int` | Rotation angle: 90, 180, or 270 degrees |

**Exceptions:**

| Exception | Condition |
|-----------|-----------|
| `ArgumentException` | Null/empty file path, or angle not 90/180/270 |
| `FileNotFoundException` | Input file does not exist |

---

### ParsePageRange

```csharp
public static int[] ParsePageRange(string rangeString, int maxPage)
```

Parses a page range string into an array of page numbers. Supports single pages and ranges (e.g., `"1-3,5,8-10"` returns `[1, 2, 3, 5, 8, 9, 10]`).

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `rangeString` | `string` | Comma-separated page numbers and ranges |
| `maxPage` | `int` | Maximum valid page number |

**Returns:** `int[]` — Array of parsed page numbers.

**Exceptions:**

| Exception | Condition |
|-----------|-----------|
| `ArgumentException` | Empty string, invalid format, or page numbers out of range |

---

## Doxygen Documentation (Auto-Generated)

The full auto-generated API documentation from source code XML comments is available online:

**[Browse Doxygen API Documentation](../doxygen/index.html){ target="_blank" }**

This includes:

- Complete class hierarchy
- All public methods with parameters and return types
- Source code browser
- XML documentation comments rendered as HTML

### Building Doxygen Locally

```bash
cd doxygen
doxygen Doxyfile
```

The generated HTML documentation will be in `doxygen/output/html/`. Open `index.html` to browse.
