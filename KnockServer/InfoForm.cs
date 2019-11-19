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
        }
    }
}