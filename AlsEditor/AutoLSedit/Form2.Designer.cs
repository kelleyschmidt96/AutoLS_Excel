namespace AutoLSedit
{
    partial class FormComment
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
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxComment
            // 
            this.textBoxComment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxComment.Location = new System.Drawing.Point(0, 0);
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(464, 20);
            this.textBoxComment.TabIndex = 0;
            this.textBoxComment.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxComment_KeyPress);
            this.textBoxComment.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxComment_KeyUp);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(187, 25);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // FormComment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 48);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxComment);
            this.MinimizeBox = false;
            this.Name = "FormComment";
            this.Text = "Comment:";
            this.Load += new System.EventHandler(this.FormComment_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormComment_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.Button buttonOk;
    }
}