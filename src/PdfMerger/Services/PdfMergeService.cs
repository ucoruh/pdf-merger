using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfMerger.Services
{
    /// <summary>
    /// Provides PDF merging operations including sequential merge,
    /// ADF odd/even page interleaving, and thesis page insertion.
    /// </summary>
    public static class PdfMergeService
    {
        /// <summary>
        /// Merges multiple PDF files sequentially into a single output file.
        /// Pages from each input file are appended in order.
        /// </summary>
        /// <param name="outputFile">Path for the merged output PDF file.</param>
        /// <param name="files">Array of input PDF file paths to merge.</param>
        /// <exception cref="ArgumentException">Thrown when fewer than 2 files are provided.</exception>
        /// <exception cref="FileNotFoundException">Thrown when an input file does not exist.</exception>
        public static void MergeSequential(string outputFile, string[] files)
        {
            if (files == null || files.Length < 2)
            {
                throw new ArgumentException("At least 2 PDF files are required for sequential merge.", nameof(files));
            }

            ValidateFilesExist(files);

            using (PdfDocument outputDocument = new PdfDocument())
            {
                outputDocument.Info.Title = "Merged Document";
                outputDocument.Info.Creator = "PDF Merger";

                foreach (string file in files)
                {
                    using (PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import))
                    {
                        int count = inputDocument.PageCount;
                        for (int i = 0; i < count; i++)
                        {
                            outputDocument.AddPage(inputDocument.Pages[i]);
                        }
                    }
                }

                outputDocument.Save(outputFile);
            }
        }

        /// <summary>
        /// Merges two PDF files by interleaving odd and even pages.
        /// Designed for single-side ADF scanners where odd and even pages
        /// are scanned separately. Even pages are inserted in reverse order.
        /// </summary>
        /// <param name="outputFile">Path for the merged output PDF file.</param>
        /// <param name="files">Array of exactly 2 PDF file paths: [0]=odd pages, [1]=even pages.</param>
        /// <exception cref="ArgumentException">Thrown when exactly 2 files are not provided or page counts don't match.</exception>
        /// <exception cref="FileNotFoundException">Thrown when an input file does not exist.</exception>
        public static void MergeAdfScanning(string outputFile, string[] files)
        {
            if (files == null || files.Length != 2)
            {
                throw new ArgumentException("Exactly 2 PDF files are required for ADF merge (odd pages and even pages).", nameof(files));
            }

            ValidateFilesExist(files);

            using (PdfDocument outputDocument = new PdfDocument())
            using (PdfDocument oddDocument = PdfReader.Open(files[0], PdfDocumentOpenMode.Import))
            using (PdfDocument evenDocument = PdfReader.Open(files[1], PdfDocumentOpenMode.Import))
            {
                if (oddDocument.PageCount != evenDocument.PageCount)
                {
                    throw new ArgumentException(
                        $"Page count mismatch: odd pages file has {oddDocument.PageCount} pages, " +
                        $"even pages file has {evenDocument.PageCount} pages. Both must have equal page counts.");
                }

                outputDocument.Version = oddDocument.Version;
                outputDocument.Info.Title = "Merged Document";
                outputDocument.Info.Creator = "PDF Merger";

                int pageCount = oddDocument.PageCount;

                for (int i = 0; i < pageCount; i++)
                {
                    outputDocument.AddPage(oddDocument.Pages[i]);
                    outputDocument.AddPage(evenDocument.Pages[pageCount - i - 1]);
                }

                outputDocument.Save(outputFile);
            }
        }

        /// <summary>
        /// Updates a thesis PDF by inserting signature and declaration pages
        /// at specific positions (pages 3 and 4 in the output).
        /// </summary>
        /// <param name="outputFile">Path for the updated thesis output PDF file.</param>
        /// <param name="files">Array of exactly 3 PDF file paths: [0]=thesis, [1]=signatures, [2]=declaration.</param>
        /// <exception cref="ArgumentException">Thrown when exactly 3 files are not provided.</exception>
        /// <exception cref="FileNotFoundException">Thrown when an input file does not exist.</exception>
        public static void ThesisUpdate(string outputFile, string[] files)
        {
            if (files == null || files.Length != 3)
            {
                throw new ArgumentException(
                    "Exactly 3 PDF files are required for thesis update: thesis, signatures, and declaration.",
                    nameof(files));
            }

            ValidateFilesExist(files);

            using (PdfDocument outputDocument = new PdfDocument())
            using (PdfDocument thesisDocument = PdfReader.Open(files[0], PdfDocumentOpenMode.Import))
            using (PdfDocument signaturesDocument = PdfReader.Open(files[1], PdfDocumentOpenMode.Import))
            using (PdfDocument declarationDocument = PdfReader.Open(files[2], PdfDocumentOpenMode.Import))
            {
                outputDocument.Version = thesisDocument.Version;
                outputDocument.Info.Title = "Merged Document";
                outputDocument.Info.Creator = "PDF Merger";

                int pageCount = thesisDocument.PageCount;

                for (int i = 0; i < pageCount; i++)
                {
                    if (i == 2)
                    {
                        outputDocument.AddPage(signaturesDocument.Pages[0]);
                    }
                    else if (i == 3)
                    {
                        outputDocument.AddPage(declarationDocument.Pages[0]);
                    }
                    else
                    {
                        outputDocument.AddPage(thesisDocument.Pages[i]);
                    }
                }

                outputDocument.Save(outputFile);
            }
        }

        /// <summary>
        /// Splits a PDF file into multiple parts at the specified page boundaries.
        /// </summary>
        /// <param name="inputFile">Path to the input PDF file.</param>
        /// <param name="outputFolder">Folder where split files will be saved.</param>
        /// <param name="splitAfterPages">1-based page numbers after which to split (e.g., [3,7] splits a 10-page PDF into pages 1-3, 4-7, 8-10).</param>
        /// <returns>Array of created output file paths.</returns>
        /// <exception cref="ArgumentException">Thrown when input parameters are invalid.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the input file does not exist.</exception>
        public static string[] SplitPdf(string inputFile, string outputFolder, int[] splitAfterPages)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new ArgumentException("Input file path is required.", nameof(inputFile));
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"PDF file not found: {inputFile}", inputFile);
            if (splitAfterPages == null || splitAfterPages.Length == 0)
                throw new ArgumentException("At least one split point is required.", nameof(splitAfterPages));
            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentException("Output folder path is required.", nameof(outputFolder));

            using (PdfDocument inputDocument = PdfReader.Open(inputFile, PdfDocumentOpenMode.Import))
            {
                int pageCount = inputDocument.PageCount;

                int[] sorted = splitAfterPages.Distinct().OrderBy(p => p).ToArray();
                foreach (int p in sorted)
                {
                    if (p < 1 || p >= pageCount)
                        throw new ArgumentException(
                            $"Split point {p} is out of range. Must be between 1 and {pageCount - 1}.",
                            nameof(splitAfterPages));
                }

                Directory.CreateDirectory(outputFolder);

                List<string> outputFiles = new List<string>();
                int startPage = 0;

                for (int partIndex = 0; partIndex <= sorted.Length; partIndex++)
                {
                    int endPage = (partIndex < sorted.Length) ? sorted[partIndex] : pageCount;

                    using (PdfDocument partDoc = new PdfDocument())
                    {
                        partDoc.Info.Creator = "PDF Merger";

                        for (int i = startPage; i < endPage; i++)
                        {
                            partDoc.AddPage(inputDocument.Pages[i]);
                        }

                        string outputPath = Path.Combine(outputFolder, $"split_part{partIndex + 1}.pdf");
                        partDoc.Save(outputPath);
                        outputFiles.Add(outputPath);
                    }

                    startPage = endPage;
                }

                return outputFiles.ToArray();
            }
        }

        /// <summary>
        /// Extracts specific pages from a PDF file into a new PDF.
        /// </summary>
        /// <param name="inputFile">Path to the input PDF file.</param>
        /// <param name="outputFile">Path for the output PDF file.</param>
        /// <param name="pageNumbers">1-based page numbers to extract.</param>
        /// <exception cref="ArgumentException">Thrown when input parameters are invalid.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the input file does not exist.</exception>
        public static void ExtractPages(string inputFile, string outputFile, int[] pageNumbers)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new ArgumentException("Input file path is required.", nameof(inputFile));
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"PDF file not found: {inputFile}", inputFile);
            if (pageNumbers == null || pageNumbers.Length == 0)
                throw new ArgumentException("At least one page number is required.", nameof(pageNumbers));

            using (PdfDocument inputDocument = PdfReader.Open(inputFile, PdfDocumentOpenMode.Import))
            {
                int pageCount = inputDocument.PageCount;

                foreach (int p in pageNumbers)
                {
                    if (p < 1 || p > pageCount)
                        throw new ArgumentException(
                            $"Page number {p} is out of range. Must be between 1 and {pageCount}.",
                            nameof(pageNumbers));
                }

                using (PdfDocument outputDocument = new PdfDocument())
                {
                    outputDocument.Info.Creator = "PDF Merger";

                    foreach (int p in pageNumbers)
                    {
                        outputDocument.AddPage(inputDocument.Pages[p - 1]);
                    }

                    outputDocument.Save(outputFile);
                }
            }
        }

        /// <summary>
        /// Rotates all pages in a PDF file by the specified angle.
        /// </summary>
        /// <param name="inputFile">Path to the input PDF file.</param>
        /// <param name="outputFile">Path for the output PDF file.</param>
        /// <param name="angle">Rotation angle: 90, 180, or 270 degrees.</param>
        /// <exception cref="ArgumentException">Thrown when the angle is not 90, 180, or 270.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the input file does not exist.</exception>
        public static void RotatePages(string inputFile, string outputFile, int angle)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new ArgumentException("Input file path is required.", nameof(inputFile));
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"PDF file not found: {inputFile}", inputFile);
            if (angle != 90 && angle != 180 && angle != 270)
                throw new ArgumentException("Angle must be 90, 180, or 270 degrees.", nameof(angle));

            using (PdfDocument inputDocument = PdfReader.Open(inputFile, PdfDocumentOpenMode.Import))
            using (PdfDocument outputDocument = new PdfDocument())
            {
                outputDocument.Info.Creator = "PDF Merger";

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    PdfPage page = outputDocument.AddPage(inputDocument.Pages[i]);
                    page.Rotate = (page.Rotate + angle) % 360;
                }

                outputDocument.Save(outputFile);
            }
        }

        /// <summary>
        /// Parses a page range string like "1-3,5,8-10" into an array of individual page numbers.
        /// </summary>
        /// <param name="rangeString">Comma-separated page ranges (e.g., "1-3,5,8-10").</param>
        /// <param name="maxPage">Maximum valid page number.</param>
        /// <returns>Array of 1-based page numbers in the order specified.</returns>
        /// <exception cref="ArgumentException">Thrown when the range string is invalid.</exception>
        public static int[] ParsePageRange(string rangeString, int maxPage)
        {
            if (string.IsNullOrWhiteSpace(rangeString))
                throw new ArgumentException("Page range string is required.", nameof(rangeString));

            List<int> pages = new List<int>();
            string[] parts = rangeString.Split(',');

            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                if (trimmed.Contains("-"))
                {
                    string[] rangeParts = trimmed.Split('-');
                    if (rangeParts.Length != 2)
                        throw new ArgumentException($"Invalid range format: '{trimmed}'. Use format like '1-3'.");

                    int start, end;
                    if (!int.TryParse(rangeParts[0].Trim(), out start) || !int.TryParse(rangeParts[1].Trim(), out end))
                        throw new ArgumentException($"Invalid range format: '{trimmed}'. Page numbers must be integers.");

                    if (start > end)
                        throw new ArgumentException($"Invalid range: '{trimmed}'. Start page must be less than or equal to end page.");

                    if (start < 1 || end > maxPage)
                        throw new ArgumentException($"Page range '{trimmed}' is out of bounds. Valid range is 1-{maxPage}.");

                    for (int i = start; i <= end; i++)
                    {
                        pages.Add(i);
                    }
                }
                else
                {
                    int page;
                    if (!int.TryParse(trimmed, out page))
                        throw new ArgumentException($"Invalid page number: '{trimmed}'.");

                    if (page < 1 || page > maxPage)
                        throw new ArgumentException($"Page number {page} is out of bounds. Valid range is 1-{maxPage}.");

                    pages.Add(page);
                }
            }

            if (pages.Count == 0)
                throw new ArgumentException("No valid page numbers found in the range string.");

            return pages.ToArray();
        }

        /// <summary>
        /// Validates that all specified files exist on disk.
        /// </summary>
        /// <param name="files">Array of file paths to validate.</param>
        /// <exception cref="FileNotFoundException">Thrown when a file does not exist.</exception>
        private static void ValidateFilesExist(string[] files)
        {
            foreach (string file in files)
            {
                if (!File.Exists(file))
                {
                    throw new FileNotFoundException($"PDF file not found: {file}", file);
                }
            }
        }
    }
}
