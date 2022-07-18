using System;
using System.IO;
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

    public partial class Form1 : Form
   
    {
        // Added to GIT 8-19-16 9:31 am
        string sProjectName = "";
        private ContextMenuStrip contextMenuForHeader = new ContextMenuStrip();
        private ContextMenuStrip contextMenuForRowHeader = new ContextMenuStrip();
        private ContextMenuStrip contextMenuForActuators = new ContextMenuStrip();
        private ContextMenuStrip contextMenuForRows = new ContextMenuStrip();
        private Point CellPoint = new Point();
        List<string> SafePoint = new List<string>();
        List<string> Transition = new List<string>();
        List<string> TransitionValue = new List<string>();
        List<string> Units = new List<string>();
        List<string> Description = new List<string>();
        List<string> GraphYaxis=new List<string>();
        List<string> LimitsAction = new List<string>();
        List<string> Required = new List<string>();
        List<string> StabMax = new List<string>();
        List<string> Comment = new List<string>();
        public List<ExtraData> ExtraDataPlayer = new List<ExtraData>();
        public List<ExtraData> ExtraDataWatcher = new List<ExtraData>();
        bool DataGridChanged = false;
        bool MCDChanged = false;
        public string strMCD = "";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.AddMenuItems();
            GetRecentFileList();
            toolStrip1.Items[2].Text = "MCD File:" + strMCD;
        }
        private void GetExtraData()
        {
            ExtraDataPlayer.Clear();
            ExtraDataWatcher.Clear();
            //--- Players
            XmlDocument xd = new XmlDocument();
            string fName = strMCD;
            if (!strMCD.Contains("MCSystem.xmcd"))
            {
                fName = strMCD + "\\MCSystem.xmcd";
            }
            try
            {
                // MessageBox.Show(fName);
                xd.Load(fName);
            }
            catch
            {
                MessageBox.Show(fName);
                return;
            }
            XmlNode Header = xd.SelectSingleNode("//Parameters");
            foreach (XmlNode hdr in Header.ChildNodes)
            {
                if (hdr.Name.Equals("Parameter"))
                {
                     ExtraData ed = new ExtraData();
                     ed.Channel=hdr.Attributes.GetNamedItem("IdMorphee").Value;
                     ed.Description=hdr.Attributes.GetNamedItem("IdA2L").Value;
                     ed.Units = hdr.Attributes.GetNamedItem("UnitA2L").Value;
                     ExtraDataPlayer.Add(ed);
                }
            }
            XmlDocument xd1 = new XmlDocument();
            fName = Properties.Settings.Default.ExtDataRoot;
            try { xd1.Load(fName); }
            catch
            { return; }
            XmlNode Header1 = xd1.SelectSingleNode("//quantities");
            foreach (XmlNode hdr in Header1.ChildNodes)
            {
                if (hdr.Name.Equals("player"))
                {
                    ExtraData ed = new ExtraData();
                    ed.Channel=hdr.ChildNodes[0].InnerXml;
                    ed.Description=hdr.ChildNodes[1].InnerXml;
                    ed.Units=hdr.ChildNodes[2].InnerXml;
                    ExtraDataPlayer.Add(ed);
                }
                // Watchers

                if (hdr.Name.Equals("watcher"))
                {
                    ExtraData ed = new ExtraData();
                    ed.Channel = hdr.ChildNodes[0].InnerXml;
                    ed.Description = hdr.ChildNodes[1].InnerXml;
                    ExtraDataWatcher.Add(ed);
                }
            }
        }
        private void GetRecentFileList()
        {
            int n = fileToolStripMenuItem.DropDownItems.Count;
            for (int i = 7; i < n; i++)
			{
                fileToolStripMenuItem.DropDownItems.RemoveAt(7);
			 
			}
            
            string s = Properties.Settings.Default.RecentFiles;
            string[] lastfiles = s.Split(',');
            foreach (string ss in lastfiles)
            {
                ToolStripMenuItem filerecent = new ToolStripMenuItem(ss, null, RecentFile_click);
                fileToolStripMenuItem.DropDownItems.Add(filerecent);
            }


        }

        private void RecentFile_click(object sender, EventArgs e)
        {
            ToolStripMenuItem cs = (ToolStripMenuItem)sender;
            if (OpenXML(cs.Text))
            {
                toolStrip1.Items[2].Text = "MCD File:" + strMCD;
                CheckMCD();
                GetExtraData();
            }
        }


        void contextMenuForActuators_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            DataGridView.HitTestInfo hitTestInfo;
            // Safepoint toggle
            if(item.Text.Equals("Toggle Safepoint"))
            {
                int  n = dataGridView1.CurrentRow.Index;
                int m = n + 1;
                //if(dataGridView1.Rows[n].DefaultCellStyle.BackColor.Equals(Color.LightGreen))
                if (SafePoint.Contains(m.ToString()))
                
                {
                    int n1 = GetLastRequired();
                    int length = CountActuators()+n1;
                    // Blue
                    for (int i = 0; i < n1; i++)
                    {
                        dataGridView1.Rows[n].Cells[i].Style.BackColor = Color.PowderBlue;
                    }
                    // White
                    for (int i = n1; i < length; i++)
                    {
                        dataGridView1.Rows[n].Cells[i].Style.BackColor = Color.White;
                    }
                    // Yellow
                    for (int i = length; i < dataGridView1.Columns.Count; i++)
                    {
                        dataGridView1.Rows[n].Cells[i].Style.BackColor = Color.Wheat;
                    }

                    int o = 0;
                    
                    foreach(string s in SafePoint)
                    {
                        int p = n+1 ;
                        if(s.Equals(p.ToString()))
                        {
                            SafePoint.RemoveAt(o);
                            break;
                        }
                        o++;
                    }
                }
                else
                {
                        for (int i1 = 0; i1 < dataGridView1.Columns.Count; i1++)
                        {
                            dataGridView1.Rows[n].Cells[i1].Style.BackColor = Color.LightGreen;
                        }
                    {
                        int x = n+1 ;
                        SafePoint.Insert(0, x.ToString());
                    }
                }
                DataGridChanged = true;
            }
            //-------------------------------------------------            
            // Insert Row
            //-------------------------------------------------            
            if (item.Text.Equals("Insert Row"))
            {

                hitTestInfo = dataGridView1.HitTest(CellPoint.X,CellPoint.Y);
                if (hitTestInfo.RowIndex > 0)
                {
                    Comment.Insert(hitTestInfo.RowIndex, "");
                    dataGridView1.Rows.InsertCopy(hitTestInfo.RowIndex,0);
                    Renumber();

                }
                else
                {
                    dataGridView1.Rows.Insert(hitTestInfo.RowIndex);
                    Comment.Add("");
                    Renumber();
                }
                for (int c = 0; c < dataGridView1.ColumnCount; c++)
                {

                if (c.Equals(4))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.Dyno.ToString();
                }

                else if (c.Equals(5))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.MinST.ToString();
                }
                else if (c.Equals(6))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.MaxST.ToString();
                }
                else if (c.Equals(7))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.StepTime.ToString();
                }
                else if (c.Equals(8))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.Measure.ToString();
                }
                else if (!dataGridView1.Columns[c].Tag.Equals("Limit_Min") && !dataGridView1.Columns[c].Tag.Equals("Limit_Max"))
                {
                    dataGridView1.Rows[0].Cells[c].Value = "0.0";

                }
                else if (dataGridView1.Columns[c].Tag.Equals("Limit_Min") || dataGridView1.Columns[c].Tag.Equals("Limit_Max"))
                {
                    dataGridView1.Rows[0].Cells[c].Value = "";

                }

             }
            }
            //-------------------------------------------------            
            // Insert Rows
            //-------------------------------------------------            
            if (item.Text.Equals("Insert Row(s)"))
            {
                hitTestInfo = dataGridView1.HitTest(CellPoint.X, CellPoint.Y);
                bool b = false;
                bool c = false;
                FormInput fi = new FormInput();
                fi.Show();

                int x = CellPoint.X + this.Left;
                int z = CellPoint.Y + this.Top+50;
                
                fi.SetDesktopLocation(x, z );
                do
                {
                    Application.DoEvents();
                    b = fi.b_OK;
                    c = fi.b_Cancel;
                } while (b.Equals(false)&&c.Equals(false));
                fi.Hide();
                dataGridView1.Cursor = Cursors.WaitCursor;
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                if (c.Equals(false))
                {


                    int n;
                    if(int.TryParse(fi.textBox1.Text,out n))
                    {
                    if (hitTestInfo.RowIndex > 0)
                    {
                        dataGridView1.Rows.InsertCopies(hitTestInfo.RowIndex - 1, hitTestInfo.RowIndex, n);

                        for (int i = 0; i < n; i++)
                        {
                            Comment.Insert(hitTestInfo.RowIndex + i, "");
                            for (int d = 0; d < dataGridView1.ColumnCount; d++)
                            {
                                dataGridView1.Rows[hitTestInfo.RowIndex + i].Cells[d].Value = dataGridView1.Rows[hitTestInfo.RowIndex - 1].Cells[d].Value;
 
                            }
                        }
                    }
                    else
                    {
                        dataGridView1.Rows.InsertCopies(hitTestInfo.RowIndex, hitTestInfo.RowIndex + 1, n - 1);
                        for (int i = 0; i < n; i++)
                        {
                            Comment.Add("");
                            for (int d = 0; d < dataGridView1.ColumnCount; d++)
                            {
                                if (d.Equals(7)&&dataGridView1.Rows[hitTestInfo.RowIndex].Cells[d].Value.Equals(0))
                                {
                                    dataGridView1.Rows[hitTestInfo.RowIndex + i].Cells[d].Value = Properties.Settings.Default.StepTime;
                                }
                                else
                                {
                                    dataGridView1.Rows[hitTestInfo.RowIndex + i].Cells[d].Value = dataGridView1.Rows[hitTestInfo.RowIndex].Cells[d].Value;
                                }
                            }
                        }
                        }
                    }
                }
                Renumber();
               dataGridView1.Cursor = Cursors.Default;
               dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                fi.Dispose();

            }
            //-------------------------------------------------            
            // Paste Clipboard
            //-------------------------------------------------            
            if (item.Text.Equals("Paste"))
            {
                KeyEventArgs ka = new KeyEventArgs(Keys.Control | Keys.V);
                dataGridView1_KeyUp(dataGridView1, (KeyEventArgs)ka);
            }

            //-------------------------------------------------            
            // Copy Clipboard
            //-------------------------------------------------            
            if (item.Text.Equals("Copy"))
            {
                KeyEventArgs ka = new KeyEventArgs(Keys.Control | Keys.C);
                dataGridView1_KeyUp(dataGridView1, (KeyEventArgs)ka);
            }


            //-------------------------------------------------            
            // Fill Column(s)
            if (item.Text.Equals("Fill Column"))
            {
                hitTestInfo = dataGridView1.HitTest(CellPoint.X, CellPoint.Y);
                bool b = false;
                bool c = false;
                FormFill fi = new FormFill();

                int x = CellPoint.X + this.Left;
                int z = CellPoint.Y + this.Top + 50;
                fi.Show();
                fi.SetDesktopLocation(x, z);
                do
                {
                    Application.DoEvents();
                    b = fi.b_OK;
                    c = fi.b_Cancel;
                } while (b.Equals(false)&&c.Equals(false));
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                fi.Hide();
                if (c.Equals(true))
                    {return;}

                int numRows = int.Parse(fi.textBoxRows.Text)-1;
                double iStart = double.Parse(fi.textBoxStart.Text);
                double iEnd = double.Parse(fi.textBoxIncrement.Text);
                double iNcrement = double.Parse(fi.textBoxIncrement.Text);
                double dFill = iStart;
                double lastvalue = 0;
                if (fi.checkBoxRepeat.Checked.Equals(true))
                {
                    int numRepeat = int.Parse(fi.textBoxRepeat.Text);
                    int dgvr = 0;
                    for (int r = 0; r < numRepeat; r++)
                    {
                        dFill = iStart;
                        for (int d = 0; d < numRows + 1; d++)
                        {
                            if (hitTestInfo.RowIndex + dgvr <= dataGridView1.RowCount - 1)
                            {
                                dataGridView1.Rows[hitTestInfo.RowIndex + dgvr].Cells[hitTestInfo.ColumnIndex].Value = dFill;
                                dFill = dFill + iNcrement;
                                lastvalue = dFill;
                                dgvr++;
                            }
                        }
                        // Vertical Mirror
                        if (fi.checkBoxMirror.Checked.Equals(true))
                        {
                            dFill = lastvalue-1;
                            for (int d = 0; d < numRows + 1; d++)
                            {
                                if (hitTestInfo.RowIndex + dgvr <= dataGridView1.RowCount - 1)
                                {
                                    dataGridView1.Rows[hitTestInfo.RowIndex + dgvr].Cells[hitTestInfo.ColumnIndex].Value = dFill;
                                    dFill = dFill - iNcrement;
                                    dgvr++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int dgvr = 0;

                    for (int d = 0; d < numRows+1; d++)
                    {
                        if (hitTestInfo.RowIndex + d <= dataGridView1.RowCount-1)
                        {
                            dataGridView1.Rows[hitTestInfo.RowIndex + d].Cells[hitTestInfo.ColumnIndex].Value = dFill;
                            dFill = dFill + iNcrement;
                            lastvalue = dFill;
                            dgvr++;
                        }
                    }
                    // Vertical Mirror
                    if (fi.checkBoxMirror.Checked.Equals(true))
                    {
                        dFill = lastvalue - 1;
                        for (int d = 0; d < numRows + 1; d++)
                        {
                            if (hitTestInfo.RowIndex + dgvr <= dataGridView1.RowCount - 2)
                            {
                                dataGridView1.Rows[hitTestInfo.RowIndex + dgvr].Cells[hitTestInfo.ColumnIndex].Value = dFill;
                                dFill = dFill - iNcrement;
                                dgvr++;
                            }
                        }
                    }

                }
                fi.Dispose();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }

        }
        void contextMenuForHeader_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo;
            ToolStripItem item = e.ClickedItem;
            ContextMenuStrip cs = (ContextMenuStrip)sender;
            int x = cs.Left;
            int y = cs.Top;
            hitTestInfo = dataGridView1.HitTest(CellPoint.X, CellPoint.Y);
            //----------------------------------------------------------------------------------------------------------------
            if (item.Text.Equals("Edit Actuator"))
            //----------------------------------------------------------------------------------------------------------------
            {


                if (!hitTestInfo.ColumnIndex.Equals(0) && !hitTestInfo.ColumnIndex.Equals(4) && !hitTestInfo.ColumnIndex.Equals(5)
                     && !hitTestInfo.ColumnIndex.Equals(6)&& !hitTestInfo.ColumnIndex.Equals(7)&& !hitTestInfo.ColumnIndex.Equals(8))
                {
                    if (dataGridView1.Columns[hitTestInfo.ColumnIndex].Tag.Equals("Actuator"))
                    {
                        FormSetpoint fs = new FormSetpoint();
                        // Transfer player list
                        fs.xdataPlayer = ExtraDataPlayer;
                        fs.strMCD = strMCD;
                        fs.comboBox1.Text = dataGridView1.Columns[hitTestInfo.ColumnIndex].HeaderText;
                        fs.textBoxDescription.Text =Description[hitTestInfo.ColumnIndex];

                        int x1 = this.Left + (this.Width / 2);
                        int z = this.Top + 100;
                        fs.Show(dataGridView1);
                        fs.SetDesktopLocation(x1,z);
                        if (hitTestInfo.ColumnIndex <= 8)
                        {
                            fs.comboBox1.Enabled = false;
                            fs.textBoxDescription.Enabled = false;
                            fs.textBoxUnit.Enabled = false;
                        }
                        fs.textBoxVal.Text = TransitionValue[hitTestInfo.ColumnIndex].ToString();
                        fs.textBoxDescription.Text = Description[hitTestInfo.ColumnIndex];
                        fs.textBoxUnit.Text = Units[hitTestInfo.ColumnIndex];
                        if (Transition[hitTestInfo.ColumnIndex].Equals("Slope"))
                        { fs.radioButton1.Select();}
                        if (Transition[hitTestInfo.ColumnIndex].Equals("Gradient"))
                        { fs.radioButton2.Select();}
                        if (Transition[hitTestInfo.ColumnIndex].Equals("Step"))
                        { fs.radioButton3.Select();}
                        if (Transition[hitTestInfo.ColumnIndex].Equals("By Row"))
                        { fs.radioButton4.Select(); }
                        if (Transition[hitTestInfo.ColumnIndex].Equals(""))
                        { fs.radioButton1.Select(); }
                        bool fsf = false;
                        do
                        {
                            Application.DoEvents();
                            fsf = fs.finished;
                        } while (fsf.Equals(false));
                        dataGridView1.Columns[hitTestInfo.ColumnIndex].HeaderText = fs.comboBox1.Text;
                        dataGridView1.Columns[hitTestInfo.ColumnIndex].ToolTipText = fs.textBoxDescription.Text;
                        Description[hitTestInfo.ColumnIndex] = fs.textBoxDescription.Text;
                        if (dataGridView1.Columns[hitTestInfo.ColumnIndex].Tag.Equals("Limit_Max"))
                        {
                            dataGridView1.Columns[hitTestInfo.ColumnIndex].HeaderText = fs.comboBox1.Text;
                            Description[hitTestInfo.ColumnIndex] = fs.textBoxDescription.Text;
                        }
                        if (dataGridView1.Columns[hitTestInfo.ColumnIndex].Tag.Equals("Limit_Min"))
                        {
                            dataGridView1.Columns[hitTestInfo.ColumnIndex].HeaderText = fs.comboBox1.Text;
                            Description[hitTestInfo.ColumnIndex] = fs.textBoxDescription.Text;
                        }
                        TransitionValue[hitTestInfo.ColumnIndex] = fs.textBoxVal.Text;
                        if (fs.radioButton1.Checked)
                            {Transition[hitTestInfo.ColumnIndex]="Slope";}
                        if (fs.radioButton2.Checked)
                            {Transition[hitTestInfo.ColumnIndex]="Gradient";}
                        if (fs.radioButton3.Checked)
                        { Transition[hitTestInfo.ColumnIndex] = "Step";}
                        if (fs.radioButton4.Checked)
                        { Transition[hitTestInfo.ColumnIndex] = "By Row"; }
                        Description[hitTestInfo.ColumnIndex] = fs.textBoxDescription.Text;
                        Units[hitTestInfo.ColumnIndex] = fs.textBoxUnit.Text;
                        
                        fs.Dispose();
                    }
            }
            }
            //----------------------------------------------------------------------------------------------------------------
            if (item.Text.Equals("Edit Limit"))
            //----------------------------------------------------------------------------------------------------------------
            {
                string columntag = dataGridView1.Columns[hitTestInfo.ColumnIndex].Tag.ToString();
                if (columntag.Equals("Limit_Min")||columntag.Equals("Limit_Max"))
                {
                    FormLimits fl = new FormLimits();
                    // Transfer watcher list
                    fl.xdataWatcher = ExtraDataWatcher;

                    bool b = false;
                    bool c = false;
                    string s = dataGridView1.Columns[hitTestInfo.ColumnIndex].HeaderText;

                    if (columntag.Equals("Limit_Min"))
                    {
                        fl.comboBox1.Text = s.Substring(0, s.Length - 4);
                        fl.textBoxDescription.Text = Description[hitTestInfo.ColumnIndex];
                    }
                    
                    if (columntag.Equals("Limit_Max"))
                    {
                        fl.comboBox1.Text = s.Substring(0, s.Length - 5);
                        fl.textBoxDescription.Text = Description[hitTestInfo.ColumnIndex];
                    }

                    
                    
                    
                    fl.Show(dataGridView1);
                    fl.textBoxMin.Enabled = false;
                    fl.textBoxMax.Enabled = false;
                    fl.comboBoxAction.Text = LimitsAction[hitTestInfo.ColumnIndex];
                    fl.textBoxMaxDev.Text = StabMax[hitTestInfo.ColumnIndex];


                    int x1 = CellPoint.X + this.Left;
                    int z = CellPoint.Y + this.Top + 50;

                    
                    
                    
                    
                    //fl.SetDesktopLocation(x, y);
                    fl.SetDesktopLocation(x1,z);
                    do
                    {
                        Application.DoEvents();
                        b = fl.b_OK;
                        c = fl.b_Cancel;
                    } while (b.Equals(false)&&c.Equals(false));
                    fl.Hide();
                    if (c.Equals(false))
                    {
                        
                        if (columntag.Equals("Limit_Min"))
                        {
                            dataGridView1.Columns[hitTestInfo.ColumnIndex].HeaderText = fl.comboBox1.Text + "_Low"; ;
                            dataGridView1.Columns[hitTestInfo.ColumnIndex + 1].HeaderText = fl.comboBox1.Text + "_High"; ;
    
                            Description[hitTestInfo.ColumnIndex] = fl.textBoxDescription.Text;
                            Description[hitTestInfo.ColumnIndex + 1] = fl.textBoxDescription.Text;

                            LimitsAction[hitTestInfo.ColumnIndex] = fl.comboBoxAction.Text;
                            LimitsAction[hitTestInfo.ColumnIndex + 1] = fl.comboBoxAction.Text;

                            StabMax[hitTestInfo.ColumnIndex] = fl.textBoxMaxDev.Text;
                            StabMax[hitTestInfo.ColumnIndex+1] = fl.textBoxMaxDev.Text;


                        }
                        if (columntag.Equals("Limit_Max"))
                        {
                            dataGridView1.Columns[hitTestInfo.ColumnIndex].HeaderText = fl.comboBox1.Text + "_High"; ;
                            dataGridView1.Columns[hitTestInfo.ColumnIndex - 1].HeaderText = fl.comboBox1.Text + "_Low"; ;

                            Description[hitTestInfo.ColumnIndex] = fl.textBoxDescription.Text;
                            Description[hitTestInfo.ColumnIndex - 1] = fl.textBoxDescription.Text;

                            LimitsAction[hitTestInfo.ColumnIndex] = fl.comboBoxAction.Text;
                            LimitsAction[hitTestInfo.ColumnIndex - 1] = fl.comboBoxAction.Text;

                            StabMax[hitTestInfo.ColumnIndex] = fl.textBoxMaxDev.Text;
                            StabMax[hitTestInfo.ColumnIndex-1] = fl.textBoxMaxDev.Text;


                        }
                    }
                    fl.Dispose();
                }
            }

                //----------------------------------------------------------------------------------------------------------------
                if (item.Text.Equals("Add Actuator"))
                //----------------------------------------------------------------------------------------------------------------
                {
                    Addnewactuator("", dataGridView1.Width/2, dataGridView1.Height/2);
                }
                //----------------------------------------------------------------------------------------------------------------
                if (item.Text.Equals("Remove Actuator"))
                //----------------------------------------------------------------------------------------------------------------
                {
                        if (dataGridView1.Columns[hitTestInfo.ColumnIndex].Tag.Equals("Actuator"))
                        {
                            {
                                if (!dataGridView1.Rows[0].Cells[hitTestInfo.ColumnIndex].Value.Equals("STAB_MIN") &&
                                    !dataGridView1.Rows[0].Cells[hitTestInfo.ColumnIndex].Value.Equals("STAB_MAX"))
                                {
                                    dataGridView1.Columns.RemoveAt(hitTestInfo.ColumnIndex);
                                }
                                Transition.RemoveAt(hitTestInfo.ColumnIndex);
                                TransitionValue.RemoveAt(hitTestInfo.ColumnIndex);
                                Description.RemoveAt(hitTestInfo.ColumnIndex);
                                Units.RemoveAt(hitTestInfo.ColumnIndex);
                                LimitsAction.RemoveAt(hitTestInfo.ColumnIndex);
                            }

                        }


                    }
                //----------------------------------------------------------------------------------------------------------------
                if (item.Text.Equals("Remove Limit"))
                //----------------------------------------------------------------------------------------------------------------
                {
                    {
                            if (dataGridView1.Columns[hitTestInfo.ColumnIndex].Tag.Equals("Limit_Min"))
                            {
                                if (CountLimits() > 1)
                                {

                                dataGridView1.Columns.RemoveAt(hitTestInfo.ColumnIndex + 1);
                                dataGridView1.Columns.RemoveAt(hitTestInfo.ColumnIndex);

                                Transition.RemoveAt(hitTestInfo.ColumnIndex + 1);
                                TransitionValue.RemoveAt(hitTestInfo.ColumnIndex + 1);
                                Description.RemoveAt(hitTestInfo.ColumnIndex + 1);
                                Units.RemoveAt(hitTestInfo.ColumnIndex + 1);
                                LimitsAction.RemoveAt(hitTestInfo.ColumnIndex + 1);

                                Transition.RemoveAt(hitTestInfo.ColumnIndex);
                                TransitionValue.RemoveAt(hitTestInfo.ColumnIndex);
                                Description.RemoveAt(hitTestInfo.ColumnIndex);
                                Units.RemoveAt(hitTestInfo.ColumnIndex);
                                LimitsAction.RemoveAt(hitTestInfo.ColumnIndex);
                                return;
                              }
                            else
                            {
                                MessageBox.Show("Must have at least one limit set", "Warning", MessageBoxButtons.OK);

                            }

                            }

                            if (dataGridView1.Columns[hitTestInfo.ColumnIndex].Tag.Equals("Limit_Max"))
                            {
                                if (CountLimits() > 1)
                                {

                                    dataGridView1.Columns.RemoveAt(hitTestInfo.ColumnIndex);
                                    dataGridView1.Columns.RemoveAt(hitTestInfo.ColumnIndex - 1);

                                    Transition.RemoveAt(hitTestInfo.ColumnIndex);
                                    TransitionValue.RemoveAt(hitTestInfo.ColumnIndex);
                                    Description.RemoveAt(hitTestInfo.ColumnIndex);
                                    Units.RemoveAt(hitTestInfo.ColumnIndex);
                                    LimitsAction.RemoveAt(hitTestInfo.ColumnIndex);

                                    Transition.RemoveAt(hitTestInfo.ColumnIndex - 1);
                                    TransitionValue.RemoveAt(hitTestInfo.ColumnIndex - 1);
                                    Description.RemoveAt(hitTestInfo.ColumnIndex - 1);
                                    Units.RemoveAt(hitTestInfo.ColumnIndex - 1);
                                    LimitsAction.RemoveAt(hitTestInfo.ColumnIndex - 1);
                                    return;
                                }
                                else
                                {
                                    MessageBox.Show("Must have at least one limit set", "Warning", MessageBoxButtons.OK);

                                }
                        }
                    }
                }

                //----------------------------------------------------------------------------------------------------------------
                if (item.Text.Equals("Add to Graph"))
                //----------------------------------------------------------------------------------------------------------------
                {
                    //GraphYaxis.Add(dataGridView1.Columns[hitTestInfo.ColumnIndex].HeaderText);
                }
                //=======================================================================================
                if (item.Text.Equals("Add Limit"))
                //=======================================================================================
                {

                    AddLimits("");

                
            }
        }
        void contextMenuForRowHeader_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo;
            ToolStripItem item = e.ClickedItem;
            ContextMenuStrip cs = (ContextMenuStrip)sender;
            int x = cs.Left;
            int y = cs.Top;
            hitTestInfo = dataGridView1.HitTest(CellPoint.X, CellPoint.Y);

            //-------------------------------------------------            
            // Delete Row
            //-------------------------------------------------            
            if (item.Text.Equals("Delete Row(s)"))
            {
                for (int i =dataGridView1.Rows.Count - 1; i >= 0; i--)
                {
                    if (dataGridView1.Rows[i].Selected)
                    {
                        dataGridView1.Rows.RemoveAt(i);
                        Comment.RemoveAt(i );
                    }
                }
                Renumber();
            }
            //-------------------------------------------------            
            // Manage Comment
            //-------------------------------------------------            
            if (item.Text.Equals("Add/Remove Comment"))
            {

                FormComment fc = new FormComment();
                bool b = false;
                bool c = false;


                fc.textBoxComment.Text = Comment[dataGridView1.CurrentRow.Index];
                fc.Show(dataGridView1);
                fc.SetDesktopLocation(dataGridView1.Width / 2, dataGridView1.Height / 2);
                do
                {
                    Application.DoEvents();
                    b = fc.b_OK;
                    c = fc.b_Cancel;
                } while (b.Equals(false) && c.Equals(false));
                
                fc.Hide();
                Comment[dataGridView1.CurrentRow.Index]=fc.textBoxComment.Text;
                dataGridView1.CurrentRow.HeaderCell.ToolTipText = fc.textBoxComment.Text;
                dataGridView1.Refresh();
                DataGridChanged = true;
            }
        
        }

        private void Renumber()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                int x = i + 1;
                dataGridView1.Rows[i].HeaderCell.Value = x.ToString();
            }

        }
        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            Renumber();
        }
        private string GetSafePoints()
        {
            string s="";
            int z1 = 1;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                ///if (dr.DefaultCellStyle.BackColor.Equals(Color.LightGreen))

                if (SafePoint.Contains(z1.ToString()))

                {
                    int n=dr.Index+1;
                    s=string.Concat(s,z1.ToString(),",");
                }
                z1++;
            }
            if (s.Length > 0)
            { s = s.Substring(0, s.Length - 1); }
            else
            { s = "1"; }
           return s;

        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            Renumber();
        }
        private int CountLimits()
        {
            int nCount=0;
            for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
            {
                if (dataGridView1.Columns[i].Tag.Equals("Limit_Min"))
                { nCount++; }

            }
                return nCount;
        }
        private int CountActuators()
        {
            int nCount = 0;

            for (int i = 0; i < dataGridView1.Columns.Count ; i++)
            {
                if (dataGridView1.Columns[i].Tag.Equals("Actuator"))
                {
                   if (Required[i].Equals("0"))
                    {
                        nCount++;
                    }
                }

            }
            return nCount;
        }

        private double GetmaxSlope(int nRow)
        {
            int nMaxSlope = 0;
            for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
            {
                foreach (string s in GraphYaxis)
                {
                    if (s.Equals(dataGridView1.Columns[i].HeaderText.ToString()))
                    {
                        if (Transition[i].Equals("Slope"))
                        {
                       //     Int16 cs = Convert.ToInt16(Int16.Parse(TransitionValue[i].ToString()));
                            Int16 cs = Convert.ToInt16(Double.Parse(TransitionValue[i]));
                            if (cs > nMaxSlope)
                            {
                                nMaxSlope = cs;
                            }
                        }
                        if (Transition[i].Equals("Gradient"))
                        {

                            double c = double.Parse(dataGridView1.Rows[nRow].Cells[i].Value.ToString());
                            double d = double.Parse(dataGridView1.Rows[nRow - 1].Cells[i].Value.ToString());
                            double Gradient = (c - d) / Convert.ToInt16(Double.Parse(TransitionValue[i]));

                            if (Gradient > nMaxSlope)
                            {
                                  nMaxSlope = Convert.ToInt16(Gradient);
                            }
                        }
                        if (Transition[i].Equals("By Row"))
                        {
                            nMaxSlope = Convert.ToInt16(double.Parse(dataGridView1.Rows[nRow].Cells[7].Value.ToString()));
                        }
                        if (Transition[i].Equals("Step"))
                        {
                        }
                    }
                }
            }
            return nMaxSlope;
        }

        private double GetCurrentSlope(int nRow, int nSeries)
        {
            int series = 0;
            double nCurrentSlope = 0;
              foreach (string s in GraphYaxis)
               {
                   if (series.Equals(nSeries))
                   {

                    for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
                    {
                        if (s.Equals(dataGridView1.Columns[i].HeaderText.ToString()))
                        {
                            if (Transition[i].Equals("Slope"))
                            {
                                Int16 cs = Convert.ToInt16(Double.Parse(TransitionValue[i]));
                                nCurrentSlope = cs;
                            }
                            if (Transition[i].Equals("Gradient"))
                            {

                                double c = (double)dataGridView1.Rows[nRow].Cells[i].Value;
                                double d = (double)dataGridView1.Rows[nRow - 1].Cells[i].Value;
                                double Gradient = (c - d) / double.Parse(TransitionValue[i]);

                                if (Gradient > nCurrentSlope)
                                {
                                    nCurrentSlope = Gradient;
                                }
                            }
                            if (Transition[i].Equals("By Row"))
                            {
                                nCurrentSlope = Convert.ToInt16((double)dataGridView1.Rows[nRow].Cells[7].Value);
                            }
                            if (Transition[i].Equals("Step"))
                            {
                            }
                            // series++;
                        }
                        
                    }
                    
                }
                   series++;
            }
            return nCurrentSlope;
        }



        private void viewChartToolStripMenuItem_Click(object sender, EventArgs e)
        {


            // Display chart of actuators including Min_St and Measure Time
            FormChart fc = new FormChart();
            fc.Show();
            int nSeries=0;
            int x2 = 0;
            //-------------------------------------------------------------------------------------------------------------------------------
            // Calculate X and create graph with X points with Max Slope
            // Then come back and fill in values with individual slopes
            //-------------------------------------------------------------------------------------------------------------------------------
            for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
            {
                foreach (string s in GraphYaxis)
                {
                    x2 = 0;

                    if(s.Equals(dataGridView1.Columns[i].HeaderText.ToString()))
                    {
                        fc.chart1.Series.Add(s);
                        fc.chart1.Series[nSeries].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                        fc.chart1.Series[nSeries].Points.Add(0);
                        x2++;

                        for (int j = 1; j < dataGridView1.RowCount-1; j++)
                        {
                            double nMaxSlope = GetmaxSlope(j);
                            double d = double.Parse(dataGridView1.Rows[j - 1].Cells[i].Value.ToString());
                            double c = double.Parse(dataGridView1.Rows[j].Cells[i].Value.ToString());
                            double increment = (c - d) / nMaxSlope;
                            double l =d;
                            int kx = 0;
                            for (int k = 0; k < nMaxSlope; k++)
                            {
                                l = l + increment;
                                fc.chart1.Series[nSeries].Points.Add(0);
                                kx++;
                                x2++;
                            }
                            string[]r1=s.Split(',');
                            r1.Count();
                            //Max Stab time
                            double minst = double.Parse(dataGridView1.Rows[j-1].Cells[6].Value.ToString());
                            for (int w = 0; w < minst; w++)
                            {
                               fc.chart1.Series[nSeries].Points.Add(0);
                                x2++;
                            }
                            //Measurement Time
                            minst = double.Parse(dataGridView1.Rows[j - 1].Cells[8].Value.ToString());
                            for (int w = 0; w < minst; w++)
                            {
                                fc.chart1.Series[nSeries].Points.Add(0);
                                x2++;
                            }
                        }
                        nSeries++;
                    }
                }
            }
            //-------------------------------------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------------------------------------
            x2 = 0;
            nSeries = 0;
            for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
            {
                foreach (string s in GraphYaxis)
                {
                    if (s.Equals(dataGridView1.Columns[i].HeaderText.ToString()))
                    {

                        x2 = 0;
                        fc.chart1.Series[nSeries].Points[x2].YValues[0] = 0;
                        x2++;
                        for (int j = 1; j < dataGridView1.RowCount - 1; j++)
                        {
                            double nCurrentSlope = GetCurrentSlope(j, nSeries);
                            double nMaxSlope = GetmaxSlope(j);
                            double d = double.Parse(dataGridView1.Rows[j - 1].Cells[i].Value.ToString());
                            double c = double.Parse(dataGridView1.Rows[j].Cells[i].Value.ToString());
                            double increment = (c - d) / nCurrentSlope;
                            double l = d;
                            int kx = 0;
                            for (int k = 0; k < nCurrentSlope; k++)
                            {
                                l = l + increment;
                                //                                fc.chart1.Series[nSeries].Points.Add(0);
                                fc.chart1.Series[nSeries].Points[x2].YValues[0] = l;
                                x2++;

                                kx++;
                            }
                            string[] r1 = s.Split(',');
                            r1.Count();

                            // Remainder of nMaxSlope
                            double SlopeDiff = nMaxSlope - nCurrentSlope;
                            for (int w = 0; w < SlopeDiff; w++)
                            {
                                // fc.chart1.Series[nSeries].Points.Add(0);
                                fc.chart1.Series[nSeries].Points[x2].YValues[0] = l;
                                x2++;

                            }



                            //Max Stab time
                            double minst = double.Parse(dataGridView1.Rows[j - 1].Cells[6].Value.ToString());
                            for (int w = 0; w < minst; w++)
                            {
                                // fc.chart1.Series[nSeries].Points.Add(0);
                                fc.chart1.Series[nSeries].Points[x2].YValues[0] = l;
                                x2++;

                            }
                            //Measurement Time
                            minst = double.Parse(dataGridView1.Rows[j - 1].Cells[8].Value.ToString());
                            for (int w = 0; w < minst; w++)
                            {
                                //fc.chart1.Series[nSeries].Points.Add(0);
                                fc.chart1.Series[nSeries].Points[x2].YValues[0] = l;
                                x2++;
                            }
                        }
                        nSeries++;
                    }
                }
            }

        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            if(sProjectName.Equals(""))
            {
              

                saveAsToolStripMenuItem_Click(null,null);

                return;
            }
            if (!sProjectName.Contains( Properties.Settings.Default.TPXL_Root))
            {
                sProjectName = Properties.Settings.Default.TPXL_Root.ToString() +"\\"+ sProjectName;
            }
            if (!sProjectName.ToLower().Contains(".tpxl"))
            {
                sProjectName = sProjectName+".tpxl";
            }


            string sValue = "";
            int nSetpoints = dataGridView1.RowCount ;
            string sSafePoints = GetSafePoints();
            XmlDocument PWConfig;
            PWConfig = new XmlDocument();
            XmlTextWriter wr = new XmlTextWriter(sProjectName, Encoding.UTF8);
//            XmlTextWriter wr = new XmlTextWriter("C:\\temp\\ALStest.xml", Encoding.UTF8);
            wr.Formatting = Formatting.Indented;
            wr.WriteStartDocument(true);
            //-------------------------------------------------            
            // Header
            //-------------------------------------------------            
            wr.WriteStartElement("Root");
            wr.WriteStartElement("Header");
            wr.WriteElementString("Author", "");
            wr.WriteElementString("Date", DateTime.Now.ToString());
            wr.WriteElementString("Setpoints", nSetpoints.ToString());
            wr.WriteElementString("Safepoints", sSafePoints);
            string sr;
            string xmlComment="";
            foreach (string s in Comment)            
            {
                // Check for commas
                sr = s;
                do
                    {
                    if(sr.Contains(","))
                       sr=sr.Remove(sr.IndexOf(','), 1);
                    }
                while (sr.Contains(","));

                xmlComment=xmlComment+sr+",";
            }

            if (xmlComment.Length > 0)
            {
                xmlComment = xmlComment.Substring(0, xmlComment.Length - 1);
            }
            else
            {
                xmlComment = ",";
            }

            if (xmlComment.Equals(""))
            {
                xmlComment = ",";
            }
            wr.WriteElementString("Comments",xmlComment);
            wr.WriteElementString("MCD", strMCD);
            wr.WriteEndElement();
            //-------------------------------------------------            
            // Liimits
            //-------------------------------------------------            
            sValue = "";
            wr.WriteStartElement("Limits");
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (dataGridView1.Columns[i].Tag.ToString().Equals("Limit_Min"))
                {
                    sValue = "";
                    string sName = dataGridView1.Columns[i].HeaderText;
                    wr.WriteStartElement("Limit");
                    wr.WriteElementString("Name", sName.Substring(0, sName.Length - 4));
                    wr.WriteElementString("Description", Description[i]);
                    wr.WriteElementString("Action", LimitsAction[i]);
                    wr.WriteElementString("Stability", StabMax[i]);
                    
                    //  wr.WriteElementString("Unit", Units[i]);
                    for (int j = 0; j < dataGridView1.RowCount ; j++)
                    {
                        sValue = string.Concat(sValue, dataGridView1.Rows[j].Cells[i].Value.ToString());
                        sName = dataGridView1.Columns[i].HeaderText;

                        if (j < dataGridView1.RowCount - 1)
                        {
                            sValue = string.Concat(sValue, ",");
                        }
                    }
                    wr.WriteElementString("Min", sValue);
                }

                if (dataGridView1.Columns[i].Tag.ToString().Equals("Limit_Max"))
                {
                    sValue = "";
                    string sName = dataGridView1.Columns[i].HeaderText;
                    for (int j = 0; j < dataGridView1.RowCount-1 ; j++)
                    {
                        sValue = string.Concat(sValue, dataGridView1.Rows[j].Cells[i].Value.ToString());
                        if (j < dataGridView1.RowCount - 1)
                        {
                            sValue = string.Concat(sValue, ",");
                        }
                    }
                    wr.WriteElementString("Max", sValue);
                    wr.WriteEndElement();
                }
            }
            wr.WriteEndElement();

            //-------------------------------------------------            
            // Actuators
            //-------------------------------------------------            
            wr.WriteStartElement("Actuators");
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (dataGridView1.Columns[i].Tag.ToString().Equals("Actuator") || dataGridView1.Columns[i].Tag.ToString().Equals("Required"))
                {
                    sValue = "";
                    wr.WriteStartElement("Actuator");
                    wr.WriteElementString("Required", Required[i]);
                    wr.WriteElementString("Name", dataGridView1.Columns[i].HeaderText);
                    wr.WriteElementString("Description", Description[i]);
                    wr.WriteElementString("Unit", Units[i]);
                    wr.WriteElementString("Transition", Transition[i].ToString());
                    wr.WriteElementString("TransVal", TransitionValue[i].ToString());

                    for (int j = 0; j < dataGridView1.RowCount ; j++)
                    {
                            if (dataGridView1.Rows[j].Cells[i].Value==null )
                            {
                                // if(dataGridView1.Rows[j].Cells[i].Value.ToString() == String.Empty)
                                {

                                    dataGridView1.Rows[j].Cells[i].Value = "0";
                                }
                            }


                            sValue = string.Concat(sValue, dataGridView1.Rows[j].Cells[i].Value.ToString());
                            if (j < dataGridView1.RowCount - 1)
                            {
                                {
                                    sValue = string.Concat(sValue, ",");
                                }
                        }
                    }
                    wr.WriteElementString("Val", sValue);
                    wr.WriteEndElement();
                }
            }
            wr.WriteEndElement();
            wr.WriteEndElement();
            wr.WriteEndDocument();
            wr.Flush();
            wr.Close();
            Cursor.Current = Cursors.Default;
            DataGridChanged = false;
            MCDChanged = false;
        }

        private void updateRecentFiles(string sFile)
        {
            // Save to RecentFiles buffer
            string s = Properties.Settings.Default.RecentFiles;
            string[] sp = s.Split(',');
            Array.Resize(ref sp, 5);
            if (!sp.Contains(sProjectName))
            {
                for (int i = 4; i > 0; i--)
                {
                    sp[i] = sp[i - 1];

                }
                sp[0] = sFile;
                Properties.Settings.Default.RecentFiles = string.Join(",", sp);
                Properties.Settings.Default.Save();
            }
            GetRecentFileList();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
                if (!CheckDataChanged())
                {
                    return;
                }
                openFileDialog1.Filter = "TXPL Files (*.tpxl)|*.tpxl|All files (*.*)|*.*";
                openFileDialog1.Title = "Select Test Plan to open";
                openFileDialog1.FileName = "";
                openFileDialog1.InitialDirectory = Properties.Settings.Default.TPXL_Root;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    sProjectName = openFileDialog1.FileName;
                    if(OpenXML(sProjectName))
                    { 
                    toolStrip1.Items[2].Text = "MCD File:" + strMCD;
                    updateRecentFiles(sProjectName);
                    CheckMCD();

                    }
                    GetExtraData();

                }
                else
                {
                    return;
                }
            }


        private bool CheckDataChanged()
           {
            bool aok = true;
            // Datagrid
            if(DataGridChanged.Equals(true)||MCDChanged.Equals(true))
            {
                var x=MessageBox.Show("Changes have been made without saving.\r Save changes now?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (x.Equals(DialogResult.Yes))
                {
                       saveAsToolStripMenuItem_Click(null, null);
                       aok = true;
                }
                else
                {
                    aok = true;
                    MCDChanged = false;
                    DataGridChanged = false;
                }
            }
            return aok;
        }
   
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckDataChanged())
            {
                this.Close();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckDataChanged())
            {
               // return;
            }

            sProjectName = "";

            // Clear DataGridView
            int n = dataGridView1.Columns.Count ;
            for (int i = 0; i < n; i++)
			{
                dataGridView1.Columns.RemoveAt(0);
			}
            //dataGridView1.RowHeadersWidth = 50;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.RowHeadersDefaultCellStyle.Format = "0.0";
            SafePoint.Clear();
            Transition.Clear();
            Required.Clear();
            TransitionValue.Clear();
            Units.Clear();
            Description.Clear();
           // GraphYaxis.Clear();
            LimitsAction.Clear();
            Required.Clear();
            Comment.Clear();
            // Get MCD File
            attachMCDFileToolStripMenuItem_Click(null, null);
            GetExtraData();
            //----------------------------------------------------------------------------------------------------------
            //Required Columns
            //----------------------------------------------------------------------------------------------------------
            
            // Name
            DataGridViewColumn dc0 = new DataGridViewColumn();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            dc0.CellTemplate = cell;
            dc0.HeaderText = "M_AL.SUBTESTNUM";
            dc0.Tag = "Required";
            dc0.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc0);
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Subtest number");
            Units.Add("-");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");
            // Name
            DataGridViewColumn dc = new DataGridViewColumn();
            dc.CellTemplate = cell;
            dc.HeaderText = "S_EC.SPEED";
            dc.Tag = "Required";
            dc.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc);
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Set Engine Speed");
            Units.Add("RPM");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");
            //----------------------------------------------------------------------------------------------------------
            DataGridViewColumn dc1 = new DataGridViewColumn();
            dc1.CellTemplate = cell;
            dc1.HeaderText = "S_EC.TORQUE";
            dc1.Tag = "Required";
            dc1.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc1);
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Set Engine Torque");
            Units.Add("kPA");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");
            //----------------------------------------------------------------------------------------------------------
            DataGridViewColumn dc2 = new DataGridViewColumn();
            dc2.CellTemplate = cell;
            dc2.HeaderText = "S_EC.THROTTLE";
            dc2.Tag = "Required";
            dc2.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc2);
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Throttle Position");
            Units.Add("%");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");

            //----------------------------------------------------------------------------------------------------------
            DataGridViewColumn dc3 = new DataGridViewColumn();
            dc3.CellTemplate = cell;
            dc3.HeaderText = "DYNO_MODE";
            dc3.Tag = "Required";
            dc3.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc3);
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Dyno Mode-Pair");
            Units.Add("%");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");

            //----------------------------------------------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------------------
            DataGridViewColumn dc4 = new DataGridViewColumn();
            dc4.CellTemplate = cell;
            dc4.HeaderText = "MIN_ST";
            dc4.Tag = "Required";
            dc4.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc4);
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Min Stabilization time");
            Units.Add("S");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");

            //----------------------------------------------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------------------
            DataGridViewColumn dc5 = new DataGridViewColumn();
            dc5.CellTemplate = cell;
            dc5.HeaderText = "MAX_ST";
            dc5.Tag = "Required";
            dc5.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc5);
            dataGridView1.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Max Stabilization time");
            Units.Add("S");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");

            //----------------------------------------------------------------------------------------------------------
            // AVG_TIME 
            //----------------------------------------------------------------------------------------------------------
            DataGridViewColumn dc6 = new DataGridViewColumn();
            dc6.CellTemplate = cell;
            dc6.HeaderText = "STEP_TIME";
            dc6.Tag = "Required";
            dc6.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc6);
            dataGridView1.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Step Ramp Time");
            Units.Add("-");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");

            //----------------------------------------------------------------------------------------------------------
            // MEASURE
            //----------------------------------------------------------------------------------------------------------
            DataGridViewColumn dc7 = new DataGridViewColumn();
            dc7.CellTemplate = cell;
            dc7.HeaderText = "MEASURE";
            dc7.Tag = "Required";
            dc7.DefaultCellStyle.BackColor = Color.PowderBlue;
            dataGridView1.Columns.Add(dc7);
            dataGridView1.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            Description.Add("Take Measurement");
            Units.Add("-");
            TransitionValue.Add("0");
            Transition.Add("By Row");
            LimitsAction.Add("Reverse");
            Required.Add("1");
            StabMax.Add("0");

            // Comment
            Comment.Add("");
            //----------------------------------------------------------------------------------------------------------
            // fill in rows
            //----------------------------------------------------------------------------------------------------------
            dataGridView1.Rows.Insert(0);
            dataGridView1.Rows[0].HeaderCell.Value = (1).ToString();
            double z = 0;
            for (int c = 0; c < dataGridView1.ColumnCount; c++)
            {
                if (c.Equals(4))
                {

                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.Dyno.ToString();

                }

                else if (c.Equals(5))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.MinST.ToString();
                }
                else if (c.Equals(6))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.MaxST.ToString();
                }
                else if (c.Equals(7))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.StepTime.ToString();
                }
                else if (c.Equals(8))
                {
                    dataGridView1.Rows[0].Cells[c].Value = Properties.Settings.Default.Measure.ToString();
                }
                else if (!dataGridView1.Columns[c].Tag.Equals("Limit_Min") && !dataGridView1.Columns[c].Tag.Equals("Limit_Max"))
                {
                    dataGridView1.Rows[0].Cells[c].Value = "0.0";
                }
            }

        }
        private void AddMenuItems()
        {
            
            //----------------------------------------------------------------------------
            // DataGridView1 Context Menu
            //----------------------------------------------------------------------------
            // Header
            contextMenuForHeader.Items.Add("Edit Actuator");
            contextMenuForHeader.Items.Add("Add Actuator");
            contextMenuForHeader.Items.Add("Add Limit");
            contextMenuForHeader.Items.Add("Edit Limit");
            contextMenuForHeader.Items.Add("Remove Actuator");
            contextMenuForHeader.Items.Add("Remove Limit");

            //contextMenuForHeader.Items.Add("Add to Graph");
            contextMenuForHeader.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuForHeader_ItemClicked);
            // Row Header
            contextMenuForRowHeader.Items.Add("Delete Row(s)");
            contextMenuForRowHeader.Items.Add("Add/Remove Comment");
            contextMenuForRowHeader.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuForRowHeader_ItemClicked);




            
            // Actuators
            contextMenuForActuators.Items.Add("Toggle Safepoint");
            contextMenuForActuators.Items.Add("Insert Row");
            contextMenuForActuators.Items.Add("Insert Row(s)");
            contextMenuForActuators.Items.Add("Fill Column");
            contextMenuForActuators.Items.Add("Copy");
            contextMenuForActuators.Items.Add("Paste");
            contextMenuForActuators.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuForActuators_ItemClicked);


            
            // Rows
            contextMenuForRows.Items.Add("Rows");

        }
        private int GetLastRequired()
        {
            int nCount = 0;
            foreach(string s in Required)
            {
                if (s.Equals("1"))
                    nCount++;
            }
            return nCount;

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if(!sProjectName.Equals(""))
            {
                saveFileDialog1.FileName=sProjectName;
            }

            saveFileDialog1.Title = "Save Test Plan as:";
            saveFileDialog1.Filter = "TXPL Files (*.tpxl)|*.tpxl|All files (*.*)|*.*";
            saveFileDialog1.InitialDirectory = Properties.Settings.Default.TPXL_Root;
            if(saveFileDialog1.ShowDialog()==DialogResult.OK)
            {
                sProjectName = saveFileDialog1.FileName;
                saveToolStripMenuItem_Click_1(null, null);
                updateRecentFiles(sProjectName);
            }
            DataGridChanged=false;
            MCDChanged = false;
            toolStrip1.Items[0].Text = "TPXFile:" + sProjectName;


        }


        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            DataGridView dv = (DataGridView)sender;
            int n1 = 0;
            if(e.Modifiers.Equals(Keys.Control))
            {
                if (e.KeyValue.Equals(86))
                {
                    try
                    {
                        int iRow = dv.CurrentCell.RowIndex;
                        int iCol = dv.CurrentCell.ColumnIndex;
                        int lastRow = dv.RowCount;
                        string s = Clipboard.GetText();
                        string[] lines = s.Split('\n');
                        // Check for enough rows to accept
                        if(lines.Count()+iRow>dv.RowCount)
                        {
                            int n = lines.Count() - dv.RowCount + iRow;
                            var x =MessageBox.Show("Not enough rows to accept Clipboard data\rDo you want to add these rows?","Warning",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Asterisk);
                            if(x.Equals(DialogResult.Yes))
                            {
                                dataGridView1.Rows.AddCopies(lastRow - 1, n);
                                for (int i = lastRow-1; i < lastRow+n-1; i++)
                                {
                                    for (int d = 0; d < dataGridView1.ColumnCount; d++)
                                    {
                                        dataGridView1.Rows[ i].Cells[d].Value = dataGridView1.Rows[lastRow- 2].Cells[d].Value;
                                    }
                                }
                            }
                            // Only fill in rows available
                            if (x.Equals(DialogResult.No))
                            {
                                n1 = 1;
                           
                            }
                            // Exit with no action
                            if (x.Equals(DialogResult.Cancel))
                            {
                                return;
                            }
                        }
                        DataGridViewCell oCell;
                        foreach (string line in lines)
                        {
                            if (iRow+n1 < dv.RowCount && line.Length > 0)
                            {
                                string[] sCells = line.Split('\t');
                                for (int i = 0; i < sCells.GetLength(0); ++i)
                                {
                                    if (iCol + i < dv.ColumnCount)
                                    {
                                        oCell = dv[iCol + i, iRow];
                                            if (oCell.Value.ToString() != sCells[i])
                                            {
                                                oCell.Value = Convert.ChangeType(sCells[i],
                                                                      oCell.ValueType);
                                            }
                                    }
                                    else
                                    { break; }
                                }
                                iRow++;
                            }
                            else
                            { break; }
                        }
                    }

                    catch (FormatException)
                    {
                        MessageBox.Show("The data you pasted is in the wrong format for the cell");
                        return;
                    }
                }

            }
        }

        private void attachMCDFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var fbd = new FolderBrowserDialog();
            fbd.Description="Select MCD file to attach";
            fbd.SelectedPath = Properties.Settings.Default.MCD_Root.ToString();
            fbd.ShowNewFolderButton = false;

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                if (!strMCD.Equals(openFileDialog1.FileName))
                {
                    MCDChanged = true;
                }
                strMCD = fbd.SelectedPath;
                toolStrip1.Items[2].Text = "MCD File:" + strMCD;
                CheckMCD();
            }

        }
        private void CheckMCD()
        {
            List<string> lc = new List<string>();
            List<string> la2l = new List<string>();
           // try
            //{
                {
                    // Check default MCD channel file to make sure that Channel ID and IdA2L are consistant with what is stored in the actuators.
                    //toolStrip1.Items[2].Text = "MCD File:" + strMCD;
                    //Validate any existing actuators
                    XmlDocument xd = new XmlDocument();
                    string fName = strMCD; 
                    if (!strMCD.Contains("MCSystem.xmcd"))
                    {
                        fName = strMCD + "\\MCSystem.xmcd";
                    }
                    try { xd.Load(fName); }
                    catch
                    {
                        var x = MessageBox.Show(fName, "MCD File not found...");
                        return;
                   
                    }
                        XmlNode Header = xd.SelectSingleNode("//Parameters");
                        try
                        {
                            foreach (XmlNode hdr in Header.ChildNodes)
                        {
                            if (hdr.Name.Equals("Parameter"))
                            {
                                lc.Add(hdr.Attributes.GetNamedItem("IdMorphee").Value);
                                la2l.Add(hdr.Attributes.GetNamedItem("IdA2L").Value);
                            }
                        }
                    }
                    catch
                    {

                    }

                    // Check supplimental file to make sure that Channel ID and IdA2L are consistant with what is stored in the actuators.
                    fName = Properties.Settings.Default.ExtDataRoot;
                    try { xd.Load(fName); }
                    catch
                    { }
                    Header = xd.SelectSingleNode("//quantities");
                    foreach (XmlNode hdr in Header.ChildNodes)
                    {
                        if (hdr.Name.Equals("player"))
                        {
                            lc.Add(hdr.ChildNodes[0].InnerXml);
                            la2l.Add(hdr.ChildNodes[1].InnerXml);
                        }
                    }

                    // Compare
                    for (int c = 9; c < dataGridView1.ColumnCount; c++)
                    {
                        if (dataGridView1.Columns[c].Tag.Equals("Actuator"))
                        {
                            string laname = dataGridView1.Columns[c].HeaderText;
                            string lattt = dataGridView1.Columns[c].HeaderCell.ToolTipText;
                            int lcindex = lc.IndexOf(laname);
                            if (lcindex.Equals(-1))
                            {
                                string msg = "Error:[" + laname + "] not found in MCD or Supplimental file. Channel might not exist Choose another MCD or remove " + laname + " from list";
                                var ok = MessageBox.Show(msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                if (!la2l[lcindex].Equals(lattt))
                                {
                                    string msg = lc[lcindex] + " Channel[" + lattt + "] not found \r Replace with [" + la2l[lcindex] + "]?";
                                    DialogResult ok = MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (ok == DialogResult.Yes)
                                    {
                                        dataGridView1.Columns[c].HeaderCell.ToolTipText = la2l[lcindex];
                                        Description[c] = la2l[lcindex];
                                    }
                                }
                            }
                        }
                    }
                }
            //}
            //catch
            //{ }
        }
        private bool OpenXML(string sFilename)
        {

            Cursor.Current = Cursors.WaitCursor;
            sProjectName = sFilename;
            toolStrip1.Items[0].Text ="TPXFile:"+ sFilename;
            string sName = "";
            int nCols = 0;
            int nRows = 0;
            int nMaxRows = 0;
            Properties.Settings.Default.TPXL_Root = Path.GetDirectoryName(sFilename);
            Properties.Settings.Default.Save();
            //this.AddMenuItems();
            //----------------------------------------------------------------------------
            // DataGridView1 Format
            //----------------------------------------------------------------------------
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.RowHeadersDefaultCellStyle.Format = "0.0";
            dataGridView1.Columns.Clear();
            SafePoint.Clear();
            Transition.Clear();
            Required.Clear();
            TransitionValue.Clear();
            Units.Clear();
            Description.Clear();
            // GraphYaxis.Clear();
            LimitsAction.Clear();
            Required.Clear();
            Comment.Clear();
            //----------------------------------------------------------------------------
            // Get header information
            //----------------------------------------------------------------------------
            //XmlDocument xd = new XmlDocument();
            ////            xd.Load("C:\\temp\\alstest.xml");
            //try
            //{ xd.Load(sFilename); }
            //catch
            //{
            //    MessageBox.Show("Error wile opening file...", "Error");
            //    return false; }
            DataRow newRow;
            DataTable dataTable;
            DataTable actTable;
            DataTable limitTable;
            Microsoft.Office.Interop.Excel.Application oExcel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                Microsoft.Office.Interop.Excel.Workbook WB = oExcel.Workbooks.Open(sFilename);
                if (WB.Sheets.Count < 3)
                {
                    oExcel.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                }
                Microsoft.Office.Interop.Excel.Worksheet data = (Microsoft.Office.Interop.Excel.Worksheet)WB.Worksheets[1];
                Microsoft.Office.Interop.Excel.Worksheet actuators = (Microsoft.Office.Interop.Excel.Worksheet)WB.Worksheets[2];
                Microsoft.Office.Interop.Excel.Worksheet limits = (Microsoft.Office.Interop.Excel.Worksheet)WB.Worksheets[3];

                int dataRows = data.UsedRange.Rows.Count;
                int dataCols = data.UsedRange.Columns.Count;

                int actRows = actuators.UsedRange.Rows.Count;
                int actCols = actuators.UsedRange.Columns.Count;

                int limitRows = limits.UsedRange.Rows.Count;
                int limitCols = limits.UsedRange.Columns.Count;

                dataTable = new DataTable("data_table");
                actTable = new DataTable("act_table");
                limitTable = new DataTable("limit_table");

                for (int i = 0; i < dataCols; i++)
                {
                    dataTable.Columns.Add(((Microsoft.Office.Interop.Excel.Range)data.Cells[i, 1]).Value.ToString());
                }
            }
            catch (FileNotFoundException e)
            {

            }

            XmlNode Header = xd.SelectSingleNode("//Header");
            foreach (XmlNode hdr in Header.ChildNodes)
            {
                if (hdr.Name.Equals("Setpoints"))
                {
                    nMaxRows = int.Parse(hdr.InnerText.ToString());
                }
                if (hdr.Name.Equals("Safepoints"))
                {
                    sName = hdr.FirstChild.InnerText.ToString();
                    string[] sSafePoint = sName.Split(',');
                    foreach (string s in sSafePoint)
                    {
                        SafePoint.Add(s);
                    }
                }
                if (hdr.Name.Equals("MCD"))
                {
                    strMCD = hdr.InnerText.ToString();
                    // Check for exrtension
                    if (!strMCD.Contains(".xmcd"))
                    {
                        //strMCD=strMCD + ".xmcd";
                    }
                }
                if (hdr.Name.Equals("Comments"))
                {
                    sName = hdr.FirstChild.InnerText.ToString();
                    string[] sSafePoint = sName.Split(',');
                    foreach (string s in sSafePoint)
                    {
                        Comment.Add(s);
                    }
                    
                }



            }
            XmlNode Actuator = xd.SelectSingleNode("//Actuators");
            //----------------------------------------------------------------------------
            // Create x columns
            //----------------------------------------------------------------------------
            foreach (XmlNode act in Actuator.ChildNodes)
            {
                dataGridView1.Columns.Add("", "");
            }



            //----------------------------------------------------------------------------
            //1st 4 rows 
            //----------------------------------------------------------------------------
            foreach (XmlNode act in Actuator.ChildNodes)
            {
                if (act.Name.Equals("Actuator"))
                {
                    nRows = 0;
                    foreach (XmlNode tor in act.ChildNodes)
                    {
                        // required
                        if (tor.Name.Equals("Required"))
                        {
                            try
                            {
                                sName = tor.FirstChild.InnerText;
                            }
                            catch
                            {
                                sName = "";
                            }
                            if (sName.Equals("1"))
                            {
                                dataGridView1.Columns[nCols].DefaultCellStyle.BackColor = Color.PowderBlue;
                                Required.Add("1");
                            }
                            else
                            {
                                Required.Add("0");

                            }

                        }
                        // Name
                        if (tor.Name.Equals("Name"))
                        {
                            try
                            {
                                sName = tor.FirstChild.InnerText;
                            }
                            catch
                            {
                                sName = "";
                            }
                            dataGridView1.Columns[nCols].HeaderText = sName;
                            dataGridView1.Columns[nCols].Tag = "Actuator";
                            dataGridView1.Columns[nCols].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        // Description
                        if (tor.Name.Equals("Description"))
                        {
                            try
                            {
                                sName = tor.FirstChild.InnerText;
                            }
                            catch
                            {
                                sName = "";
                            }
                            Description.Add(sName);
                            LimitsAction.Add("Reverse");
                            dataGridView1.Columns[nCols].ToolTipText = sName;
                            StabMax.Add("0");
                        }
                        // Units
                        try
                        {
                            if (tor.Name.Equals("Unit"))
                            {
                                sName = tor.FirstChild.InnerText;
                                Units.Add(sName);
                            }
                        }
                        catch
                        { }

                        ///  <TransVal>
                        if (tor.Name.Equals("TransVal"))
                        {
                            try
                            {
                                sName = tor.FirstChild.InnerText;
                            }
                            catch
                            {
                                sName = "";
                            }
                            TransitionValue.Add(sName);
                        }
                        // Slope
                        if (tor.Name.Equals("Transition"))
                        {
                            try
                            {
                                sName = tor.FirstChild.InnerText;
                            }
                            catch
                            {
                                sName = "";
                            }
                            Transition.Add(sName);
                        }
                        

                    }
                    nCols++;
                }
            }
            //----------------------------------------------------------------------------
            //Remaining rows 
            //----------------------------------------------------------------------------
            dataGridView1.Rows.Add(nMaxRows);

            // Check that comments were loaded
            if (!Comment.Count.Equals(nMaxRows))
            {
                for (int i = 0; i < nMaxRows; i++)
                {
                    Comment.Add("");
                }
            }



            nCols = 0;
            foreach (XmlNode act in Actuator.ChildNodes)
            {
                if (act.Name.Equals("Actuator"))
                {
                    foreach (XmlNode tor in act.ChildNodes)
                    {
                        // Name
                        if (tor.Name.Equals("Val"))
                        {
                            try
                            {
                                sName = tor.FirstChild.InnerText.ToString();
                            }
                            catch
                            {
                                sName = "";
                            }
                            string[] sCell = sName.Split(',');
                            foreach (string s in sCell)
                            {
                                dataGridView1.Rows[nRows].Cells[nCols].Value = double.Parse(s);
                                // Update Row header
                                int n = nRows + 1;
                                nRows++;
                            }
                        }
                    }

                    nCols++;
                }
                nRows = 0;
            }
            //----------------------------------------------------------------------------
            //Limits
            //----------------------------------------------------------------------------
            //----------------------------------------------------------------------------
            // Get header information
            //----------------------------------------------------------------------------
            XmlNode Limit = xd.SelectSingleNode("//Limits");
            nRows = 0;
            nCols = 0;
            string sColumnName = "";
            foreach (XmlNode hdr in Limit.ChildNodes)
            {
                if (hdr.Name.Equals("Limit"))
                    foreach (XmlNode lmt in hdr.ChildNodes)
                    {
                        if (lmt.Name.Equals("Name"))
                        {
                            sColumnName = lmt.FirstChild.InnerText;
                        }
                        if (lmt.Name.Equals("Description"))
                        {
                            //two times one for low and one for high
                            Description.Add(lmt.FirstChild.InnerText);
                            Description.Add(lmt.FirstChild.InnerText);
                        }
                        if (lmt.Name.Equals("Min"))
                        {
                            //Low
                            DataGridViewColumn dc = new DataGridViewColumn();
                            DataGridViewCell cell = new DataGridViewTextBoxCell();
                            dc.CellTemplate = cell;
                            dc.HeaderText = "";
                            dc.HeaderText = sColumnName + "_Low";
                            dc.DefaultCellStyle.BackColor = Color.Wheat;
                            dc.Tag = "Limit_Min";
                            dataGridView1.Columns.Add(dc);
                            Transition.Add("By Row");
                            TransitionValue.Add("0");
                            Units.Add("");
                            try
                            { sName = lmt.FirstChild.InnerText; }
                            catch
                            { sName = ""; }
                            nRows = 0;
                            string[] sCell = sName.Split(',');
                            foreach (string s in sCell)
                            {
                                try
                                {
                                    if (!s.Equals(""))
                                    {
                                        dataGridView1.Rows[nRows].Cells[dataGridView1.Columns.Count - 1].Value = double.Parse(s);
                                        nRows++;
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[nRows].Cells[dataGridView1.Columns.Count - 1].Value = "";
                                        nRows++;
                                    }
                                }
                                catch
                                {
                                    dataGridView1.Rows[nRows].Cells[dataGridView1.Columns.Count - 1].Value = "";
                                    nRows++;

                                }
                            }
                        }
                        if (lmt.Name.Equals("Max"))
                        {
                            //High
                            DataGridViewColumn dc1 = new DataGridViewColumn();
                            DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                            dc1.CellTemplate = cell1;
                            dc1.HeaderText = "";
                            dc1.HeaderText = sColumnName + "_High";
                            dc1.DefaultCellStyle.BackColor = Color.Wheat;
                            dc1.Tag = "Limit_Max";
                            dataGridView1.Columns.Add(dc1);
                            Transition.Add("By Row");
                            TransitionValue.Add("0");
                            Units.Add("");
                            try
                            { sName = lmt.FirstChild.InnerText; }
                            catch
                            { sName = ""; }
                            nRows = 0;
                            string[] sCell = sName.Split(',');
                            foreach (string s in sCell)
                            {
                                try
                                {
                                    if (!s.Equals(""))
                                    {
                                        dataGridView1.Rows[nRows].Cells[dataGridView1.Columns.Count - 1].Value = double.Parse(s);
                                        nRows++;
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[nRows].Cells[dataGridView1.Columns.Count - 1].Value = "";
                                    }
                                }
                                catch
                                {
                                    dataGridView1.Rows[nRows].Cells[dataGridView1.Columns.Count - 1].Value = "";
                                    nRows++;

                                }
                            }
                        }
                        if (lmt.Name.Equals("Action"))
                        {
                            // Two time one for low and one for high
                            LimitsAction.Add(lmt.FirstChild.InnerText);
                            LimitsAction.Add(lmt.FirstChild.InnerText);
                        }
                        if (lmt.Name.Equals("Stability"))
                        {
                            // Two time one for low and one for high
                            StabMax.Add(lmt.FirstChild.InnerText);
                            StabMax.Add(lmt.FirstChild.InnerText);
                        }

                    }
            }
            int z1 = 1;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {

                if (SafePoint.Contains(z1.ToString()))
                for (int i1 = 0; i1 < dataGridView1.Columns.Count; i1++)
                {
                    dataGridView1.Rows[z1-1].Cells[i1].Style.BackColor = Color.LightGreen;
                }
                z1++;
            }
            Renumber();
            Cursor.Current = Cursors.Default;




            DataGridChanged = false;
            MCDChanged = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Refresh();
            return true;
            //// Check that comments were loaded
            //if (!Comment.Count.Equals(nRows))
            //{
            //    for (int i = 0; i < nRows; i++)
            //    {
            //        Comment.Add("");
            //    }
            //}


        }

        private void Addnewactuator(string NewActuator, int x, int y)
        {

            int nColumn = GetLastRequired() + CountActuators();
            FormSetpoint fs = new FormSetpoint();
            // Transfer player list
            fs.xdataPlayer = ExtraDataPlayer;
            fs.strMCD = strMCD;
            fs.comboBox1.Text = "";
            fs.textBoxDescription.Text = "";
            fs.Show(dataGridView1);
            fs.textBoxVal.Text = "0";
            fs.comboBox1.Text = NewActuator;

            int x1 =  this.Left+(this.Width/2);
            int z =  this.Top + 100;


            fs.SetDesktopLocation(x1,z);

            { fs.radioButton4.Select(); }
                bool fsf = false;
                do
                {
                    Application.DoEvents();
                    fsf = fs.finished;
                } while (fsf.Equals(false));
            DataGridViewColumn dc = new DataGridViewColumn();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            dc.CellTemplate = cell;
            dc.HeaderText = "";
            dc.Tag = "Actuator";
           
            dataGridView1.Columns.Insert(nColumn, dc);
            dataGridView1.Columns[nColumn].ToolTipText = fs.textBoxDescription.Text;
            dataGridView1.Columns[nColumn].HeaderText = fs.comboBox1.Text;
            TransitionValue.Insert(nColumn, fs.textBoxVal.Text);
            if (fs.radioButton1.Checked)
            { Transition.Insert(nColumn, "Slope"); }
            if (fs.radioButton2.Checked)
            { Transition.Insert(nColumn, "Gradient"); }
            if (fs.radioButton3.Checked)
            { Transition.Insert(nColumn, "Step"); }
            if (fs.radioButton4.Checked)
            { Transition.Insert(nColumn, "By Row"); }

            if (fs.textBoxVal.Text == "") { fs.textBoxVal.Text = "0.0"; }
            for (int i = 0; i < dataGridView1.RowCount ; i++)
            {
                dataGridView1.Rows[i].Cells[nColumn].Value = fs.textBoxVal.Text;
            }
            TransitionValue.Insert(nColumn, "0");
            Description.Insert(nColumn, fs.textBoxDescription.Text);
            Units.Insert(nColumn, fs.textBoxUnit.Text);
            LimitsAction.Insert(nColumn, "By Row");

            StabMax.Insert(nColumn, "0");

            Required.Insert(nColumn, "0");
            fs.Dispose();
        }
        private void AddLimits(string NewLimit)
        {
            bool b = false;
            bool c = false;
            FormLimits fl = new FormLimits();
            fl.xdataWatcher = ExtraDataWatcher;

            fl.Show();

            if (!NewLimit.Equals(""));
            {
                fl.comboBox1.Text = NewLimit;
                fl.textBoxDescription.Text = NewLimit;
            }
            int x1 = this.Left + (this.Width / 2);
            int z = this.Top + 100;

            fl.SetDesktopLocation(x1, z);

            do
            {
                Application.DoEvents();
                b = fl.b_OK;
                c = fl.b_Cancel;
            } while (b.Equals(false) && c.Equals(false));
            fl.Hide();
            if (c.Equals(false))
            {
                DataGridViewColumn dc4 = new DataGridViewColumn();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                dc4.CellTemplate = cell;
                dc4.HeaderText = fl.comboBox1.Text + "_Low";
                dc4.Tag = "Limit_Min";
                dc4.DefaultCellStyle.BackColor = Color.Wheat;
                dc4.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns.Add(dc4);
                Transition.Add("By Row");
                TransitionValue.Add("0");
                Description.Add(fl.textBoxDescription.Text);
                Units.Add("deg.C");
                LimitsAction.Add(fl.comboBoxAction.Text);
                StabMax.Add(fl.textBoxMaxDev.Text);
                DataGridViewColumn dc5 = new DataGridViewColumn();
                dc5.CellTemplate = cell;
                dc5.HeaderText = fl.comboBox1.Text + "_High";
                dc5.Tag = "Limit_Max";
                dc5.DefaultCellStyle.BackColor = Color.Wheat;
                dc5.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns.Add(dc5);
                Transition.Add("Step");
                TransitionValue.Add("0");
                Description.Add("low limit");
                Units.Add("deg.C");
                LimitsAction.Add(fl.comboBoxAction.Text);
                StabMax.Add(fl.textBoxMaxDev.Text);
                for (int d = 0; d < dataGridView1.RowCount ; d++)
                {
                    dataGridView1.Rows[d].Cells[dataGridView1.Columns.Count - 1].Value = fl.textBoxMax.Text;
                    dataGridView1.Rows[d].Cells[dataGridView1.Columns.Count - 2].Value = fl.textBoxMin.Text;
                }
            }
            fl.Dispose();
        }



        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
          // Get MCD file
          //  attachMCDFileToolStripMenuItem_Click(null, null);
         
            //Reset to prevet wanting to save later
            MCDChanged = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            newToolStripMenuItem_Click(null, null);
            List<string> limitsadded = new List<string>();
            List<string> sActs = new List<string>();
            string sss = strMCD;
            openFileDialog1.Filter = "TPL Files (*.tpl)|*.tpl|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.InitialDirectory = Properties.Settings.Default.ImportRoot;
            openFileDialog1.Title = "Select TPL file to import";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                GetExtraData();
                Properties.Settings.Default.ImportRoot = Path.GetDirectoryName(openFileDialog1.FileName);
                Properties.Settings.Default.Save();
                int ss1 = openFileDialog1.FileName.LastIndexOf("\\")+1;
                int ss2 = openFileDialog1.FileName.LastIndexOf(".");
                sProjectName = openFileDialog1.FileName.Substring(ss1, ss2-ss1);
                toolStrip1.Items[0].Text = "TPXFile:" + sProjectName;
                //----------------------------------------
                // Create new tpxl with default row
                //----------------------------------------
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                string sHeader = sr.ReadLine();
                string[] strHdr = sHeader.Split('\t');
                //Description
                sHeader = sr.ReadLine();
                string[] sDesc = sHeader.Split('\t');
                //Units
                sHeader = sr.ReadLine();
                string[] sUnits = sHeader.Split('\t');
                foreach (string s in strHdr)
                {
                    
                    // Create new actuators
                    if (s.StartsWith("M_"))
                    {
                        Addnewactuator(s, (dataGridView1.Width / 2), dataGridView1.Height / 2);
                        sActs.Add(s);
                    }
                    // Create Limits
                    else if (s.Contains("|") && (!limitsadded.Contains(s)))
                    {
                        AddLimits(s.Substring(0, s.IndexOf("|")));
                        limitsadded.Add(s);
                        sActs.Add(s.Substring(0, s.IndexOf("|")));

                    }
                    else
                    {
                        sActs.Add(s);
                    }
                }

                dataGridView1.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                //Remainder
                Int32 x=0;
                int nRow =0;
                while (!sr.EndOfStream)
                {
                    
                    sHeader = sr.ReadLine();
                    string[] sValues = sHeader.Split('\t');
                    if(Int32.TryParse(sValues[0],out x))
                    {
                        if(x>0)
                        {
                            if (nRow > 0)
                            {
                                // Add new row
                                dataGridView1.Rows.AddCopies(0, 1);

                            }
                            try
                            {

                              //  dataGridView1.Rows[nRow].HeaderCell.Value = (nRow+1).ToString();

                                dataGridView1.Rows[nRow].Cells[1].Value = sValues[sActs.IndexOf("ML_RPM_DEMAND")];
                                if (sValues[sActs.IndexOf("ML_SPD_THROT")].Equals("1"))
                                {
                                    dataGridView1.Rows[nRow].Cells[2].Value = "0";

                                    if (!sValues[sActs.IndexOf("ML_THROT_DEMAND")].Equals(""))
                                    {
                                        dataGridView1.Rows[nRow].Cells[3].Value = sValues[sActs.IndexOf("ML_THROT_DEMAND")];
                                        dataGridView1.Rows[nRow].Cells[4].Value = "1";
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[nRow].Cells[3].Value = "0";
                                    }
                                }
                                else if (sValues[sActs.IndexOf("ML_SPD_TORQ")].Equals("1"))
                                {
                                    dataGridView1.Rows[nRow].Cells[3].Value = "0";
                                    dataGridView1.Rows[nRow].Cells[2].Value = sValues[sActs.IndexOf("ML_TRQ_DEMAND")];
                                    dataGridView1.Rows[nRow].Cells[4].Value = "2";
                                }
                                dataGridView1.Rows[nRow].Cells[0].Value = "1";
                                dataGridView1.Rows[nRow].Cells[5].Value = sValues[sActs.IndexOf("Min_ST")];
                                dataGridView1.Rows[nRow].Cells[6].Value = sValues[sActs.IndexOf("Max_ST")];
                                dataGridView1.Rows[nRow].Cells[7].Value = sValues[sActs.IndexOf("Step_Time")];
                                dataGridView1.Rows[nRow].Cells[8].Value = sValues[sActs.IndexOf("Measure")];
                            }
                            catch
                            {
                                MessageBox.Show("Problem importing standard items like ML_RPM_DEMAND");
                                break;
                            }
                            //int x1 = GetLastRequired() + CountActuators()  ;
                            int x1 = dataGridView1.Columns.Count;
                            string s2use = "";
                            for (int i =9 ; i < x1; i++)
                            {
                                try
                                {
                                    if (dataGridView1.Columns[i].HeaderText.StartsWith("M_"))
                                    {
                                        s2use = sValues[sActs.IndexOf(dataGridView1.Columns[i].HeaderText)];
                                    }
                                    else if (dataGridView1.Columns[i].HeaderText.Contains("_Low"))
                                    {
                                        int nlow = dataGridView1.Columns[i].HeaderText.IndexOf("_Low");
                                        s2use = sValues[sActs.IndexOf(dataGridView1.Columns[i].HeaderText.Substring(0,nlow))];
                                        dataGridView1.Rows[nRow].Cells[i].Value = s2use;
                                        s2use = sValues[sActs.IndexOf(dataGridView1.Columns[i].HeaderText.Substring(0, nlow))+1];
                                        dataGridView1.Rows[nRow].Cells[i + 1].Value = s2use;
                                        i++;
                                        continue;
                                    }
                                    else if (dataGridView1.Columns[i].HeaderText.Contains("_High"))
                                    {
                                      //  s2use = sValues[sActs.IndexOf(dataGridView1.Columns[i].HeaderText.Substring(0, dataGridView1.Columns[i].HeaderText.IndexOf("_High")))];
                                    }
                                    else
                                    {
                                        s2use = sValues[sActs.IndexOf(dataGridView1.Columns[i].HeaderText)];
                                    }
                                    
                                }
                                catch
                                {
                                   s2use = "";
                                }
                                dataGridView1.Rows[nRow].Cells[i].Value = s2use;
                            }
                            Comment.Add("");

                            nRow++;
                        }
                    }
                }
                dataGridView1.Cursor = Cursors.Default;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                Renumber();

            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridChanged = true;
        }

        private void dataGridView1_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            DataGridChanged = true;
        }

        private void dataGridView1_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            DataGridChanged = true;
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            DataGridChanged = true;
        }

        private void dataGridView1_RowHeaderCellChanged(object sender, DataGridViewRowEventArgs e)
        {
            DataGridChanged = true;
        }

        private void checkMCDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckMCD();
        }

        private void viewSupplimentalFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = Properties.Settings.Default.ExtDataRoot;
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("Notepad.Exe");
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                procStartInfo.Arguments = fName;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                //string result = proc.StandardOutput.ReadToEnd();
                //Console.WriteLine(result);
            }
            catch (Exception objException)
            {
            }


        }

        private void dataGridView1_MouseUp_1(object sender, MouseEventArgs e)
        {
            // Load context menu on right mouse click
            DataGridView.HitTestInfo hitTestInfo;
            CellPoint.X = e.X;
            CellPoint.Y = e.Y;
            if (e.Button == MouseButtons.Right)
            {
                hitTestInfo = dataGridView1.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
                {
                    contextMenuForActuators.Show(dataGridView1, new Point(e.X, e.Y));
                }
                if (hitTestInfo.Type == DataGridViewHitTestType.RowHeader)
                {
                    contextMenuForRowHeader.Show(dataGridView1, new Point(e.X, e.Y));
                }
                if (hitTestInfo.Type == DataGridViewHitTestType.ColumnHeader)
                {
                    contextMenuForHeader.Show(dataGridView1, new Point(e.X, e.Y));
                }
            }
        }

        private void dataGridView1_RowPostPaint_1(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //   DataGridView s = (DataGridView)sender;

            //    //Convert the image to icon, in order to load it in the row header column
            //    Bitmap myBitmap = new Bitmap("C:\\Users\\ut0467h\\Source\\Repos\\ALS_Editor\\AutoLSedit\\Image1.png");
            //    Icon myIcon = Icon.FromHandle(myBitmap.GetHicon());

            Graphics graphics = e.Graphics;

            //Set Image dimension - User's choice
            int iconHeight = 6;
            int iconWidth = 6;

            //Set x/y position - As the center of the RowHeaderCell
            int xPosition = e.RowBounds.X + (dataGridView1.RowHeadersWidth) - iconWidth;
            int yPosition = e.RowBounds.Y +
            ((dataGridView1.Rows[e.RowIndex].Height - iconHeight) / 2) - 8;
            //Rectangle rectangle = new Rectangle(xPosition, yPosition, iconWidth, iconHeight);
            //-------------------------
            if (e.RowIndex < Comment.Count)
            {
                if (!Comment[e.RowIndex].Equals(""))
                {
                    PointF[] pnt = new PointF[3];

                    pnt[0].X = xPosition;
                    pnt[0].Y = yPosition;

                    pnt[1].X = xPosition + 5;
                    pnt[1].Y = yPosition;

                    pnt[2].X = xPosition + 5;
                    pnt[2].Y = yPosition + 5;

                    graphics.FillPolygon(Brushes.Red, pnt);
                    graphics.DrawPolygon(Pens.Red, pnt);
                }
            }


        }

        private void dataGridView1_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridChanged = true;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowsRemoved_1(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Renumber();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            dataGridView1.Dispose();
           
        }

    }
}
