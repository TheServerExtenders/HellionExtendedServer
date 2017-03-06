namespace HellionExtendedServer
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.serverconfig_properties = new System.Windows.Forms.PropertyGrid();
            this.server_config_debugmode = new System.Windows.Forms.CheckBox();
            this.server_config_setdefaults = new System.Windows.Forms.Button();
            this.server_config_cancel = new System.Windows.Forms.Button();
            this.server_config_save = new System.Windows.Forms.Button();
            this.server_config_autostart = new System.Windows.Forms.CheckBox();
            this.server_config_stopserver = new System.Windows.Forms.Button();
            this.server_config_startserver = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(760, 479);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.statusStrip1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(752, 453);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Server";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.serverconfig_properties);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.server_config_debugmode);
            this.splitContainer1.Panel2.Controls.Add(this.server_config_setdefaults);
            this.splitContainer1.Panel2.Controls.Add(this.server_config_cancel);
            this.splitContainer1.Panel2.Controls.Add(this.server_config_save);
            this.splitContainer1.Panel2.Controls.Add(this.server_config_autostart);
            this.splitContainer1.Panel2.Controls.Add(this.server_config_stopserver);
            this.splitContainer1.Panel2.Controls.Add(this.server_config_startserver);
            this.splitContainer1.Size = new System.Drawing.Size(746, 425);
            this.splitContainer1.SplitterDistance = 366;
            this.splitContainer1.TabIndex = 3;
            // 
            // serverconfig_properties
            // 
            this.serverconfig_properties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverconfig_properties.Location = new System.Drawing.Point(0, 0);
            this.serverconfig_properties.Name = "serverconfig_properties";
            this.serverconfig_properties.Size = new System.Drawing.Size(746, 366);
            this.serverconfig_properties.TabIndex = 0;
            // 
            // server_config_debugmode
            // 
            this.server_config_debugmode.AutoSize = true;
            this.server_config_debugmode.Location = new System.Drawing.Point(202, 9);
            this.server_config_debugmode.Name = "server_config_debugmode";
            this.server_config_debugmode.Size = new System.Drawing.Size(88, 17);
            this.server_config_debugmode.TabIndex = 5;
            this.server_config_debugmode.Text = "Debug Mode";
            this.server_config_debugmode.UseVisualStyleBackColor = true;
            this.server_config_debugmode.CheckedChanged += new System.EventHandler(this.server_config_debugmode_CheckedChanged);
            // 
            // server_config_setdefaults
            // 
            this.server_config_setdefaults.Location = new System.Drawing.Point(488, 6);
            this.server_config_setdefaults.Name = "server_config_setdefaults";
            this.server_config_setdefaults.Size = new System.Drawing.Size(115, 23);
            this.server_config_setdefaults.TabIndex = 4;
            this.server_config_setdefaults.Text = "Set Config Defaults";
            this.server_config_setdefaults.UseVisualStyleBackColor = true;
            this.server_config_setdefaults.Click += new System.EventHandler(this.server_config_setdefaults_Click);
            // 
            // server_config_cancel
            // 
            this.server_config_cancel.Location = new System.Drawing.Point(666, 31);
            this.server_config_cancel.Name = "server_config_cancel";
            this.server_config_cancel.Size = new System.Drawing.Size(75, 23);
            this.server_config_cancel.TabIndex = 3;
            this.server_config_cancel.Text = "Cancel";
            this.server_config_cancel.UseVisualStyleBackColor = true;
            this.server_config_cancel.Click += new System.EventHandler(this.server_config_cancel_Click);
            // 
            // server_config_save
            // 
            this.server_config_save.Location = new System.Drawing.Point(666, 5);
            this.server_config_save.Name = "server_config_save";
            this.server_config_save.Size = new System.Drawing.Size(75, 23);
            this.server_config_save.TabIndex = 1;
            this.server_config_save.Text = "Save";
            this.server_config_save.UseVisualStyleBackColor = true;
            this.server_config_save.Click += new System.EventHandler(this.server_config_save_Click);
            // 
            // server_config_autostart
            // 
            this.server_config_autostart.AutoSize = true;
            this.server_config_autostart.Location = new System.Drawing.Point(85, 9);
            this.server_config_autostart.Name = "server_config_autostart";
            this.server_config_autostart.Size = new System.Drawing.Size(111, 17);
            this.server_config_autostart.TabIndex = 2;
            this.server_config_autostart.Text = "Auto-Start on load";
            this.server_config_autostart.UseVisualStyleBackColor = true;
            this.server_config_autostart.CheckedChanged += new System.EventHandler(this.server_config_autostart_CheckedChanged);
            // 
            // server_config_stopserver
            // 
            this.server_config_stopserver.Location = new System.Drawing.Point(3, 31);
            this.server_config_stopserver.Name = "server_config_stopserver";
            this.server_config_stopserver.Size = new System.Drawing.Size(75, 23);
            this.server_config_stopserver.TabIndex = 1;
            this.server_config_stopserver.Text = "Stop Server";
            this.server_config_stopserver.UseVisualStyleBackColor = true;
            this.server_config_stopserver.Click += new System.EventHandler(this.server_config_stopserver_Click);
            // 
            // server_config_startserver
            // 
            this.server_config_startserver.Location = new System.Drawing.Point(3, 4);
            this.server_config_startserver.Name = "server_config_startserver";
            this.server_config_startserver.Size = new System.Drawing.Size(75, 23);
            this.server_config_startserver.TabIndex = 0;
            this.server_config_startserver.Text = "Start Server";
            this.server_config_startserver.UseVisualStyleBackColor = true;
            this.server_config_startserver.Click += new System.EventHandler(this.server_config_startserver_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(3, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(746, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 479);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "Hellion Extended Server GUI";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid serverconfig_properties;
        private System.Windows.Forms.CheckBox server_config_autostart;
        private System.Windows.Forms.Button server_config_stopserver;
        private System.Windows.Forms.Button server_config_startserver;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button server_config_setdefaults;
        private System.Windows.Forms.Button server_config_cancel;
        private System.Windows.Forms.Button server_config_save;
        private System.Windows.Forms.CheckBox server_config_debugmode;
    }
}

