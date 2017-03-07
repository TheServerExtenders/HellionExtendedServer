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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Online");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Offline");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Players", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Online");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Offline");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Admins", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.cpc_messagebox = new System.Windows.Forms.TextBox();
            this.cpc_chat_send = new System.Windows.Forms.Button();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.cpc_players_treelist = new System.Windows.Forms.TreeView();
            this.cpc_players_kick = new System.Windows.Forms.Button();
            this.cpc_chat_tabs = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.cpc_chat_list = new System.Windows.Forms.TextBox();
            this.cpc_chat_privatemessages = new System.Windows.Forms.TextBox();
            this.cpc_players_ban = new System.Windows.Forms.Button();
            this.cpc_players_promote = new System.Windows.Forms.Button();
            this.cpc_players_demote = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.cpc_chat_tabs.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(812, 517);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.statusStrip);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(804, 491);
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
            this.splitContainer1.Size = new System.Drawing.Size(798, 463);
            this.splitContainer1.SplitterDistance = 398;
            this.splitContainer1.TabIndex = 3;
            // 
            // serverconfig_properties
            // 
            this.serverconfig_properties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverconfig_properties.Location = new System.Drawing.Point(0, 0);
            this.serverconfig_properties.Name = "serverconfig_properties";
            this.serverconfig_properties.Size = new System.Drawing.Size(798, 398);
            this.serverconfig_properties.TabIndex = 0;
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
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBar});
            this.statusStrip.Location = new System.Drawing.Point(3, 466);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(798, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            // 
            // StatusBar
            // 
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(0, 17);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(804, 491);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Chat & Player Control";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(798, 485);
            this.splitContainer2.SplitterDistance = 229;
            this.splitContainer2.TabIndex = 0;
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
            this.splitContainer3.Size = new System.Drawing.Size(565, 485);
            this.splitContainer3.SplitterDistance = 452;
            this.splitContainer3.TabIndex = 0;
            // 
            // cpc_messagebox
            // 
            this.cpc_messagebox.Location = new System.Drawing.Point(3, 5);
            this.cpc_messagebox.Name = "cpc_messagebox";
            this.cpc_messagebox.Size = new System.Drawing.Size(500, 20);
            this.cpc_messagebox.TabIndex = 0;
            // 
            // cpc_chat_send
            // 
            this.cpc_chat_send.Location = new System.Drawing.Point(509, 3);
            this.cpc_chat_send.Name = "cpc_chat_send";
            this.cpc_chat_send.Size = new System.Drawing.Size(50, 23);
            this.cpc_chat_send.TabIndex = 1;
            this.cpc_chat_send.Text = "Send";
            this.cpc_chat_send.UseVisualStyleBackColor = true;
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
            this.splitContainer4.Panel1.Controls.Add(this.cpc_players_treelist);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.cpc_players_demote);
            this.splitContainer4.Panel2.Controls.Add(this.cpc_players_promote);
            this.splitContainer4.Panel2.Controls.Add(this.cpc_players_ban);
            this.splitContainer4.Panel2.Controls.Add(this.cpc_players_kick);
            this.splitContainer4.Size = new System.Drawing.Size(229, 485);
            this.splitContainer4.SplitterDistance = 387;
            this.splitContainer4.TabIndex = 0;
            // 
            // cpc_players_treelist
            // 
            this.cpc_players_treelist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpc_players_treelist.Location = new System.Drawing.Point(0, 0);
            this.cpc_players_treelist.Name = "cpc_players_treelist";
            treeNode1.Name = "Online";
            treeNode1.Text = "Online";
            treeNode2.Name = "Offline";
            treeNode2.Text = "Offline";
            treeNode3.Name = "Players";
            treeNode3.Text = "Players";
            treeNode4.Name = "AOnline";
            treeNode4.Text = "Online";
            treeNode5.Name = "AOffline";
            treeNode5.Text = "Offline";
            treeNode6.Name = "Admins";
            treeNode6.Text = "Admins";
            this.cpc_players_treelist.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode6});
            this.cpc_players_treelist.Size = new System.Drawing.Size(229, 387);
            this.cpc_players_treelist.TabIndex = 2;
            // 
            // cpc_players_kick
            // 
            this.cpc_players_kick.Location = new System.Drawing.Point(5, 3);
            this.cpc_players_kick.Name = "cpc_players_kick";
            this.cpc_players_kick.Size = new System.Drawing.Size(63, 23);
            this.cpc_players_kick.TabIndex = 1;
            this.cpc_players_kick.Text = "Kick";
            this.cpc_players_kick.UseVisualStyleBackColor = true;
            // 
            // cpc_chat_tabs
            // 
            this.cpc_chat_tabs.Controls.Add(this.tabPage3);
            this.cpc_chat_tabs.Controls.Add(this.tabPage4);
            this.cpc_chat_tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpc_chat_tabs.Location = new System.Drawing.Point(0, 0);
            this.cpc_chat_tabs.Name = "cpc_chat_tabs";
            this.cpc_chat_tabs.SelectedIndex = 0;
            this.cpc_chat_tabs.Size = new System.Drawing.Size(565, 452);
            this.cpc_chat_tabs.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cpc_chat_list);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(557, 426);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Chat";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.cpc_chat_privatemessages);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(557, 426);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Private Messages";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // cpc_chat_list
            // 
            this.cpc_chat_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpc_chat_list.Location = new System.Drawing.Point(3, 3);
            this.cpc_chat_list.Multiline = true;
            this.cpc_chat_list.Name = "cpc_chat_list";
            this.cpc_chat_list.ReadOnly = true;
            this.cpc_chat_list.Size = new System.Drawing.Size(551, 420);
            this.cpc_chat_list.TabIndex = 2;
            // 
            // cpc_chat_privatemessages
            // 
            this.cpc_chat_privatemessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpc_chat_privatemessages.Location = new System.Drawing.Point(3, 3);
            this.cpc_chat_privatemessages.Multiline = true;
            this.cpc_chat_privatemessages.Name = "cpc_chat_privatemessages";
            this.cpc_chat_privatemessages.ReadOnly = true;
            this.cpc_chat_privatemessages.Size = new System.Drawing.Size(551, 420);
            this.cpc_chat_privatemessages.TabIndex = 2;
            // 
            // cpc_players_ban
            // 
            this.cpc_players_ban.Location = new System.Drawing.Point(5, 31);
            this.cpc_players_ban.Name = "cpc_players_ban";
            this.cpc_players_ban.Size = new System.Drawing.Size(63, 23);
            this.cpc_players_ban.TabIndex = 2;
            this.cpc_players_ban.Text = "Ban";
            this.cpc_players_ban.UseVisualStyleBackColor = true;
            // 
            // cpc_players_promote
            // 
            this.cpc_players_promote.Location = new System.Drawing.Point(162, 3);
            this.cpc_players_promote.Name = "cpc_players_promote";
            this.cpc_players_promote.Size = new System.Drawing.Size(64, 23);
            this.cpc_players_promote.TabIndex = 3;
            this.cpc_players_promote.Text = "Promote";
            this.cpc_players_promote.UseVisualStyleBackColor = true;
            // 
            // cpc_players_demote
            // 
            this.cpc_players_demote.Location = new System.Drawing.Point(162, 31);
            this.cpc_players_demote.Name = "cpc_players_demote";
            this.cpc_players_demote.Size = new System.Drawing.Size(64, 23);
            this.cpc_players_demote.TabIndex = 4;
            this.cpc_players_demote.Text = "Demote";
            this.cpc_players_demote.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(812, 517);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hellion Extended Server GUI";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.cpc_chat_tabs.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
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
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Button server_config_setdefaults;
        private System.Windows.Forms.Button server_config_cancel;
        private System.Windows.Forms.Button server_config_save;
        private System.Windows.Forms.CheckBox server_config_debugmode;
        private System.Windows.Forms.ToolStripStatusLabel StatusBar;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.TreeView cpc_players_treelist;
        private System.Windows.Forms.Button cpc_players_demote;
        private System.Windows.Forms.Button cpc_players_promote;
        private System.Windows.Forms.Button cpc_players_ban;
        private System.Windows.Forms.Button cpc_players_kick;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TabControl cpc_chat_tabs;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox cpc_chat_list;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox cpc_chat_privatemessages;
        private System.Windows.Forms.Button cpc_chat_send;
        private System.Windows.Forms.TextBox cpc_messagebox;
    }
}

