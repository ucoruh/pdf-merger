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
    }
}
