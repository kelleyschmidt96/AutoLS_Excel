using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoLSedit
{
    public partial class FormLimits : Form
    {
        public bool b_OK = false;
        public bool b_Cancel = false;
        public List<ExtraData> xdataWatcher = new List<ExtraData>();
 
        public FormLimits()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBoxMaxDev.Text.Equals(""))
            {
                MessageBox.Show("Stability option requires a Max Deviation value", "Stop");
                return;
            }
            
            //if ((comboBox1.Text.Equals("")) || textBoxDescription.Text.Equals("") || (textBoxMin.Enabled && textBoxMin.Text.Equals("")) || (textBoxMax.Enabled&&textBoxMax.Text.Equals("")) || comboBoxAction.Text.Equals(""))
            if ((comboBox1.Text.Equals("")) || textBoxDescription.Text.Equals("") || comboBoxAction.Text.Equals(""))
                {
                MessageBox.Show("Make sure all items are filled out", "Stop");
                return;
            }
            else
            {
                b_OK = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            b_Cancel = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.' || e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&(e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
                textBoxDescription.Focus();
            }
        }

        private void textBoxDescription_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
                textBoxMin.Focus();
            }

        }


        private void textBoxMin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
                textBoxMax.Focus();
            }
        }

        private void textBoxMax_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.Equals(13))
            {
                button1.Focus();
            }
        }

        private void FormLimits_Load(object sender, EventArgs e)
        {
            foreach (ExtraData xdt in xdataWatcher)
            {
                comboBox1.Items.Add(xdt.Channel);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxDescription.Text = xdataWatcher[comboBox1.SelectedIndex].Description;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
