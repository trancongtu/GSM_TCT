using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GSM_BYTUTJ2025.Profile;

namespace GSM_BYTUTJ2025
{
    public partial class fBegin : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public fBegin()
        {
            InitializeComponent();
        }

        private void btnCreatProfile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fCreatProfile f = new fCreatProfile();
            f.ShowDialog();
        }
    }
}
