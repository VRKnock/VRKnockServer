using System;
using System.Windows.Forms;

namespace KnockServer
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();

            var manager = NotificationManager.GetInstance();
            qrPictureBox.Image = manager.GetQRCode().GetGraphic(4);
            
            hostTextBox.Text = NotificationManager.GetLocalIPAddress();
            codeTextBox.Text = manager.connectionCode;

            autostartCheckbox.Checked = Properties.Settings.Default.AutoStart;
            gameActivityCheckbox.Checked = Properties.Settings.Default.ShowActivity;
        }
    
        
      private void gameActivityCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowActivity = gameActivityCheckbox.Checked;
            Properties.Settings.Default.Save();
        }

        private void autostartCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoStart = autostartCheckbox.Checked;
            Properties.Settings.Default.Save();
            Program.RegisterAutoLaunchApp();
        }
    }
}