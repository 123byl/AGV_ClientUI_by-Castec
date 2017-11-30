
using CtOutLookBar.Public;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtOutLookBar.Internal {

    internal class BandTagInfo {
        public OutlookBar outlookBar;
        public int index;

        public BandTagInfo(OutlookBar ob, int index) {
            outlookBar = ob;
            this.index = index;
        }
    }

    internal class BandPanel : Panel {
        public BandPanel(string caption, OutlookCategory content, BandTagInfo bti) {
            BackColor = System.Drawing.Color.LightPink;
            //Dock = DockStyle.Fill;
            BandButton bandButton = new BandButton(caption, bti);
            content.BandButton = bandButton;
            Controls.Add(bandButton);
            Controls.Add(content.Control);
        }
    }

    internal class BandButton : Button {
        private BandTagInfo bti;

        public BandButton(string caption, BandTagInfo bti) {
            Text = caption;
            Font = new Font("微軟正黑體", 12);
            FlatStyle = FlatStyle.Standard;
            Visible = true;
            this.bti = bti;
            Click += new EventHandler(SelectBand);
        }

        private void SelectBand(object sender, EventArgs e) {
            bti.outlookBar.SelectCategory(bti.index);
        }
    }

}
