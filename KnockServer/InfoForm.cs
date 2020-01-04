using System;
using System.Reflection;
using System.Windows.Forms;

namespace KnockServer
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();

            var manager = NotificationManager.GetInstance();
            var qrcode = manager.GetQRCode();
            qrPictureBox.Image = qrcode.GetGraphic(3);
            
            hostTextBox.Text = NotificationManager.GetLocalIPAddress();
            codeTextBox.Text = manager.connectionCode;

            autostartCheckbox.Checked = Properties.Settings.Default.AutoStart;
            gameActivityCheckbox.Checked = Properties.Settings.Default.ShowActivity;
            connectionMethodComboBox.SelectedIndex = connectionMethodComboBox.FindStringExact(Properties.Settings.Default.ConnectionMethod);

            versionLabel.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            clientCountLabel.Text = CustomApplicationContext.GetClientCount().ToString()+" Client(s)";
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

        private void connectionMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ConnectionMethod =  connectionMethodComboBox.GetItemText(connectionMethodComboBox.SelectedItem);
            Properties.Settings.Default.Save();
        }
    }
}