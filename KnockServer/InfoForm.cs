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
            hostLabel.Text = "Host:  "+ NotificationManager.GetLocalIPAddress();
            codeLabel.Text = "Code:  " + manager.connectionCode;

            autostartCheckbox.Checked = Properties.Settings.Default.AutoStart;
            gameActivityCheckbox.Checked = Properties.Settings.Default.ShowActivity;
        }
        
        private void gameActivityCheckbox_Click(object sender, System.EventArgs e)  
        {
            Properties.Settings.Default.ShowActivity = gameActivityCheckbox.Checked;
        } 
        
        private void autostartCheckbox_Click(object sender, System.EventArgs e)
        {
            Properties.Settings.Default.AutoStart = autostartCheckbox.Checked;
            Program.RegisterAutoLaunchApp();
        } 
        
    }
}