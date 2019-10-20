using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PDF571
{
    public partial class MainFrm : Form
    {
        public MainFrm()
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
            if (listFiles.Items.Count == 0)
            {
                btnMoveUp.Enabled = false;
                btnMoveDown.Enabled = false;
                btnRemove.Enabled = false;
            }
            if (listFiles.Items.Count <= 1)
            {
                btnMergeAll.Enabled = false;
            }

            if (listFiles.Items.Count !=2)
            {
                btnMergeADF.Enabled = false;
            }

            if (listFiles.Items.Count != 3)
            {
                btnUpdateThesis.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Multiselect = true;
            d.Filter = "PDF Documents|*.pdf";
            DialogResult dr = d.ShowDialog(this);

            if (dr != DialogResult.Cancel && d.FileNames.Length > 0)
            {
                listFiles.Items.AddRange(d.FileNames);

                btnMoveUp.Enabled = true;
                btnMoveDown.Enabled = true;
                btnRemove.Enabled = true;

                if (listFiles.Items.Count > 1)
                {
                    btnMergeAll.Enabled = true;
                }

                if (listFiles.Items.Count == 2)
                {
                    btnMergeADF.Enabled = true;
                }

                if (listFiles.Items.Count == 3)
                {
                    btnUpdateThesis.Enabled = true;
                }
            }
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "PDF Documents|*.pdf";
            s.AddExtension = true;
            s.DefaultExt = "pdf";
            DialogResult dr = s.ShowDialog(this);

            if (dr != DialogResult.Cancel)
            {
                // Merge the PDFs
                string[] files = new string[listFiles.Items.Count];
                listFiles.Items.CopyTo(files, 0);

                try
                {
                    // Merge the document...
                    MergeSequentialScanning(s.FileName, files);
                    //MergeDocuments(s.FileName, files);
                    // ... and start a viewer
                    Process.Start(s.FileName);
                }
                catch (UnauthorizedAccessException uaexc)
                {
                    MessageBox.Show("Error writing output document, perhaps you do not have write permission?" + Environment.NewLine +
                                    "Error message: " + uaexc.Message,
                        "Failed to write document",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                catch (System.IO.IOException ioexc)
                {
                    MessageBox.Show("Error writing output document, are you still viewing it in your PDF reader?." + Environment.NewLine +
                                    "Error message: " + ioexc.Message,
                        "Failed to write document",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void listFiles_DragEnter(object sender, DragEventArgs e)
        {
            // Only accept dropped files, not text etc
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void listFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            bool allPDFs = true;
            // Make sure only PDFs are dropped
            foreach (string s in files)
            {
                if (!s.ToLower().EndsWith(".pdf"))
                {
                    allPDFs = false;
                }
            }

            if (allPDFs && files.Length > 0)
            {
                listFiles.Items.AddRange(files);

                btnMoveUp.Enabled = true;
                btnMoveDown.Enabled = true;
                btnRemove.Enabled = true;

                if (listFiles.Items.Count > 1)
                {
                    btnMergeAll.Enabled = true;
                }

                if (listFiles.Items.Count == 2)
                {
                    btnMergeADF.Enabled = true;
                }

                if (listFiles.Items.Count == 3)
                {
                    btnUpdateThesis.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("You can only add PDFs to the list! Please ensure all selected files are of type .PDF", "File error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Imports all pages from a list of documents.
        /// </summary>
        public void ThesisUpdate(string outputFile, string[] files)
        {
            if (files.Length == 0) { return; }

            PdfDocument outputMergedDocument = new PdfDocument();

            PdfDocument inputThesis = PdfReader.Open(files[0], PdfDocumentOpenMode.Import);
            PdfDocument inputSignatures = PdfReader.Open(files[1], PdfDocumentOpenMode.Import);
            PdfDocument inputDeclaration = PdfReader.Open(files[2], PdfDocumentOpenMode.Import);


            outputMergedDocument.Version = inputThesis.Version;
            outputMergedDocument.Info.Title = "merged document";
            outputMergedDocument.Info.Creator = "application";

            int pageCount = inputThesis.PageCount;

            for (int idx = 0; idx < pageCount; idx++)
            {

                if (idx == 2)
                {
                    outputMergedDocument.AddPage(inputSignatures.Pages[0]);
                }
                else if (idx == 3)
                {
                    outputMergedDocument.AddPage(inputDeclaration.Pages[0]);
                }
                else
                {
                    outputMergedDocument.AddPage(inputThesis.Pages[idx]);
                }

                
            }

            outputMergedDocument.Save(outputFile);

            outputMergedDocument.Close();
            inputThesis.Close();
            inputSignatures.Close();
            inputDeclaration.Close();

        }

        /// <summary>
        /// Imports all pages from a list of documents.
        /// </summary>
        public void MergeAdfScanning(string outputFile, string[] files)
        {
            if (files.Length == 0) { return; }

            PdfDocument outputMergedDocument = new PdfDocument();

            PdfDocument inputDocumentOdd = PdfReader.Open(files[0], PdfDocumentOpenMode.Import);
            PdfDocument inputDocumentEven = PdfReader.Open(files[1], PdfDocumentOpenMode.Import);

            if (inputDocumentOdd.PageCount != inputDocumentEven.PageCount)
            {
                MessageBox.Show("Page Size Must Be Equal");
                return;
            }

            outputMergedDocument.Version = inputDocumentOdd.Version;
            outputMergedDocument.Info.Title = "merged document";
            outputMergedDocument.Info.Creator = "application";

            int pageCount = inputDocumentOdd.PageCount;

            for (int idx = 0; idx < pageCount; idx++)
            {
                outputMergedDocument.AddPage(inputDocumentOdd.Pages[idx]);
                outputMergedDocument.AddPage(inputDocumentEven.Pages[pageCount-idx-1]);
            }

            outputMergedDocument.Save(outputFile);

            outputMergedDocument.Close();
            inputDocumentOdd.Close();
            inputDocumentEven.Close();

        }

        /// <summary>
        /// Imports all pages from a list of documents.
        /// </summary>
        public void MergeSequentialScanning(string outputFile, string[] files)
        {
            if (files.Length == 0) { return; }

            PdfDocument outputMergedDocument = new PdfDocument();


            // Iterate over input files
            foreach (string file in files)
            {
                // Open the document to import pages from it
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                // Iterate pages
                int count = inputDocument.PageCount;
                for (int i = 0; i < count; i++)
                {
                    outputMergedDocument.AddPage(inputDocument.Pages[i]);
                }

                inputDocument.Close();
            }

            outputMergedDocument.Save(outputFile);

            outputMergedDocument.Close();

        }

        private void btnMergeADF_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "PDF Documents|*.pdf";
            s.AddExtension = true;
            s.DefaultExt = "pdf";
            DialogResult dr = s.ShowDialog(this);

            if (dr != DialogResult.Cancel)
            {
                // Merge the PDFs
                string[] files = new string[listFiles.Items.Count];
                listFiles.Items.CopyTo(files, 0);

                try
                {
                    // Merge the document...
                    MergeAdfScanning(s.FileName, files);
                    // ... and start a viewer
                    Process.Start(s.FileName);
                }
                catch (UnauthorizedAccessException uaexc)
                {
                    MessageBox.Show("Error writing output document, perhaps you do not have write permission?" + Environment.NewLine +
                                    "Error message: " + uaexc.Message,
                        "Failed to write document",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                catch (System.IO.IOException ioexc)
                {
                    MessageBox.Show("Error writing output document, are you still viewing it in your PDF reader?." + Environment.NewLine +
                                    "Error message: " + ioexc.Message,
                        "Failed to write document",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void btnUpdateThesis_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "PDF Documents|*.pdf";
            s.AddExtension = true;
            s.DefaultExt = "pdf";
            DialogResult dr = s.ShowDialog(this);

            if (dr != DialogResult.Cancel)
            {
                // Merge the PDFs
                string[] files = new string[listFiles.Items.Count];
                listFiles.Items.CopyTo(files, 0);

                try
                {
                    // Merge the document...
                    ThesisUpdate(s.FileName, files);
                    // ... and start a viewer
                    Process.Start(s.FileName);
                }
                catch (UnauthorizedAccessException uaexc)
                {
                    MessageBox.Show("Error writing output document, perhaps you do not have write permission?" + Environment.NewLine +
                                    "Error message: " + uaexc.Message,
                        "Failed to write document",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                catch (System.IO.IOException ioexc)
                {
                    MessageBox.Show("Error writing output document, are you still viewing it in your PDF reader?." + Environment.NewLine +
                                    "Error message: " + ioexc.Message,
                        "Failed to write document",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }
}
