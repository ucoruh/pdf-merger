# User Guide

## Adding Files

### Drag and Drop

Drag PDF files from Windows Explorer directly into the white list area of the application.

!!! note
    Only `.pdf` files are accepted. Non-PDF files will be rejected with an error message.

### Add Button

Click the **Add** button to open a file browser dialog. You can select multiple files at once.

![Adding Files](assets/screenshots/2021-12-30-23-05-18-image.png)

## Managing the File List

- **Move Up** - Move the selected file up in the list order
- **Move Down** - Move the selected file down in the list order
- **Remove** - Remove the selected file from the list

## Merge Modes

### Sequential Merge

Combines all listed PDF files into a single document in the displayed order.

**Requirements:** At least 2 PDF files

**How it works:**

1. Add 2 or more PDF files
2. Arrange them in the desired order
3. Click **Merge Sequential**
4. Choose the save location

The output file will contain all pages from all input files, appended sequentially.

### ADF Odd/Even Merge

Designed for single-side ADF scanners. When you scan a double-sided document with a single-side scanner:

1. First, scan all pages face-up (odd pages: 1, 3, 5, 7...)
2. Flip the stack and scan again (even pages in reverse: 8, 6, 4, 2...)

This mode interleaves the pages to reconstruct the correct page order.

**Requirements:** Exactly 2 PDF files with equal page counts

**How it works:**

1. Add the odd-pages PDF as the first file
2. Add the even-pages PDF as the second file
3. Click **Merge ADF Odd/Even Mix**
4. Choose the save location

![ADF Merge](assets/screenshots/2021-12-30-23-03-38-image.png)

### Thesis Update

Inserts signed signature and declaration pages into your thesis PDF at specific positions (replacing pages 3 and 4).

**Requirements:** Exactly 3 PDF files

**How it works:**

1. Add files in this exact order:
    - **File 1:** Main thesis document
    - **File 2:** Signed signatures page
    - **File 3:** Signed declaration page
2. Click **Thesis Update**
3. Choose the save location

The output will be your thesis with the signature page at position 3 and the declaration page at position 4.
