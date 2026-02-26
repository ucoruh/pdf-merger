using System.Windows.Forms;

namespace PdfMerger
{
    public partial class InputDialogForm : Form
    {
        public string InputText
        {
            get { return txtInput.Text; }
        }

        public InputDialogForm(string title, string prompt, string defaultValue)
        {
            InitializeComponent();
            Text = title;
            lblPrompt.Text = prompt;
            txtInput.Text = defaultValue ?? "";
        }
    }
}
