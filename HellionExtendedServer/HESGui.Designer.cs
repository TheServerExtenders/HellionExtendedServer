namespace HellionExtendedServer
{
    partial class HESGui
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HESGui));
            this.Tabs = new System.Windows.Forms.TabControl();
            this.Server = new System.Windows.Forms.TabPage();
            this.ServerContainer = new System.Windows.Forms.SplitContainer();
            this.serverconfig_properties = new System.Windows.Forms.PropertyGrid();
            this.server_config_reload = new System.Windows.Forms.Button();
            this.server_config_debugmode = new System.Windows.Forms.CheckBox();
            this.server_config_setdefaults = new System.Windows.Forms.Button();
            this.server_config_cancel = new System.Windows.Forms.Button();
            this.server_config_save = new System.Windows.Forms.Button();
            this.server_config_autostart = new System.Windows.Forms.CheckBox();
            this.server_config_stopserver = new System.Windows.Forms.Button();
            this.server_config_startserver = new System.Windows.Forms.Button();
            this.ChatPlayer = new System.Windows.Forms.TabPage();
            this.ChatPlayerContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.cpc_players_demote = new System.Windows.Forms.Button();
            this.cpc_players_promote = new System.Windows.Forms.Button();
            this.cpc_players_ban = new System.Windows.Forms.Button();
            this.cpc_players_kick = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.cpc_chat_tabs = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cpc_chat_list = new System.Windows.Forms.TextBox();
            this.cpc_chat_send = new System.Windows.Forms.Button();
            this.cpc_messagebox = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Tabs.SuspendLayout();
            this.Server.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServerContainer)).BeginInit();
            this.ServerContainer.Panel1.SuspendLayout();
            this.ServerContainer.Panel2.SuspendLayout();
            this.ServerContainer.SuspendLayout();
            this.ChatPlayer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChatPlayerContainer)).BeginInit();
            this.ChatPlayerContainer.Panel1.SuspendLayout();
            this.ChatPlayerContainer.Panel2.SuspendLayout();
            this.ChatPlayerContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.cpc_chat_tabs.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.Server);
            this.Tabs.Controls.Add(this.ChatPlayer);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabs.Location = new System.Drawing.Point(0, 0);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(812, 495);
            this.Tabs.TabIndex = 0;
            // 
            // Server
            // 
            this.Server.Controls.Add(this.ServerContainer);
            this.Server.Location = new System.Drawing.Point(4, 22);
            this.Server.Name = "Server";
            this.Server.Padding = new System.Windows.Forms.Padding(3);
            this.Server.Size = new System.Drawing.Size(804, 469);
            this.Server.TabIndex = 0;
            this.Server.Text = "Server";
            this.Server.UseVisualStyleBackColor = true;
            // 
            // ServerContainer
            // 
            this.ServerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerContainer.Location = new System.Drawing.Point(3, 3);
            this.ServerContainer.Name = "ServerContainer";
            this.ServerContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ServerContainer.Panel1
            // 
            this.ServerContainer.Panel1.Controls.Add(this.serverconfig_properties);
            // 
            // ServerContainer.Panel2
            // 
            this.ServerContainer.Panel2.Controls.Add(this.server_config_reload);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_debugmode);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_setdefaults);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_cancel);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_save);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_autostart);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_stopserver);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_startserver);
            this.ServerContainer.Size = new System.Drawing.Size(798, 463);
            this.ServerContainer.SplitterDistance = 398;
            this.ServerContainer.TabIndex = 3;
            // 
            // serverconfig_properties
            // 
            this.serverconfig_properties.CommandsBorderColor = System.Drawing.SystemColors.ControlLight;
            this.serverconfig_properties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverconfig_properties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverconfig_properties.LineColor = System.Drawing.SystemColors.ControlDark;
            this.serverconfig_properties.Location = new System.Drawing.Point(0, 0);
            this.serverconfig_properties.Name = "serverconfig_properties";
            this.serverconfig_properties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.serverconfig_properties.Size = new System.Drawing.Size(798, 398);
            this.serverconfig_properties.TabIndex = 0;
            this.serverconfig_properties.ViewBorderColor = System.Drawing.SystemColors.ControlLight;
            // 
            // server_config_reload
            // 
            this.server_config_reload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.server_config_reload.Location = new System.Drawing.Point(571, 37);
            this.server_config_reload.Name = "server_config_reload";
            this.server_config_reload.Size = new System.Drawing.Size(120, 23);
            this.server_config_reload.TabIndex = 6;
            this.server_config_reload.Text = "Reload Config";
            this.server_config_reload.UseVisualStyleBackColor = true;
            this.server_config_reload.Click += new System.EventHandler(this.server_config_reload_Click);
            // 
            // server_config_debugmode
            // 
            this.server_config_debugmode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
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
            this.server_config_setdefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.server_config_setdefaults.Location = new System.Drawing.Point(571, 12);
            this.server_config_setdefaults.Name = "server_config_setdefaults";
            this.server_config_setdefaults.Size = new System.Drawing.Size(120, 23);
            this.server_config_setdefaults.TabIndex = 4;
            this.server_config_setdefaults.Text = "Set Config Defaults";
            this.server_config_setdefaults.UseVisualStyleBackColor = true;
            this.server_config_setdefaults.Click += new System.EventHandler(this.server_config_setdefaults_Click);
            // 
            // server_config_cancel
            // 
            this.server_config_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.server_config_cancel.Location = new System.Drawing.Point(716, 37);
            this.server_config_cancel.Name = "server_config_cancel";
            this.server_config_cancel.Size = new System.Drawing.Size(75, 23);
            this.server_config_cancel.TabIndex = 3;
            this.server_config_cancel.Text = "Cancel";
            this.server_config_cancel.UseVisualStyleBackColor = true;
            this.server_config_cancel.Click += new System.EventHandler(this.server_config_cancel_Click);
            // 
            // server_config_save
            // 
            this.server_config_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.server_config_save.Location = new System.Drawing.Point(716, 11);
            this.server_config_save.Name = "server_config_save";
            this.server_config_save.Size = new System.Drawing.Size(75, 23);
            this.server_config_save.TabIndex = 1;
            this.server_config_save.Text = "Save";
            this.server_config_save.UseVisualStyleBackColor = true;
            this.server_config_save.Click += new System.EventHandler(this.server_config_save_Click);
            // 
            // server_config_autostart
            // 
            this.server_config_autostart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
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
            this.server_config_stopserver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.server_config_stopserver.Location = new System.Drawing.Point(3, 37);
            this.server_config_stopserver.Name = "server_config_stopserver";
            this.server_config_stopserver.Size = new System.Drawing.Size(75, 23);
            this.server_config_stopserver.TabIndex = 1;
            this.server_config_stopserver.Text = "Stop Server";
            this.server_config_stopserver.UseVisualStyleBackColor = true;
            this.server_config_stopserver.Click += new System.EventHandler(this.server_config_stopserver_Click);
            // 
            // server_config_startserver
            // 
            this.server_config_startserver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.server_config_startserver.Location = new System.Drawing.Point(3, 10);
            this.server_config_startserver.Name = "server_config_startserver";
            this.server_config_startserver.Size = new System.Drawing.Size(75, 23);
            this.server_config_startserver.TabIndex = 0;
            this.server_config_startserver.Text = "Start Server";
            this.server_config_startserver.UseVisualStyleBackColor = true;
            this.server_config_startserver.Click += new System.EventHandler(this.server_config_startserver_Click);
            // 
            // ChatPlayer
            // 
            this.ChatPlayer.Controls.Add(this.ChatPlayerContainer);
            this.ChatPlayer.Location = new System.Drawing.Point(4, 22);
            this.ChatPlayer.Name = "ChatPlayer";
            this.ChatPlayer.Padding = new System.Windows.Forms.Padding(3);
            this.ChatPlayer.Size = new System.Drawing.Size(804, 469);
            this.ChatPlayer.TabIndex = 1;
            this.ChatPlayer.Text = "Chat & Player Control";
            this.ChatPlayer.UseVisualStyleBackColor = true;
            // 
            // ChatPlayerContainer
            // 
            this.ChatPlayerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatPlayerContainer.Location = new System.Drawing.Point(3, 3);
            this.ChatPlayerContainer.Name = "ChatPlayerContainer";
            // 
            // ChatPlayerContainer.Panel1
            // 
            this.ChatPlayerContainer.Panel1.Controls.Add(this.splitContainer4);
            // 
            // ChatPlayerContainer.Panel2
            // 
            this.ChatPlayerContainer.Panel2.Controls.Add(this.splitContainer3);
            this.ChatPlayerContainer.Size = new System.Drawing.Size(798, 463);
            this.ChatPlayerContainer.SplitterDistance = 356;
            this.ChatPlayerContainer.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.cpc_players_demote);
            this.splitContainer4.Panel2.Controls.Add(this.cpc_players_promote);
            this.splitContainer4.Panel2.Controls.Add(this.cpc_players_ban);
            this.splitContainer4.Panel2.Controls.Add(this.cpc_players_kick);
            this.splitContainer4.Size = new System.Drawing.Size(356, 463);
            this.splitContainer4.SplitterDistance = 369;
            this.splitContainer4.TabIndex = 0;
            // 
            // cpc_players_demote
            // 
            this.cpc_players_demote.Enabled = false;
            this.cpc_players_demote.Location = new System.Drawing.Point(162, 31);
            this.cpc_players_demote.Name = "cpc_players_demote";
            this.cpc_players_demote.Size = new System.Drawing.Size(64, 23);
            this.cpc_players_demote.TabIndex = 4;
            this.cpc_players_demote.Text = "Demote";
            this.cpc_players_demote.UseVisualStyleBackColor = true;
            // 
            // cpc_players_promote
            // 
            this.cpc_players_promote.Enabled = false;
            this.cpc_players_promote.Location = new System.Drawing.Point(162, 3);
            this.cpc_players_promote.Name = "cpc_players_promote";
            this.cpc_players_promote.Size = new System.Drawing.Size(64, 23);
            this.cpc_players_promote.TabIndex = 3;
            this.cpc_players_promote.Text = "Promote";
            this.cpc_players_promote.UseVisualStyleBackColor = true;
            // 
            // cpc_players_ban
            // 
            this.cpc_players_ban.Location = new System.Drawing.Point(5, 31);
            this.cpc_players_ban.Name = "cpc_players_ban";
            this.cpc_players_ban.Size = new System.Drawing.Size(63, 23);
            this.cpc_players_ban.TabIndex = 2;
            this.cpc_players_ban.Text = "Ban";
            this.cpc_players_ban.UseVisualStyleBackColor = true;
            this.cpc_players_ban.Click += new System.EventHandler(this.cpc_players_ban_Click);
            // 
            // cpc_players_kick
            // 
            this.cpc_players_kick.Location = new System.Drawing.Point(5, 3);
            this.cpc_players_kick.Name = "cpc_players_kick";
            this.cpc_players_kick.Size = new System.Drawing.Size(63, 23);
            this.cpc_players_kick.TabIndex = 1;
            this.cpc_players_kick.Text = "Kick";
            this.cpc_players_kick.UseVisualStyleBackColor = true;
            this.cpc_players_kick.Click += new System.EventHandler(this.cpc_players_kick_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.cpc_chat_tabs);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.cpc_chat_send);
            this.splitContainer3.Panel2.Controls.Add(this.cpc_messagebox);
            this.splitContainer3.Size = new System.Drawing.Size(438, 463);
            this.splitContainer3.SplitterDistance = 431;
            this.splitContainer3.TabIndex = 0;
            // 
            // cpc_chat_tabs
            // 
            this.cpc_chat_tabs.Controls.Add(this.tabPage3);
            this.cpc_chat_tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpc_chat_tabs.Enabled = false;
            this.cpc_chat_tabs.Location = new System.Drawing.Point(0, 0);
            this.cpc_chat_tabs.Name = "cpc_chat_tabs";
            this.cpc_chat_tabs.SelectedIndex = 0;
            this.cpc_chat_tabs.Size = new System.Drawing.Size(438, 431);
            this.cpc_chat_tabs.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cpc_chat_list);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(430, 405);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Chat";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cpc_chat_list
            // 
            this.cpc_chat_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpc_chat_list.Location = new System.Drawing.Point(3, 3);
            this.cpc_chat_list.Multiline = true;
            this.cpc_chat_list.Name = "cpc_chat_list";
            this.cpc_chat_list.ReadOnly = true;
            this.cpc_chat_list.Size = new System.Drawing.Size(424, 399);
            this.cpc_chat_list.TabIndex = 2;
            // 
            // cpc_chat_send
            // 
            this.cpc_chat_send.Location = new System.Drawing.Point(509, 3);
            this.cpc_chat_send.Name = "cpc_chat_send";
            this.cpc_chat_send.Size = new System.Drawing.Size(50, 23);
            this.cpc_chat_send.TabIndex = 1;
            this.cpc_chat_send.Text = "Send";
            this.cpc_chat_send.UseVisualStyleBackColor = true;
            this.cpc_chat_send.Click += new System.EventHandler(this.cpc_chat_send_Click);
            // 
            // cpc_messagebox
            // 
            this.cpc_messagebox.Location = new System.Drawing.Point(3, 5);
            this.cpc_messagebox.Name = "cpc_messagebox";
            this.cpc_messagebox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.cpc_messagebox.Size = new System.Drawing.Size(500, 20);
            this.cpc_messagebox.TabIndex = 0;
            this.cpc_messagebox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cpc_messagebox_KeyDown);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 495);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(812, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            // 
            // StatusBar
            // 
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(0, 17);
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(356, 369);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // HESGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(812, 517);
            this.Controls.Add(this.Tabs);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HESGui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "-";
            this.Tabs.ResumeLayout(false);
            this.Server.ResumeLayout(false);
            this.ServerContainer.Panel1.ResumeLayout(false);
            this.ServerContainer.Panel2.ResumeLayout(false);
            this.ServerContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServerContainer)).EndInit();
            this.ServerContainer.ResumeLayout(false);
            this.ChatPlayer.ResumeLayout(false);
            this.ChatPlayerContainer.Panel1.ResumeLayout(false);
            this.ChatPlayerContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ChatPlayerContainer)).EndInit();
            this.ChatPlayerContainer.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.cpc_chat_tabs.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage Server;
        private System.Windows.Forms.SplitContainer ServerContainer;
        private System.Windows.Forms.PropertyGrid serverconfig_properties;
        private System.Windows.Forms.CheckBox server_config_autostart;
        private System.Windows.Forms.Button server_config_stopserver;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Button server_config_setdefaults;
        private System.Windows.Forms.Button server_config_cancel;
        private System.Windows.Forms.Button server_config_save;
        private System.Windows.Forms.CheckBox server_config_debugmode;
        private System.Windows.Forms.ToolStripStatusLabel StatusBar;
        private System.Windows.Forms.TabPage ChatPlayer;
        private System.Windows.Forms.SplitContainer ChatPlayerContainer;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Button cpc_players_demote;
        private System.Windows.Forms.Button cpc_players_promote;
        private System.Windows.Forms.Button cpc_players_ban;
        private System.Windows.Forms.Button cpc_players_kick;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TabControl cpc_chat_tabs;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox cpc_chat_list;
        private System.Windows.Forms.Button cpc_chat_send;
        private System.Windows.Forms.TextBox cpc_messagebox;
        public System.Windows.Forms.Button server_config_startserver;
        private System.Windows.Forms.Button server_config_reload;
        private System.Windows.Forms.ListView listView1;
    }
}

