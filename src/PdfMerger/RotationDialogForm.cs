using System.Windows.Forms;

namespace PdfMerger
{
    public partial class RotationDialogForm : Form
    {
        public int SelectedAngle
        {
            get
            {
                switch (cmbAngle.SelectedIndex)
                {
                    case 0: return 90;
                    case 1: return 180;
                    case 2: return 270;
                    default: return 90;
                }
            }
        }

        public RotationDialogForm()
        {
            InitializeComponent();
            cmbAngle.SelectedIndex = 0;
        }
    }
}
