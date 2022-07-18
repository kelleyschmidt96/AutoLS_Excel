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
    public partial class FormFill : Form
    {
        public bool b_OK = false;
        public bool b_Cancel = false;
        public FormFill()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if ((checkBoxRepeat.Checked&&textBoxRepeat.Text.Equals("")) || textBoxRows.Text.Equals("") || textBoxStart.Text.Equals("") || textBoxIncrement.Text.Equals(""))
            {
                MessageBox.Show("Make sure all items are fille out", "Stop");
            }
            else
            { 
                b_OK = true;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            b_Cancel = true;
        }

        private void textBoxRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

        }

        private void textBoxStart_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.' && e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

        }

        private void textBoxStop_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.' && e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }


        }

        private void textBoxRepeat_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxRepeat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

        }

        private void FormFill_Load(object sender, EventArgs e)
        {
            textBoxRows.Focus();
        }

        private void FormFill_Activated(object sender, EventArgs e)
        {
            textBoxRows.Focus();
        }

        private void textBoxRows_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
                textBoxStart.Focus();
            }

        }

        private void textBoxStart_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
               textBoxIncrement.Focus();
            }

        }

        private void textBoxIncrement_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
                textBoxRepeat.Focus();
            }

        }

        private void textBoxRepeat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
                buttonOK.Focus();
            }

        }
    }
}
