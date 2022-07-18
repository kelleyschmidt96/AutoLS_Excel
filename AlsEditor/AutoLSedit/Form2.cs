using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoLSedit
{
    public partial class FormComment : Form
    {
        public bool b_OK = false;
        public bool b_Cancel = false;
        public FormComment()
        {
            InitializeComponent();
        }

        private void FormComment_Load(object sender, EventArgs e)
        {

        }

        private void FormComment_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            b_OK = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            b_Cancel = true;
        }

        private void textBoxComment_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
                b_OK = true;
            }
        }

        private void textBoxComment_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Do not allow ,
            if (e.KeyChar == ',')
            {
                e.Handled = true;
            }

        }

    }
}
