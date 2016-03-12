using System;
using System.Windows.Forms;
using System.Reflection;

namespace SUMDJoy
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            labelProductName.Text = Application.ProductName;

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            labelVersion.Text = string.Format("Version: {0}.{1}", ver.Major, ver.Minor);
            labelLicence.Text = "Creative Commons\nAttribution-NonCommercial - ShareAlike 4.0 International";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.github.com/scavanger/sumdjoy");
        }

        private void pictureBoxLicence_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://creativecommons.org/licenses/by-nc-sa/4.0/");
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
