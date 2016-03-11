namespace SUMDJoy
{
    partial class AboutForm
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
            this.labelProductName = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelBy = new System.Windows.Forms.Label();
            this.labelLicence = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.pictureBoxLicence = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLicence)).BeginInit();
            this.SuspendLayout();
            // 
            // labelProductName
            // 
            this.labelProductName.AutoSize = true;
            this.labelProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProductName.Location = new System.Drawing.Point(81, 9);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(101, 24);
            this.labelProductName.TabIndex = 0;
            this.labelProductName.Text = "(SumdJoy)";
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(112, 194);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(60, 27);
            this.button_OK.TabIndex = 1;
            this.button_OK.Text = "&OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.Location = new System.Drawing.Point(98, 33);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(67, 23);
            this.labelVersion.TabIndex = 2;
            this.labelVersion.Text = "(Version)";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelBy
            // 
            this.labelBy.Location = new System.Drawing.Point(101, 56);
            this.labelBy.Name = "labelBy";
            this.labelBy.Size = new System.Drawing.Size(81, 13);
            this.labelBy.TabIndex = 3;
            this.labelBy.Text = "by Andi Kanzler";
            // 
            // labelLicence
            // 
            this.labelLicence.Location = new System.Drawing.Point(4, 147);
            this.labelLicence.Name = "labelLicence";
            this.labelLicence.Size = new System.Drawing.Size(277, 33);
            this.labelLicence.TabIndex = 4;
            this.labelLicence.Text = "(license)";
            this.labelLicence.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelLicence.Click += new System.EventHandler(this.labelLicence_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(48, 69);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(167, 13);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "github.com/scavanger/SUMDJoy";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // pictureBoxLicence
            // 
            this.pictureBoxLicence.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxLicence.Image = global::SUMDJoy.Properties.Resources.by_nc_sa_eu;
            this.pictureBoxLicence.Location = new System.Drawing.Point(88, 99);
            this.pictureBoxLicence.Name = "pictureBoxLicence";
            this.pictureBoxLicence.Size = new System.Drawing.Size(86, 45);
            this.pictureBoxLicence.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLicence.TabIndex = 6;
            this.pictureBoxLicence.TabStop = false;
            this.pictureBoxLicence.Click += new System.EventHandler(this.pictureBoxLicence_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 233);
            this.Controls.Add(this.pictureBoxLicence);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.labelLicence);
            this.Controls.Add(this.labelBy);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.labelProductName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AboutForm";
            this.Text = "AboutForm";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLicence)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelBy;
        private System.Windows.Forms.Label labelLicence;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.PictureBox pictureBoxLicence;
    }
}