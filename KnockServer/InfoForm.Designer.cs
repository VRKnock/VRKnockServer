using System.ComponentModel;

namespace KnockServer
{
    partial class InfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(InfoForm));
            this.label1 = new System.Windows.Forms.Label();
            this.qrPictureBox = new System.Windows.Forms.PictureBox();
            this.hostLabel = new System.Windows.Forms.Label();
            this.codeLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gameActivityCheckbox = new System.Windows.Forms.CheckBox();
            this.autostartCheckbox = new System.Windows.Forms.CheckBox();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.codeTextBox = new System.Windows.Forms.TextBox();
            this.connectionMethodComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.clientCountLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize) (this.qrPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(446, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Scan the QR Code Below using the VRKnock App!";
            // 
            // qrPictureBox
            // 
            this.qrPictureBox.Location = new System.Drawing.Point(10, 30);
            this.qrPictureBox.Name = "qrPictureBox";
            this.qrPictureBox.Size = new System.Drawing.Size(199, 200);
            this.qrPictureBox.TabIndex = 1;
            this.qrPictureBox.TabStop = false;
            // 
            // hostLabel
            // 
            this.hostLabel.Location = new System.Drawing.Point(10, 240);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(50, 20);
            this.hostLabel.TabIndex = 2;
            this.hostLabel.Text = "Host:";
            // 
            // codeLabel
            // 
            this.codeLabel.Location = new System.Drawing.Point(10, 265);
            this.codeLabel.Name = "codeLabel";
            this.codeLabel.Size = new System.Drawing.Size(50, 20);
            this.codeLabel.TabIndex = 3;
            this.codeLabel.Text = "Code:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 434);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(226, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Right-Click the Tray Icon to Exit";
            // 
            // gameActivityCheckbox
            // 
            this.gameActivityCheckbox.Location = new System.Drawing.Point(12, 354);
            this.gameActivityCheckbox.Name = "gameActivityCheckbox";
            this.gameActivityCheckbox.Size = new System.Drawing.Size(199, 20);
            this.gameActivityCheckbox.TabIndex = 5;
            this.gameActivityCheckbox.Text = "Show Current Game In App";
            this.gameActivityCheckbox.UseVisualStyleBackColor = true;
            this.gameActivityCheckbox.CheckedChanged +=
                new System.EventHandler(this.gameActivityCheckbox_CheckedChanged);
            // 
            // autostartCheckbox
            // 
            this.autostartCheckbox.Location = new System.Drawing.Point(12, 384);
            this.autostartCheckbox.Name = "autostartCheckbox";
            this.autostartCheckbox.Size = new System.Drawing.Size(199, 20);
            this.autostartCheckbox.TabIndex = 6;
            this.autostartCheckbox.Text = "Autostart with SteamVR";
            this.autostartCheckbox.UseVisualStyleBackColor = true;
            this.autostartCheckbox.CheckedChanged += new System.EventHandler(this.autostartCheckbox_CheckedChanged);
            // 
            // hostTextBox
            // 
            this.hostTextBox.Location = new System.Drawing.Point(66, 237);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.ReadOnly = true;
            this.hostTextBox.Size = new System.Drawing.Size(200, 23);
            this.hostTextBox.TabIndex = 7;
            // 
            // codeTextBox
            // 
            this.codeTextBox.Location = new System.Drawing.Point(66, 262);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.ReadOnly = true;
            this.codeTextBox.Size = new System.Drawing.Size(200, 23);
            this.codeTextBox.TabIndex = 8;
            // 
            // connectionMethodComboBox
            // 
            this.connectionMethodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.connectionMethodComboBox.FormattingEnabled = true;
            this.connectionMethodComboBox.Items.AddRange(new object[] {"DIRECT", "BRIDGE"});
            this.connectionMethodComboBox.Location = new System.Drawing.Point(87, 301);
            this.connectionMethodComboBox.Name = "connectionMethodComboBox";
            this.connectionMethodComboBox.Size = new System.Drawing.Size(178, 23);
            this.connectionMethodComboBox.TabIndex = 9;
            this.connectionMethodComboBox.SelectedIndexChanged +=
                new System.EventHandler(this.connectionMethodComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 301);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 30);
            this.label3.TabIndex = 10;
            this.label3.Text = "Connection Method";
            // 
            // clientCountLabel
            // 
            this.clientCountLabel.Location = new System.Drawing.Point(10, 482);
            this.clientCountLabel.Name = "clientCountLabel";
            this.clientCountLabel.Size = new System.Drawing.Size(72, 20);
            this.clientCountLabel.TabIndex = 11;
            this.clientCountLabel.Text = "0 Clients";
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(230, 482);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(72, 20);
            this.versionLabel.TabIndex = 12;
            this.versionLabel.Text = "0.0.0";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // InfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 511);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.clientCountLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.connectionMethodComboBox);
            this.Controls.Add(this.codeTextBox);
            this.Controls.Add(this.hostTextBox);
            this.Controls.Add(this.autostartCheckbox);
            this.Controls.Add(this.gameActivityCheckbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.codeLabel);
            this.Controls.Add(this.hostLabel);
            this.Controls.Add(this.qrPictureBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "InfoForm";
            this.Text = "VRKnock Server";
            ((System.ComponentModel.ISupportInitialize) (this.qrPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox qrPictureBox;
        private System.Windows.Forms.Label codeLabel;
        private System.Windows.Forms.Label hostLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox gameActivityCheckbox;
        private System.Windows.Forms.CheckBox autostartCheckbox;
        private System.Windows.Forms.TextBox codeTextBox;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.ComboBox connectionMethodComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label clientCountLabel;
        private System.Windows.Forms.Label versionLabel;
    }
}