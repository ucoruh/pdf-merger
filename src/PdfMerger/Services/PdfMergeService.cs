using System;
using System.IO;
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
