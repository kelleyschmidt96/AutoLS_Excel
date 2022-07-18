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
    
    public partial class FormSetpoint : Form
    {
        public string strMCD ;
        public bool finished = false;
        public List<ExtraData> xdataPlayer = new List<ExtraData>();
        
        public FormSetpoint()


        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.Hide();
            this.Dispose();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBoxVal_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormSetpoint_Load(object sender, EventArgs e)
        {
            foreach (ExtraData xdt in xdataPlayer)
            {
                comboBox1.Items.Add(xdt.Channel);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            finished = true;
        }

        private void textBoxVal_KeyPress(object sender, KeyPressEventArgs e)
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

        private void FormSetpoint_Activated(object sender, EventArgs e)
        {
            comboBox1.Focus();
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                ComboBox cb = (ComboBox)sender;
                //textBoxDescription.Text = Description[cb.SelectedIndex];

                textBoxDescription.Text = xdataPlayer[cb.SelectedIndex].Description;
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            //textBoxDescription.Text = Description[cb.SelectedIndex];
            textBoxDescription.Text = xdataPlayer[cb.SelectedIndex].Description;
            if (!xdataPlayer[cb.SelectedIndex].Units.Equals(""))
            {
                textBoxUnit.Text = xdataPlayer[cb.SelectedIndex].Units;
            }
            else
            {
                textBoxUnit.Text = "-";
            }


        }

    }
}
