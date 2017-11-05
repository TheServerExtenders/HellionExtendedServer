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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Players");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HESGui));
            this.Tabs = new System.Windows.Forms.TabControl();
            this.ServerTab = new System.Windows.Forms.TabPage();
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
            this.ChatTab = new System.Windows.Forms.TabPage();
            this.ChatPlayerContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.cpc_chat_tabs = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cpc_chat_list = new System.Windows.Forms.TextBox();
            this.cpc_chat_send = new System.Windows.Forms.Button();
            this.cpc_messagebox = new System.Windows.Forms.TextBox();
            this.ObjectManipulationTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectManipulation_treeview = new System.Windows.Forms.TreeView();
            this.objectManipulation_grid = new System.Windows.Forms.PropertyGrid();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.PlayersTab = new System.Windows.Forms.TabPage();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pc_showOnlinePlayersOnly_checkbox = new System.Windows.Forms.CheckBox();
            this.pc_players_listview = new System.Windows.Forms.ListView();
            this.pc_demoteplayer = new System.Windows.Forms.Button();
            this.pc_promoteplayer = new System.Windows.Forms.Button();
            this.pc_banplayer = new System.Windows.Forms.Button();
            this.pc_kickplayer = new System.Windows.Forms.Button();
            this.splitContainer8 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.sc_playerslist_listview = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.sc_onlineplayers_label = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.HESWIKI = new System.Windows.Forms.TabPage();
            this.hesw_Website = new System.Windows.Forms.WebBrowser();
            this.Tabs.SuspendLayout();
            this.ServerTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServerContainer)).BeginInit();
            this.ServerContainer.Panel1.SuspendLayout();
            this.ServerContainer.Panel2.SuspendLayout();
            this.ServerContainer.SuspendLayout();
            this.ChatTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChatPlayerContainer)).BeginInit();
            this.ChatPlayerContainer.Panel1.SuspendLayout();
            this.ChatPlayerContainer.Panel2.SuspendLayout();
            this.ChatPlayerContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.cpc_chat_tabs.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.ObjectManipulationTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.PlayersTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel1.SuspendLayout();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).BeginInit();
            this.splitContainer8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.HESWIKI.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.ServerTab);
            this.Tabs.Controls.Add(this.ChatTab);
            this.Tabs.Controls.Add(this.PlayersTab);
            this.Tabs.Controls.Add(this.ObjectManipulationTab);
            this.Tabs.Controls.Add(this.HESWIKI);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabs.Location = new System.Drawing.Point(0, 0);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(780, 495);
            this.Tabs.TabIndex = 0;
            this.Tabs.Selected += new System.Windows.Forms.TabControlEventHandler(this.Tabs_Selected);
            // 
            // ServerTab
            // 
            this.ServerTab.Controls.Add(this.ServerContainer);
            this.ServerTab.Location = new System.Drawing.Point(4, 22);
            this.ServerTab.Name = "ServerTab";
            this.ServerTab.Padding = new System.Windows.Forms.Padding(3);
            this.ServerTab.Size = new System.Drawing.Size(772, 469);
            this.ServerTab.TabIndex = 0;
            this.ServerTab.Text = "Server";
            this.ServerTab.UseVisualStyleBackColor = true;
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
            this.ServerContainer.Panel2.Controls.Add(this.checkBox4);
            this.ServerContainer.Panel2.Controls.Add(this.checkBox3);
            this.ServerContainer.Panel2.Controls.Add(this.checkBox1);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_reload);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_debugmode);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_setdefaults);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_cancel);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_save);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_autostart);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_stopserver);
            this.ServerContainer.Panel2.Controls.Add(this.server_config_startserver);
            this.ServerContainer.Size = new System.Drawing.Size(766, 463);
            this.ServerContainer.SplitterDistance = 385;
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
            this.serverconfig_properties.Size = new System.Drawing.Size(766, 385);
            this.serverconfig_properties.TabIndex = 0;
            this.serverconfig_properties.ViewBorderColor = System.Drawing.SystemColors.ControlLight;
            // 
            // server_config_reload
            // 
            this.server_config_reload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.server_config_reload.Location = new System.Drawing.Point(546, 44);
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
            this.server_config_debugmode.Location = new System.Drawing.Point(267, 6);
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
            this.server_config_setdefaults.Location = new System.Drawing.Point(546, 7);
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
            this.server_config_cancel.Location = new System.Drawing.Point(685, 42);
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
            this.server_config_save.Location = new System.Drawing.Point(685, 6);
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
            this.server_config_autostart.Location = new System.Drawing.Point(370, 7);
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
            this.server_config_stopserver.Location = new System.Drawing.Point(3, 45);
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
            this.server_config_startserver.Location = new System.Drawing.Point(4, 6);
            this.server_config_startserver.Name = "server_config_startserver";
            this.server_config_startserver.Size = new System.Drawing.Size(75, 23);
            this.server_config_startserver.TabIndex = 0;
            this.server_config_startserver.Text = "Start Server";
            this.server_config_startserver.UseVisualStyleBackColor = true;
            this.server_config_startserver.Click += new System.EventHandler(this.server_config_startserver_Click);
            // 
            // ChatTab
            // 
            this.ChatTab.Controls.Add(this.ChatPlayerContainer);
            this.ChatTab.Location = new System.Drawing.Point(4, 22);
            this.ChatTab.Name = "ChatTab";
            this.ChatTab.Padding = new System.Windows.Forms.Padding(3);
            this.ChatTab.Size = new System.Drawing.Size(772, 469);
            this.ChatTab.TabIndex = 1;
            this.ChatTab.Text = "Server Chat";
            this.ChatTab.UseVisualStyleBackColor = true;
            // 
            // ChatPlayerContainer
            // 
            this.ChatPlayerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatPlayerContainer.Location = new System.Drawing.Point(3, 3);
            this.ChatPlayerContainer.Name = "ChatPlayerContainer";
            // 
            // ChatPlayerContainer.Panel1
            // 
            this.ChatPlayerContainer.Panel1.Controls.Add(this.splitContainer2);
            // 
            // ChatPlayerContainer.Panel2
            // 
            this.ChatPlayerContainer.Panel2.Controls.Add(this.splitContainer3);
            this.ChatPlayerContainer.Size = new System.Drawing.Size(766, 463);
            this.ChatPlayerContainer.SplitterDistance = 204;
            this.ChatPlayerContainer.TabIndex = 0;
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
            this.splitContainer3.Size = new System.Drawing.Size(558, 463);
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
            this.cpc_chat_tabs.Size = new System.Drawing.Size(558, 431);
            this.cpc_chat_tabs.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cpc_chat_list);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(550, 405);
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
            this.cpc_chat_list.Size = new System.Drawing.Size(544, 399);
            this.cpc_chat_list.TabIndex = 2;
            // 
            // cpc_chat_send
            // 
            this.cpc_chat_send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cpc_chat_send.Location = new System.Drawing.Point(507, 3);
            this.cpc_chat_send.Name = "cpc_chat_send";
            this.cpc_chat_send.Size = new System.Drawing.Size(50, 23);
            this.cpc_chat_send.TabIndex = 1;
            this.cpc_chat_send.Text = "Send";
            this.cpc_chat_send.UseVisualStyleBackColor = true;
            this.cpc_chat_send.Click += new System.EventHandler(this.cpc_chat_send_Click);
            // 
            // cpc_messagebox
            // 
            this.cpc_messagebox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cpc_messagebox.Location = new System.Drawing.Point(3, 5);
            this.cpc_messagebox.Name = "cpc_messagebox";
            this.cpc_messagebox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.cpc_messagebox.Size = new System.Drawing.Size(498, 20);
            this.cpc_messagebox.TabIndex = 0;
            this.cpc_messagebox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cpc_messagebox_KeyDown);
            // 
            // ObjectManipulationTab
            // 
            this.ObjectManipulationTab.Controls.Add(this.splitContainer1);
            this.ObjectManipulationTab.Location = new System.Drawing.Point(4, 22);
            this.ObjectManipulationTab.Name = "ObjectManipulationTab";
            this.ObjectManipulationTab.Padding = new System.Windows.Forms.Padding(3);
            this.ObjectManipulationTab.Size = new System.Drawing.Size(772, 469);
            this.ObjectManipulationTab.TabIndex = 2;
            this.ObjectManipulationTab.Text = "Object Manipulation";
            this.ObjectManipulationTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.objectManipulation_treeview);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.objectManipulation_grid);
            this.splitContainer1.Size = new System.Drawing.Size(766, 463);
            this.splitContainer1.SplitterDistance = 262;
            this.splitContainer1.TabIndex = 0;
            // 
            // objectManipulation_treeview
            // 
            this.objectManipulation_treeview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectManipulation_treeview.Location = new System.Drawing.Point(0, 0);
            this.objectManipulation_treeview.Name = "objectManipulation_treeview";
            treeNode1.Name = "PlayersNode";
            treeNode1.Text = "Players";
            this.objectManipulation_treeview.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.objectManipulation_treeview.Size = new System.Drawing.Size(262, 463);
            this.objectManipulation_treeview.TabIndex = 0;
            this.objectManipulation_treeview.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.objectManipulation_treeview_AfterSelect);
            this.objectManipulation_treeview.Click += new System.EventHandler(this.objectManipulation_treeview_Click);
            // 
            // objectManipulation_grid
            // 
            this.objectManipulation_grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectManipulation_grid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.objectManipulation_grid.Location = new System.Drawing.Point(0, 0);
            this.objectManipulation_grid.Name = "objectManipulation_grid";
            this.objectManipulation_grid.Size = new System.Drawing.Size(500, 463);
            this.objectManipulation_grid.TabIndex = 0;
            this.objectManipulation_grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.objectManipulation_grid_PropertyValueChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 495);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(780, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            // 
            // StatusBar
            // 
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(0, 17);
            // 
            // PlayersTab
            // 
            this.PlayersTab.Controls.Add(this.splitContainer5);
            this.PlayersTab.Location = new System.Drawing.Point(4, 22);
            this.PlayersTab.Name = "PlayersTab";
            this.PlayersTab.Padding = new System.Windows.Forms.Padding(3);
            this.PlayersTab.Size = new System.Drawing.Size(772, 469);
            this.PlayersTab.TabIndex = 3;
            this.PlayersTab.Text = "Player Control";
            this.PlayersTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(3, 3);
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.splitContainer6);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.splitContainer8);
            this.splitContainer5.Size = new System.Drawing.Size(766, 463);
            this.splitContainer5.SplitterDistance = 247;
            this.splitContainer5.TabIndex = 1;
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.Location = new System.Drawing.Point(0, 0);
            this.splitContainer6.Name = "splitContainer6";
            this.splitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.splitContainer7);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.pc_demoteplayer);
            this.splitContainer6.Panel2.Controls.Add(this.pc_promoteplayer);
            this.splitContainer6.Panel2.Controls.Add(this.pc_banplayer);
            this.splitContainer6.Panel2.Controls.Add(this.pc_kickplayer);
            this.splitContainer6.Size = new System.Drawing.Size(247, 463);
            this.splitContainer6.SplitterDistance = 369;
            this.splitContainer6.TabIndex = 0;
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.Location = new System.Drawing.Point(0, 0);
            this.splitContainer7.Name = "splitContainer7";
            this.splitContainer7.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.pc_players_listview);
            this.splitContainer7.Size = new System.Drawing.Size(247, 369);
            this.splitContainer7.SplitterDistance = 25;
            this.splitContainer7.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pc_showOnlinePlayersOnly_checkbox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(247, 25);
            this.panel2.TabIndex = 0;
            // 
            // pc_showOnlinePlayersOnly_checkbox
            // 
            this.pc_showOnlinePlayersOnly_checkbox.AutoSize = true;
            this.pc_showOnlinePlayersOnly_checkbox.Checked = true;
            this.pc_showOnlinePlayersOnly_checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pc_showOnlinePlayersOnly_checkbox.Location = new System.Drawing.Point(5, 5);
            this.pc_showOnlinePlayersOnly_checkbox.Name = "pc_showOnlinePlayersOnly_checkbox";
            this.pc_showOnlinePlayersOnly_checkbox.Size = new System.Drawing.Size(147, 17);
            this.pc_showOnlinePlayersOnly_checkbox.TabIndex = 0;
            this.pc_showOnlinePlayersOnly_checkbox.Text = "Show Online Players Only";
            this.pc_showOnlinePlayersOnly_checkbox.UseVisualStyleBackColor = true;
            // 
            // pc_players_listview
            // 
            this.pc_players_listview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pc_players_listview.Location = new System.Drawing.Point(0, 0);
            this.pc_players_listview.Name = "pc_players_listview";
            this.pc_players_listview.Size = new System.Drawing.Size(247, 340);
            this.pc_players_listview.TabIndex = 0;
            this.pc_players_listview.UseCompatibleStateImageBehavior = false;
            this.pc_players_listview.View = System.Windows.Forms.View.Details;
            // 
            // pc_demoteplayer
            // 
            this.pc_demoteplayer.Enabled = false;
            this.pc_demoteplayer.Location = new System.Drawing.Point(162, 31);
            this.pc_demoteplayer.Name = "pc_demoteplayer";
            this.pc_demoteplayer.Size = new System.Drawing.Size(64, 23);
            this.pc_demoteplayer.TabIndex = 4;
            this.pc_demoteplayer.Text = "Demote";
            this.pc_demoteplayer.UseVisualStyleBackColor = true;
            // 
            // pc_promoteplayer
            // 
            this.pc_promoteplayer.Enabled = false;
            this.pc_promoteplayer.Location = new System.Drawing.Point(162, 3);
            this.pc_promoteplayer.Name = "pc_promoteplayer";
            this.pc_promoteplayer.Size = new System.Drawing.Size(64, 23);
            this.pc_promoteplayer.TabIndex = 3;
            this.pc_promoteplayer.Text = "Promote";
            this.pc_promoteplayer.UseVisualStyleBackColor = true;
            // 
            // pc_banplayer
            // 
            this.pc_banplayer.Location = new System.Drawing.Point(5, 31);
            this.pc_banplayer.Name = "pc_banplayer";
            this.pc_banplayer.Size = new System.Drawing.Size(63, 23);
            this.pc_banplayer.TabIndex = 2;
            this.pc_banplayer.Text = "Ban";
            this.pc_banplayer.UseVisualStyleBackColor = true;
            // 
            // pc_kickplayer
            // 
            this.pc_kickplayer.Location = new System.Drawing.Point(5, 3);
            this.pc_kickplayer.Name = "pc_kickplayer";
            this.pc_kickplayer.Size = new System.Drawing.Size(63, 23);
            this.pc_kickplayer.TabIndex = 1;
            this.pc_kickplayer.Text = "Kick";
            this.pc_kickplayer.UseVisualStyleBackColor = true;
            // 
            // splitContainer8
            // 
            this.splitContainer8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer8.Location = new System.Drawing.Point(0, 0);
            this.splitContainer8.Name = "splitContainer8";
            this.splitContainer8.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer8.Size = new System.Drawing.Size(515, 463);
            this.splitContainer8.SplitterDistance = 212;
            this.splitContainer8.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.sc_onlineplayers_label);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer2.Size = new System.Drawing.Size(204, 463);
            this.splitContainer2.SplitterDistance = 30;
            this.splitContainer2.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(204, 30);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
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
            this.splitContainer4.Panel1.Controls.Add(this.sc_playerslist_listview);
            this.splitContainer4.Size = new System.Drawing.Size(204, 429);
            this.splitContainer4.SplitterDistance = 393;
            this.splitContainer4.TabIndex = 0;
            // 
            // sc_playerslist_listview
            // 
            this.sc_playerslist_listview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sc_playerslist_listview.Location = new System.Drawing.Point(0, 0);
            this.sc_playerslist_listview.Name = "sc_playerslist_listview";
            this.sc_playerslist_listview.Size = new System.Drawing.Size(204, 393);
            this.sc_playerslist_listview.TabIndex = 0;
            this.sc_playerslist_listview.UseCompatibleStateImageBehavior = false;
            this.sc_playerslist_listview.View = System.Windows.Forms.View.List;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current Online Players: ";
            // 
            // sc_onlineplayers_label
            // 
            this.sc_onlineplayers_label.AutoSize = true;
            this.sc_onlineplayers_label.Location = new System.Drawing.Point(119, 9);
            this.sc_onlineplayers_label.Name = "sc_onlineplayers_label";
            this.sc_onlineplayers_label.Size = new System.Drawing.Size(13, 13);
            this.sc_onlineplayers_label.TabIndex = 2;
            this.sc_onlineplayers_label.Text = "0";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(370, 46);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(111, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "Auto-Update HES";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(370, 26);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(173, 17);
            this.checkBox3.TabIndex = 8;
            this.checkBox3.Text = "Auto-Update Hellion Dedicated";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(94, 6);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(142, 17);
            this.checkBox4.TabIndex = 9;
            this.checkBox4.Text = "Disable UI on Next Load";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // HESWIKI
            // 
            this.HESWIKI.Controls.Add(this.hesw_Website);
            this.HESWIKI.Location = new System.Drawing.Point(4, 22);
            this.HESWIKI.Name = "HESWIKI";
            this.HESWIKI.Padding = new System.Windows.Forms.Padding(3);
            this.HESWIKI.Size = new System.Drawing.Size(772, 469);
            this.HESWIKI.TabIndex = 4;
            this.HESWIKI.Text = "Hellion Extended Server Website";
            this.HESWIKI.UseVisualStyleBackColor = true;
            // 
            // hesw_Website
            // 
            this.hesw_Website.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hesw_Website.IsWebBrowserContextMenuEnabled = false;
            this.hesw_Website.Location = new System.Drawing.Point(3, 3);
            this.hesw_Website.MinimumSize = new System.Drawing.Size(20, 20);
            this.hesw_Website.Name = "hesw_Website";
            this.hesw_Website.ScriptErrorsSuppressed = true;
            this.hesw_Website.Size = new System.Drawing.Size(766, 463);
            this.hesw_Website.TabIndex = 0;
            this.hesw_Website.Url = new System.Uri("https://hellionextendedserver.com/", System.UriKind.Absolute);
            // 
            // HESGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(780, 517);
            this.Controls.Add(this.Tabs);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HESGui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "-";
            this.Tabs.ResumeLayout(false);
            this.ServerTab.ResumeLayout(false);
            this.ServerContainer.Panel1.ResumeLayout(false);
            this.ServerContainer.Panel2.ResumeLayout(false);
            this.ServerContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServerContainer)).EndInit();
            this.ServerContainer.ResumeLayout(false);
            this.ChatTab.ResumeLayout(false);
            this.ChatPlayerContainer.Panel1.ResumeLayout(false);
            this.ChatPlayerContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ChatPlayerContainer)).EndInit();
            this.ChatPlayerContainer.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.cpc_chat_tabs.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ObjectManipulationTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.PlayersTab.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
            this.splitContainer6.ResumeLayout(false);
            this.splitContainer7.Panel1.ResumeLayout(false);
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).EndInit();
            this.splitContainer8.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.HESWIKI.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage ServerTab;
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
        private System.Windows.Forms.TabPage ChatTab;
        private System.Windows.Forms.SplitContainer ChatPlayerContainer;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TabControl cpc_chat_tabs;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox cpc_chat_list;
        private System.Windows.Forms.Button cpc_chat_send;
        private System.Windows.Forms.TextBox cpc_messagebox;
        public System.Windows.Forms.Button server_config_startserver;
        private System.Windows.Forms.Button server_config_reload;
        private System.Windows.Forms.TabPage ObjectManipulationTab;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView objectManipulation_treeview;
        private System.Windows.Forms.PropertyGrid objectManipulation_grid;
        private System.Windows.Forms.TabPage PlayersTab;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label sc_onlineplayers_label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.ListView sc_playerslist_listview;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.SplitContainer splitContainer6;
        private System.Windows.Forms.SplitContainer splitContainer7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox pc_showOnlinePlayersOnly_checkbox;
        private System.Windows.Forms.ListView pc_players_listview;
        private System.Windows.Forms.Button pc_demoteplayer;
        private System.Windows.Forms.Button pc_promoteplayer;
        private System.Windows.Forms.Button pc_banplayer;
        private System.Windows.Forms.Button pc_kickplayer;
        private System.Windows.Forms.SplitContainer splitContainer8;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TabPage HESWIKI;
        private System.Windows.Forms.WebBrowser hesw_Website;
    }
}

