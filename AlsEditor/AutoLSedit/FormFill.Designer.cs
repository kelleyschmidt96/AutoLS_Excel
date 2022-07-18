namespace AutoLSedit
{
    partial class FormFill
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxStart = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIncrement = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxRepeat = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxRows = new System.Windows.Forms.TextBox();
            this.checkBoxRepeat = new System.Windows.Forms.CheckBox();
            this.checkBoxMirror = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(35, 218);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "Ok";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(160, 218);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxStart
            // 
            this.textBoxStart.Location = new System.Drawing.Point(110, 70);
            this.textBoxStart.Name = "textBoxStart";
            this.textBoxStart.Size = new System.Drawing.Size(100, 20);
            this.textBoxStart.TabIndex = 1;
            this.textBoxStart.WordWrap = false;
            this.textBoxStart.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxStart_KeyPress);
            this.textBoxStart.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxStart_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(81, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Start:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxIncrement
            // 
            this.textBoxIncrement.Location = new System.Drawing.Point(110, 108);
            this.textBoxIncrement.Name = "textBoxIncrement";
            this.textBoxIncrement.Size = new System.Drawing.Size(100, 20);
            this.textBoxIncrement.TabIndex = 2;
            this.textBoxIncrement.WordWrap = false;
            this.textBoxIncrement.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxStop_KeyPress);
            this.textBoxIncrement.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxIncrement_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Increment:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxRepeat
            // 
            this.textBoxRepeat.Location = new System.Drawing.Point(110, 146);
            this.textBoxRepeat.Name = "textBoxRepeat";
            this.textBoxRepeat.Size = new System.Drawing.Size(45, 20);
            this.textBoxRepeat.TabIndex = 4;
            this.textBoxRepeat.TextChanged += new System.EventHandler(this.textBoxRepeat_TextChanged);
            this.textBoxRepeat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxRepeat_KeyDown);
            this.textBoxRepeat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxRepeat_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Number of rows:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxRows
            // 
            this.textBoxRows.Location = new System.Drawing.Point(110, 44);
            this.textBoxRows.Name = "textBoxRows";
            this.textBoxRows.Size = new System.Drawing.Size(100, 20);
            this.textBoxRows.TabIndex = 0;
            this.textBoxRows.WordWrap = false;
            this.textBoxRows.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxRows_KeyPress);
            this.textBoxRows.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxRows_KeyUp);
            // 
            // checkBoxRepeat
            // 
            this.checkBoxRepeat.AutoSize = true;
            this.checkBoxRepeat.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.checkBoxRepeat.Location = new System.Drawing.Point(36, 146);
            this.checkBoxRepeat.Name = "checkBoxRepeat";
            this.checkBoxRepeat.Size = new System.Drawing.Size(61, 17);
            this.checkBoxRepeat.TabIndex = 3;
            this.checkBoxRepeat.Text = "Repeat";
            this.checkBoxRepeat.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxRepeat.UseVisualStyleBackColor = true;
            // 
            // checkBoxMirror
            // 
            this.checkBoxMirror.AutoSize = true;
            this.checkBoxMirror.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.checkBoxMirror.Location = new System.Drawing.Point(7, 169);
            this.checkBoxMirror.Name = "checkBoxMirror";
            this.checkBoxMirror.Size = new System.Drawing.Size(90, 17);
            this.checkBoxMirror.TabIndex = 5;
            this.checkBoxMirror.Text = "Vertical Mirror";
            this.checkBoxMirror.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxMirror.UseVisualStyleBackColor = true;
            // 
            // FormFill
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.checkBoxMirror);
            this.Controls.Add(this.checkBoxRepeat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxRows);
            this.Controls.Add(this.textBoxRepeat);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIncrement);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxStart);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Name = "FormFill";
            this.Text = "FormFill";
            this.Activated += new System.EventHandler(this.FormFill_Activated);
            this.Load += new System.EventHandler(this.FormFill_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.TextBox textBoxStart;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBoxIncrement;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox textBoxRepeat;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox textBoxRows;
        public System.Windows.Forms.CheckBox checkBoxRepeat;
        public System.Windows.Forms.CheckBox checkBoxMirror;
    }
}