using System;
using System.IO;
using NUnit.Framework;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfMerger.Services;

namespace PdfMerger.Tests.Services
{
    [TestFixture]
    public class PdfMergeServiceTests
    {
        private string _testDir;

        [SetUp]
        public void SetUp()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "PdfMergerTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }

        #region Helper Methods

        private string CreateTestPdf(int pageCount, string namePrefix = "test")
        {
            string filePath = Path.Combine(_testDir, $"{namePrefix}_{pageCount}pages.pdf");
            using (PdfDocument doc = new PdfDocument())
            {
                for (int i = 0; i < pageCount; i++)
                {
                    doc.AddPage();
                }
                doc.Save(filePath);
            }
            return filePath;
        }

        private string GetOutputPath(string name = "output.pdf")
        {
            return Path.Combine(_testDir, name);
        }

        private int GetPageCount(string filePath)
        {
            using (PdfDocument doc = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
            {
                return doc.PageCount;
            }
        }

        #endregion

        #region MergeSequential Tests

        [Test]
        public void MergeSequential_TwoFiles_MergesAllPages()
        {
            string file1 = CreateTestPdf(3, "doc1");
            string file2 = CreateTestPdf(2, "doc2");
            string output = GetOutputPath("sequential.pdf");

            PdfMergeService.MergeSequential(output, new[] { file1, file2 });

            Assert.That(File.Exists(output), Is.True);
            Assert.That(GetPageCount(output), Is.EqualTo(5));
        }

        [Test]
        public void MergeSequential_ThreeFiles_MergesAllPages()
        {
            string file1 = CreateTestPdf(2, "a");
            string file2 = CreateTestPdf(3, "b");
            string file3 = CreateTestPdf(1, "c");
            string output = GetOutputPath("sequential3.pdf");

            PdfMergeService.MergeSequential(output, new[] { file1, file2, file3 });

            Assert.That(File.Exists(output), Is.True);
            Assert.That(GetPageCount(output), Is.EqualTo(6));
        }

        [Test]
        public void MergeSequential_NullFiles_ThrowsArgumentException()
        {
            string output = GetOutputPath();
            Assert.Throws<ArgumentException>(() => PdfMergeService.MergeSequential(output, null));
        }

        [Test]
        public void MergeSequential_SingleFile_ThrowsArgumentException()
        {
            string file1 = CreateTestPdf(3);
            string output = GetOutputPath();
            Assert.Throws<ArgumentException>(() => PdfMergeService.MergeSequential(output, new[] { file1 }));
        }

        [Test]
        public void MergeSequential_NonExistentFile_ThrowsFileNotFoundException()
        {
            string file1 = CreateTestPdf(3);
            string fakeFile = Path.Combine(_testDir, "nonexistent.pdf");
            string output = GetOutputPath();

            Assert.Throws<FileNotFoundException>(() =>
                PdfMergeService.MergeSequential(output, new[] { file1, fakeFile }));
        }

        #endregion

        #region MergeAdfScanning Tests

        [Test]
        public void MergeAdfScanning_EqualPageCounts_InterleavesPages()
        {
            string oddFile = CreateTestPdf(3, "odd");
            string evenFile = CreateTestPdf(3, "even");
            string output = GetOutputPath("adf.pdf");

            PdfMergeService.MergeAdfScanning(output, new[] { oddFile, evenFile });

            Assert.That(File.Exists(output), Is.True);
            Assert.That(GetPageCount(output), Is.EqualTo(6));
        }

        [Test]
        public void MergeAdfScanning_UnequalPageCounts_ThrowsArgumentException()
        {
            string oddFile = CreateTestPdf(3, "odd");
            string evenFile = CreateTestPdf(2, "even");
            string output = GetOutputPath("adf_fail.pdf");

            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.MergeAdfScanning(output, new[] { oddFile, evenFile }));
        }

        [Test]
        public void MergeAdfScanning_WrongFileCount_ThrowsArgumentException()
        {
            string file1 = CreateTestPdf(3);
            string output = GetOutputPath();

            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.MergeAdfScanning(output, new[] { file1 }));
        }

        [Test]
        public void MergeAdfScanning_ThreeFiles_ThrowsArgumentException()
        {
            string file1 = CreateTestPdf(3, "a");
            string file2 = CreateTestPdf(3, "b");
            string file3 = CreateTestPdf(3, "c");
            string output = GetOutputPath();

            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.MergeAdfScanning(output, new[] { file1, file2, file3 }));
        }

        [Test]
        public void MergeAdfScanning_NullFiles_ThrowsArgumentException()
        {
            string output = GetOutputPath();
            Assert.Throws<ArgumentException>(() => PdfMergeService.MergeAdfScanning(output, null));
        }

        #endregion

        #region ThesisUpdate Tests

        [Test]
        public void ThesisUpdate_ValidInputs_InsertsSignatureAndDeclaration()
        {
            string thesis = CreateTestPdf(10, "thesis");
            string signatures = CreateTestPdf(1, "sig");
            string declaration = CreateTestPdf(1, "decl");
            string output = GetOutputPath("thesis_updated.pdf");

            PdfMergeService.ThesisUpdate(output, new[] { thesis, signatures, declaration });

            Assert.That(File.Exists(output), Is.True);
            Assert.That(GetPageCount(output), Is.EqualTo(10));
        }

        [Test]
        public void ThesisUpdate_WrongFileCount_ThrowsArgumentException()
        {
            string file1 = CreateTestPdf(5);
            string file2 = CreateTestPdf(1);
            string output = GetOutputPath();

            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.ThesisUpdate(output, new[] { file1, file2 }));
        }

        [Test]
        public void ThesisUpdate_NullFiles_ThrowsArgumentException()
        {
            string output = GetOutputPath();
            Assert.Throws<ArgumentException>(() => PdfMergeService.ThesisUpdate(output, null));
        }

        [Test]
        public void ThesisUpdate_FourFiles_ThrowsArgumentException()
        {
            string file1 = CreateTestPdf(5, "a");
            string file2 = CreateTestPdf(1, "b");
            string file3 = CreateTestPdf(1, "c");
            string file4 = CreateTestPdf(1, "d");
            string output = GetOutputPath();

            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.ThesisUpdate(output, new[] { file1, file2, file3, file4 }));
        }

        #endregion

        #region SplitPdf Tests

        [Test]
        public void SplitPdf_ValidInput_CreatesCorrectFiles()
        {
            string input = CreateTestPdf(10, "split_input");
            string outputFolder = Path.Combine(_testDir, "split_output");

            string[] result = PdfMergeService.SplitPdf(input, outputFolder, new[] { 3, 7 });

            Assert.That(result.Length, Is.EqualTo(3));
            Assert.That(GetPageCount(result[0]), Is.EqualTo(3));
            Assert.That(GetPageCount(result[1]), Is.EqualTo(4));
            Assert.That(GetPageCount(result[2]), Is.EqualTo(3));
        }

        [Test]
        public void SplitPdf_SingleSplitPoint_CreatesTwoParts()
        {
            string input = CreateTestPdf(6, "split_single");
            string outputFolder = Path.Combine(_testDir, "split_single_output");

            string[] result = PdfMergeService.SplitPdf(input, outputFolder, new[] { 4 });

            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(GetPageCount(result[0]), Is.EqualTo(4));
            Assert.That(GetPageCount(result[1]), Is.EqualTo(2));
        }

        [Test]
        public void SplitPdf_NullFile_ThrowsArgumentException()
        {
            string outputFolder = Path.Combine(_testDir, "split_null");
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.SplitPdf(null, outputFolder, new[] { 3 }));
        }

        [Test]
        public void SplitPdf_EmptySplitPoints_ThrowsArgumentException()
        {
            string input = CreateTestPdf(5, "split_empty");
            string outputFolder = Path.Combine(_testDir, "split_empty_output");
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.SplitPdf(input, outputFolder, new int[0]));
        }

        [Test]
        public void SplitPdf_PageNumberOutOfRange_ThrowsArgumentException()
        {
            string input = CreateTestPdf(5, "split_range");
            string outputFolder = Path.Combine(_testDir, "split_range_output");
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.SplitPdf(input, outputFolder, new[] { 10 }));
        }

        [Test]
        public void SplitPdf_NonExistentFile_ThrowsFileNotFoundException()
        {
            string fakeFile = Path.Combine(_testDir, "nonexistent.pdf");
            string outputFolder = Path.Combine(_testDir, "split_fake_output");
            Assert.Throws<FileNotFoundException>(() =>
                PdfMergeService.SplitPdf(fakeFile, outputFolder, new[] { 3 }));
        }

        #endregion

        #region ExtractPages Tests

        [Test]
        public void ExtractPages_ValidRange_ExtractsCorrectPages()
        {
            string input = CreateTestPdf(10, "extract_input");
            string output = GetOutputPath("extracted.pdf");

            PdfMergeService.ExtractPages(input, output, new[] { 1, 2, 3, 5, 8 });

            Assert.That(File.Exists(output), Is.True);
            Assert.That(GetPageCount(output), Is.EqualTo(5));
        }

        [Test]
        public void ExtractPages_SinglePage_ExtractsOnePage()
        {
            string input = CreateTestPdf(5, "extract_single");
            string output = GetOutputPath("extracted_single.pdf");

            PdfMergeService.ExtractPages(input, output, new[] { 3 });

            Assert.That(File.Exists(output), Is.True);
            Assert.That(GetPageCount(output), Is.EqualTo(1));
        }

        [Test]
        public void ExtractPages_AllPages_ExtractsAll()
        {
            string input = CreateTestPdf(4, "extract_all");
            string output = GetOutputPath("extracted_all.pdf");

            PdfMergeService.ExtractPages(input, output, new[] { 1, 2, 3, 4 });

            Assert.That(File.Exists(output), Is.True);
            Assert.That(GetPageCount(output), Is.EqualTo(4));
        }

        [Test]
        public void ExtractPages_NullFile_ThrowsArgumentException()
        {
            string output = GetOutputPath();
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.ExtractPages(null, output, new[] { 1 }));
        }

        [Test]
        public void ExtractPages_PageOutOfRange_ThrowsArgumentException()
        {
            string input = CreateTestPdf(5, "extract_range");
            string output = GetOutputPath("extracted_range.pdf");
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.ExtractPages(input, output, new[] { 6 }));
        }

        [Test]
        public void ExtractPages_EmptyPageList_ThrowsArgumentException()
        {
            string input = CreateTestPdf(5, "extract_empty");
            string output = GetOutputPath("extracted_empty.pdf");
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.ExtractPages(input, output, new int[0]));
        }

        #endregion

        #region RotatePages Tests

        [Test]
        public void RotatePages_90Degrees_RotatesAllPages()
        {
            string input = CreateTestPdf(3, "rotate_90");
            string output = GetOutputPath("rotated_90.pdf");

            PdfMergeService.RotatePages(input, output, 90);

            Assert.That(File.Exists(output), Is.True);
            Assert.That(GetPageCount(output), Is.EqualTo(3));

            using (PdfDocument doc = PdfReader.Open(output, PdfDocumentOpenMode.Import))
            {
                for (int i = 0; i < doc.PageCount; i++)
                {
                    Assert.That(doc.Pages[i].Rotate, Is.EqualTo(90));
                }
            }
        }

        [Test]
        public void RotatePages_180Degrees_RotatesAllPages()
        {
            string input = CreateTestPdf(2, "rotate_180");
            string output = GetOutputPath("rotated_180.pdf");

            PdfMergeService.RotatePages(input, output, 180);

            Assert.That(File.Exists(output), Is.True);
            using (PdfDocument doc = PdfReader.Open(output, PdfDocumentOpenMode.Import))
            {
                for (int i = 0; i < doc.PageCount; i++)
                {
                    Assert.That(doc.Pages[i].Rotate, Is.EqualTo(180));
                }
            }
        }

        [Test]
        public void RotatePages_270Degrees_RotatesAllPages()
        {
            string input = CreateTestPdf(2, "rotate_270");
            string output = GetOutputPath("rotated_270.pdf");

            PdfMergeService.RotatePages(input, output, 270);

            Assert.That(File.Exists(output), Is.True);
            using (PdfDocument doc = PdfReader.Open(output, PdfDocumentOpenMode.Import))
            {
                for (int i = 0; i < doc.PageCount; i++)
                {
                    Assert.That(doc.Pages[i].Rotate, Is.EqualTo(270));
                }
            }
        }

        [Test]
        public void RotatePages_InvalidAngle_ThrowsArgumentException()
        {
            string input = CreateTestPdf(2, "rotate_invalid");
            string output = GetOutputPath("rotated_invalid.pdf");
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.RotatePages(input, output, 45));
        }

        [Test]
        public void RotatePages_NullFile_ThrowsArgumentException()
        {
            string output = GetOutputPath();
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.RotatePages(null, output, 90));
        }

        #endregion

        #region ParsePageRange Tests

        [Test]
        public void ParsePageRange_SimpleRange_ReturnsCorrectPages()
        {
            int[] result = PdfMergeService.ParsePageRange("1-3", 10);
            Assert.That(result, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void ParsePageRange_MixedRangeAndSingle_ReturnsCorrectPages()
        {
            int[] result = PdfMergeService.ParsePageRange("1-3,5,8-10", 10);
            Assert.That(result, Is.EqualTo(new[] { 1, 2, 3, 5, 8, 9, 10 }));
        }

        [Test]
        public void ParsePageRange_SinglePage_ReturnsOnePage()
        {
            int[] result = PdfMergeService.ParsePageRange("5", 10);
            Assert.That(result, Is.EqualTo(new[] { 5 }));
        }

        [Test]
        public void ParsePageRange_OutOfRange_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.ParsePageRange("1-15", 10));
        }

        [Test]
        public void ParsePageRange_EmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.ParsePageRange("", 10));
        }

        [Test]
        public void ParsePageRange_InvalidFormat_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                PdfMergeService.ParsePageRange("abc", 10));
        }

        #endregion
    }
}
