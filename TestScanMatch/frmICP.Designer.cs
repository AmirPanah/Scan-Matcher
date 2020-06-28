namespace TestScanMatch
{
    partial class frmICP
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
            this.btnStep = new System.Windows.Forms.Button();
            this.llLogFile = new System.Windows.Forms.LinkLabel();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.btnInitialize = new System.Windows.Forms.Button();
            this.txtCurrLaserIndex = new System.Windows.Forms.TextBox();
            this.txtPrevLaserIndex = new System.Windows.Forms.TextBox();
            this.btnSeed = new System.Windows.Forms.Button();
            this.lblNote = new System.Windows.Forms.Label();
            this.chkSeed = new System.Windows.Forms.CheckBox();
            this.btnContinues = new System.Windows.Forms.Button();
            this.rwDrawer = new LogViewer.DXRenderWindow();
            this.SuspendLayout();
            // 
            // btnStep
            // 
            this.btnStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep.Location = new System.Drawing.Point(628, 70);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 1;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // llLogFile
            // 
            this.llLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.llLogFile.AutoSize = true;
            this.llLogFile.Location = new System.Drawing.Point(12, 443);
            this.llLogFile.Name = "llLogFile";
            this.llLogFile.Size = new System.Drawing.Size(200, 13);
            this.llLogFile.TabIndex = 2;
            this.llLogFile.TabStop = true;
            this.llLogFile.Text = "C:\\Users\\Edris\\Desktop\\Log\\INS 0.3.txt";
            this.llLogFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llLogFile_LinkClicked);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.Filter = "All Files|*.*";
            // 
            // btnInitialize
            // 
            this.btnInitialize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInitialize.Location = new System.Drawing.Point(628, 12);
            this.btnInitialize.Name = "btnInitialize";
            this.btnInitialize.Size = new System.Drawing.Size(75, 23);
            this.btnInitialize.TabIndex = 3;
            this.btnInitialize.Text = "Initialize";
            this.btnInitialize.UseVisualStyleBackColor = true;
            this.btnInitialize.Click += new System.EventHandler(this.btnInitialize_Click);
            // 
            // txtCurrLaserIndex
            // 
            this.txtCurrLaserIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrLaserIndex.Location = new System.Drawing.Point(669, 405);
            this.txtCurrLaserIndex.Name = "txtCurrLaserIndex";
            this.txtCurrLaserIndex.Size = new System.Drawing.Size(34, 20);
            this.txtCurrLaserIndex.TabIndex = 4;
            this.txtCurrLaserIndex.Text = "2";
            this.txtCurrLaserIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPrevLaserIndex
            // 
            this.txtPrevLaserIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrevLaserIndex.Location = new System.Drawing.Point(628, 405);
            this.txtPrevLaserIndex.Name = "txtPrevLaserIndex";
            this.txtPrevLaserIndex.Size = new System.Drawing.Size(33, 20);
            this.txtPrevLaserIndex.TabIndex = 5;
            this.txtPrevLaserIndex.Text = "2090";
            this.txtPrevLaserIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSeed
            // 
            this.btnSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSeed.Location = new System.Drawing.Point(628, 41);
            this.btnSeed.Name = "btnSeed";
            this.btnSeed.Size = new System.Drawing.Size(75, 23);
            this.btnSeed.TabIndex = 6;
            this.btnSeed.Text = "Apply Seed";
            this.btnSeed.UseVisualStyleBackColor = true;
            this.btnSeed.Click += new System.EventHandler(this.btnSeed_Click);
            // 
            // lblNote
            // 
            this.lblNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNote.AutoSize = true;
            this.lblNote.Location = new System.Drawing.Point(634, 388);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(64, 13);
            this.lblNote.TabIndex = 7;
            this.lblNote.Text = "Step Range";
            // 
            // chkSeed
            // 
            this.chkSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSeed.AutoSize = true;
            this.chkSeed.Checked = true;
            this.chkSeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSeed.Location = new System.Drawing.Point(630, 444);
            this.chkSeed.Name = "chkSeed";
            this.chkSeed.Size = new System.Drawing.Size(73, 17);
            this.chkSeed.TabIndex = 8;
            this.chkSeed.Text = "Use Seed";
            this.chkSeed.UseVisualStyleBackColor = true;
            // 
            // btnContinues
            // 
            this.btnContinues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContinues.Location = new System.Drawing.Point(628, 140);
            this.btnContinues.Name = "btnContinues";
            this.btnContinues.Size = new System.Drawing.Size(75, 23);
            this.btnContinues.TabIndex = 9;
            this.btnContinues.Text = "Continues";
            this.btnContinues.UseVisualStyleBackColor = true;
            this.btnContinues.Click += new System.EventHandler(this.btnContinues_Click);
            // 
            // rwDrawer
            // 
            this.rwDrawer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rwDrawer.BackColor = System.Drawing.Color.White;
            this.rwDrawer.Location = new System.Drawing.Point(12, 12);
            this.rwDrawer.Name = "rwDrawer";
            this.rwDrawer.Size = new System.Drawing.Size(610, 413);
            this.rwDrawer.TabIndex = 0;
            this.rwDrawer.UserPaint += new LogViewer.DXRenderWindow.UserPaints(this.rwDrawer_UserPaint);
            // 
            // frmICP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 473);
            this.Controls.Add(this.btnContinues);
            this.Controls.Add(this.chkSeed);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.btnSeed);
            this.Controls.Add(this.txtPrevLaserIndex);
            this.Controls.Add(this.txtCurrLaserIndex);
            this.Controls.Add(this.btnInitialize);
            this.Controls.Add(this.llLogFile);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.rwDrawer);
            this.Name = "frmICP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmICP";
            this.Load += new System.EventHandler(this.frmICP_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LogViewer.DXRenderWindow rwDrawer;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.LinkLabel llLogFile;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.Button btnInitialize;
        private System.Windows.Forms.TextBox txtCurrLaserIndex;
        private System.Windows.Forms.TextBox txtPrevLaserIndex;
        private System.Windows.Forms.Button btnSeed;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.CheckBox chkSeed;
        private System.Windows.Forms.Button btnContinues;
    }
}