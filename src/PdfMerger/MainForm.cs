using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PdfMerger.Services;

namespace PdfMerger
{
    /// <summary>
    /// Main application form for the PDF Merger utility.
    /// Provides UI for adding, reordering, and merging PDF files.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedIndex >= 1)
            {
                int index = listFiles.SelectedIndex;
                string s = (string)listFiles.SelectedItem;
                listFiles.Items.RemoveAt(index);
                listFiles.Items.Insert(index - 1, s);
                listFiles.SelectedIndex = index - 1;
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedIndex != -1 && listFiles.SelectedIndex != listFiles.Items.Count - 1)
            {
                int index = listFiles.SelectedIndex;
                string s = (string)listFiles.SelectedItem;
                listFiles.Items.RemoveAt(index);
                listFiles.Items.Insert(index + 1, s);
                listFiles.SelectedIndex = index + 1;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedIndex != -1)
            {
                int index = listFiles.SelectedIndex;
                listFiles.Items.RemoveAt(index);
                if (index == listFiles.Items.Count)
                {
                    index--;
                }
                if (index >= 0)
                {
                    listFiles.SelectedIndex = index;
                }
            }

            UpdateButtonStates();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = true;
                dialog.Filter = "PDF Documents|*.pdf";
                DialogResult result = dialog.ShowDialog(this);

                if (result != DialogResult.Cancel && dialog.FileNames.Length > 0)
                {
                    listFiles.Items.AddRange(dialog.FileNames);
                    UpdateButtonStates();
                }
            }
        }

        private void listFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void listFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            bool allPDFs = true;
            foreach (string file in files)
            {
                if (!file.ToLowerInvariant().EndsWith(".pdf"))
                {
                    allPDFs = false;
                    break;
                }
            }

            if (allPDFs && files.Length > 0)
            {
                listFiles.Items.AddRange(files);
                UpdateButtonStates();
            }
            else
            {
                MessageBox.Show(
                    "You can only add PDF files to the list. Please ensure all selected files have the .pdf extension.",
                    "Invalid File Type",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            ExecuteMergeOperation("Sequential Merge", (outputFile, files) =>
                PdfMergeService.MergeSequential(outputFile, files));
        }

        private void btnMergeADF_Click(object sender, EventArgs e)
        {
            ExecuteMergeOperation("ADF Merge", (outputFile, files) =>
                PdfMergeService.MergeAdfScanning(outputFile, files));
        }

        private void btnUpdateThesis_Click(object sender, EventArgs e)
        {
            ExecuteMergeOperation("Thesis Update", (outputFile, files) =>
                PdfMergeService.ThesisUpdate(outputFile, files));
        }

        /// <summary>
        /// Common method for executing merge operations with save dialog and error handling.
        /// </summary>
        private void ExecuteMergeOperation(string operationName, Action<string, string[]> mergeAction)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "PDF Documents|*.pdf";
                dialog.AddExtension = true;
                dialog.DefaultExt = "pdf";
                DialogResult result = dialog.ShowDialog(this);

                if (result != DialogResult.Cancel)
                {
                    string[] files = new string[listFiles.Items.Count];
                    listFiles.Items.CopyTo(files, 0);

                    try
                    {
                        mergeAction(dialog.FileName, files);
                        Process.Start(dialog.FileName);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            operationName + " - Invalid Input",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show(
                            "Error writing output document. You may not have write permission." +
                            Environment.NewLine + "Error: " + ex.Message,
                            operationName + " - Access Denied",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(
                            "Error writing output document. The file may be open in another application." +
                            Environment.NewLine + "Error: " + ex.Message,
                            operationName + " - File Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            if (listFiles.Items.Count < 1) return;

            string inputFile = (string)listFiles.Items[0];

            using (var inputDialog = new InputDialogForm(
                "Split PDF",
                "Enter page numbers to split after (comma-separated, e.g., 3,7,12):",
                ""))
            {
                if (inputDialog.ShowDialog(this) != DialogResult.OK) return;

                string input = inputDialog.InputText.Trim();
                if (string.IsNullOrEmpty(input)) return;

                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select output folder for split PDF files";
                    if (folderDialog.ShowDialog(this) != DialogResult.OK) return;

                    try
                    {
                        int[] splitPoints = input.Split(',')
                            .Select(s => int.Parse(s.Trim()))
                            .ToArray();

                        string[] outputFiles = PdfMergeService.SplitPdf(inputFile, folderDialog.SelectedPath, splitPoints);

                        MessageBox.Show(
                            $"PDF split into {outputFiles.Length} files successfully.\nOutput folder: {folderDialog.SelectedPath}",
                            "Split PDF - Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        Process.Start(folderDialog.SelectedPath);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show(
                            "Invalid page numbers. Please enter comma-separated integers (e.g., 3,7,12).",
                            "Split PDF - Invalid Input",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(ex.Message, "Split PDF - Invalid Input",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(
                            "Error writing output files." + Environment.NewLine + "Error: " + ex.Message,
                            "Split PDF - File Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            if (listFiles.Items.Count < 1) return;

            string inputFile = (string)listFiles.Items[0];

            using (var inputDialog = new InputDialogForm(
                "Extract Pages",
                "Enter page range to extract (e.g., 1-3,5,8-10):",
                ""))
            {
                if (inputDialog.ShowDialog(this) != DialogResult.OK) return;

                string input = inputDialog.InputText.Trim();
                if (string.IsNullOrEmpty(input)) return;

                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "PDF Documents|*.pdf";
                    saveDialog.AddExtension = true;
                    saveDialog.DefaultExt = "pdf";
                    if (saveDialog.ShowDialog(this) != DialogResult.OK) return;

                    try
                    {
                        int pageCount;
                        using (var doc = PdfSharp.Pdf.IO.PdfReader.Open(inputFile, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import))
                        {
                            pageCount = doc.PageCount;
                        }

                        int[] pages = PdfMergeService.ParsePageRange(input, pageCount);
                        PdfMergeService.ExtractPages(inputFile, saveDialog.FileName, pages);
                        Process.Start(saveDialog.FileName);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(ex.Message, "Extract Pages - Invalid Input",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(
                            "Error writing output file." + Environment.NewLine + "Error: " + ex.Message,
                            "Extract Pages - File Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            if (listFiles.Items.Count < 1) return;

            using (var rotationDialog = new RotationDialogForm())
            {
                if (rotationDialog.ShowDialog(this) != DialogResult.OK) return;

                int angle = rotationDialog.SelectedAngle;

                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "PDF Documents|*.pdf";
                    saveDialog.AddExtension = true;
                    saveDialog.DefaultExt = "pdf";
                    if (saveDialog.ShowDialog(this) != DialogResult.OK) return;

                    try
                    {
                        string inputFile = (string)listFiles.Items[0];
                        PdfMergeService.RotatePages(inputFile, saveDialog.FileName, angle);
                        Process.Start(saveDialog.FileName);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(ex.Message, "Rotate Pages - Invalid Input",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(
                            "Error writing output file." + Environment.NewLine + "Error: " + ex.Message,
                            "Rotate Pages - File Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the enabled state of all buttons based on the current file list count.
        /// </summary>
        private void UpdateButtonStates()
        {
            int count = listFiles.Items.Count;
            bool hasFiles = count > 0;

            btnMoveUp.Enabled = hasFiles;
            btnMoveDown.Enabled = hasFiles;
            btnRemove.Enabled = hasFiles;
            btnMergeAll.Enabled = count > 1;
            btnMergeADF.Enabled = count == 2;
            btnUpdateThesis.Enabled = count == 3;
            btnSplit.Enabled = hasFiles;
            btnExtract.Enabled = hasFiles;
            btnRotate.Enabled = hasFiles;
        }
    }
}
