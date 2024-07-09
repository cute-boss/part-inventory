namespace SPSServer
{
    partial class SvrForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SvrForm));
            this.Btn_exit = new System.Windows.Forms.Button();
            this.Lbl_copyright = new System.Windows.Forms.Label();
            this.Lbl_licenseStatus = new System.Windows.Forms.Label();
            this.Lbl_versionStatus = new System.Windows.Forms.Label();
            this.Lbl_portStatus = new System.Windows.Forms.Label();
            this.Lbl_ipStatus = new System.Windows.Forms.Label();
            this.Lbl_ip = new System.Windows.Forms.Label();
            this.Lbl_license = new System.Windows.Forms.Label();
            this.Lbl_port = new System.Windows.Forms.Label();
            this.Lbl_version = new System.Windows.Forms.Label();
            this.Panel_result = new System.Windows.Forms.Panel();
            this.Lbl_title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Btn_exit
            // 
            this.Btn_exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_exit.Location = new System.Drawing.Point(372, 290);
            this.Btn_exit.Name = "Btn_exit";
            this.Btn_exit.Size = new System.Drawing.Size(75, 23);
            this.Btn_exit.TabIndex = 108;
            this.Btn_exit.Text = "&Exit";
            this.Btn_exit.UseVisualStyleBackColor = true;
            this.Btn_exit.Click += new System.EventHandler(this.Btn_exit_Click);
            // 
            // Lbl_copyright
            // 
            this.Lbl_copyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Lbl_copyright.AutoSize = true;
            this.Lbl_copyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_copyright.Location = new System.Drawing.Point(15, 296);
            this.Lbl_copyright.Name = "Lbl_copyright";
            this.Lbl_copyright.Size = new System.Drawing.Size(56, 12);
            this.Lbl_copyright.TabIndex = 107;
            this.Lbl_copyright.Text = "Copyright @";
            // 
            // Lbl_licenseStatus
            // 
            this.Lbl_licenseStatus.AutoSize = true;
            this.Lbl_licenseStatus.Location = new System.Drawing.Point(290, 259);
            this.Lbl_licenseStatus.Name = "Lbl_licenseStatus";
            this.Lbl_licenseStatus.Size = new System.Drawing.Size(10, 13);
            this.Lbl_licenseStatus.TabIndex = 106;
            this.Lbl_licenseStatus.Text = "-";
            // 
            // Lbl_versionStatus
            // 
            this.Lbl_versionStatus.AutoSize = true;
            this.Lbl_versionStatus.Location = new System.Drawing.Point(290, 235);
            this.Lbl_versionStatus.Name = "Lbl_versionStatus";
            this.Lbl_versionStatus.Size = new System.Drawing.Size(10, 13);
            this.Lbl_versionStatus.TabIndex = 105;
            this.Lbl_versionStatus.Text = "-";
            // 
            // Lbl_portStatus
            // 
            this.Lbl_portStatus.AutoSize = true;
            this.Lbl_portStatus.Location = new System.Drawing.Point(84, 259);
            this.Lbl_portStatus.Name = "Lbl_portStatus";
            this.Lbl_portStatus.Size = new System.Drawing.Size(10, 13);
            this.Lbl_portStatus.TabIndex = 104;
            this.Lbl_portStatus.Text = "-";
            this.Lbl_portStatus.Visible = false;
            // 
            // Lbl_ipStatus
            // 
            this.Lbl_ipStatus.AutoSize = true;
            this.Lbl_ipStatus.Location = new System.Drawing.Point(84, 235);
            this.Lbl_ipStatus.Name = "Lbl_ipStatus";
            this.Lbl_ipStatus.Size = new System.Drawing.Size(10, 13);
            this.Lbl_ipStatus.TabIndex = 103;
            this.Lbl_ipStatus.Text = "-";
            // 
            // Lbl_ip
            // 
            this.Lbl_ip.AutoSize = true;
            this.Lbl_ip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_ip.Location = new System.Drawing.Point(14, 235);
            this.Lbl_ip.Name = "Lbl_ip";
            this.Lbl_ip.Size = new System.Drawing.Size(64, 13);
            this.Lbl_ip.TabIndex = 102;
            this.Lbl_ip.Text = "IP Address: ";
            // 
            // Lbl_license
            // 
            this.Lbl_license.AutoSize = true;
            this.Lbl_license.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_license.Location = new System.Drawing.Point(236, 259);
            this.Lbl_license.Name = "Lbl_license";
            this.Lbl_license.Size = new System.Drawing.Size(50, 13);
            this.Lbl_license.TabIndex = 101;
            this.Lbl_license.Text = "License: ";
            // 
            // Lbl_port
            // 
            this.Lbl_port.AutoSize = true;
            this.Lbl_port.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_port.Location = new System.Drawing.Point(14, 259);
            this.Lbl_port.Name = "Lbl_port";
            this.Lbl_port.Size = new System.Drawing.Size(32, 13);
            this.Lbl_port.TabIndex = 100;
            this.Lbl_port.Text = "Port: ";
            this.Lbl_port.Visible = false;
            // 
            // Lbl_version
            // 
            this.Lbl_version.AutoSize = true;
            this.Lbl_version.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_version.Location = new System.Drawing.Point(236, 235);
            this.Lbl_version.Name = "Lbl_version";
            this.Lbl_version.Size = new System.Drawing.Size(48, 13);
            this.Lbl_version.TabIndex = 99;
            this.Lbl_version.Text = "Version: ";
            // 
            // Panel_result
            // 
            this.Panel_result.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel_result.BackColor = System.Drawing.Color.LightGray;
            this.Panel_result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_result.Location = new System.Drawing.Point(11, 61);
            this.Panel_result.Name = "Panel_result";
            this.Panel_result.Size = new System.Drawing.Size(436, 168);
            this.Panel_result.TabIndex = 98;
            // 
            // Lbl_title
            // 
            this.Lbl_title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Lbl_title.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Lbl_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_title.Location = new System.Drawing.Point(11, 7);
            this.Lbl_title.Name = "Lbl_title";
            this.Lbl_title.Size = new System.Drawing.Size(436, 47);
            this.Lbl_title.TabIndex = 97;
            this.Lbl_title.Text = "Spare Part System";
            this.Lbl_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SvrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 320);
            this.Controls.Add(this.Btn_exit);
            this.Controls.Add(this.Lbl_copyright);
            this.Controls.Add(this.Lbl_licenseStatus);
            this.Controls.Add(this.Lbl_versionStatus);
            this.Controls.Add(this.Lbl_portStatus);
            this.Controls.Add(this.Lbl_ipStatus);
            this.Controls.Add(this.Lbl_ip);
            this.Controls.Add(this.Lbl_license);
            this.Controls.Add(this.Lbl_port);
            this.Controls.Add(this.Lbl_version);
            this.Controls.Add(this.Panel_result);
            this.Controls.Add(this.Lbl_title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SvrForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPS Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SvrForm_FormClosing);
            this.Load += new System.EventHandler(this.SvrForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_exit;
        private System.Windows.Forms.Label Lbl_copyright;
        private System.Windows.Forms.Label Lbl_licenseStatus;
        private System.Windows.Forms.Label Lbl_versionStatus;
        private System.Windows.Forms.Label Lbl_portStatus;
        private System.Windows.Forms.Label Lbl_ipStatus;
        private System.Windows.Forms.Label Lbl_ip;
        private System.Windows.Forms.Label Lbl_license;
        private System.Windows.Forms.Label Lbl_port;
        private System.Windows.Forms.Label Lbl_version;
        private System.Windows.Forms.Panel Panel_result;
        private System.Windows.Forms.Label Lbl_title;
    }
}

