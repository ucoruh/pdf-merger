# API Reference

## PdfMergeService

**Namespace:** `PdfMerger.Services`

Static class providing PDF merging operations.

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

## Doxygen Documentation

For detailed auto-generated API documentation from source code XML comments, build the Doxygen docs:

```bash
cd doxygen
doxygen Doxyfile
```

The generated HTML documentation will be in `doxygen/output/html/`.
