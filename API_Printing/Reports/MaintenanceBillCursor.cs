using System.ComponentModel;

namespace API_Printing.Reports
{
    public partial class MaintenanceBillCursor : DevExpress.XtraReports.UI.XtraReport
    {
        public MaintenanceBillCursor()
        {
            InitializeComponent();
        }

        private void Detail_BeforePrint(object sender, CancelEventArgs e)
        {

        }
    }
}
