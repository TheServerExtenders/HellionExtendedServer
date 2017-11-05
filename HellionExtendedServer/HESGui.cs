using HellionExtendedServer.GUI.Objects;
using HellionExtendedServer.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ZeroGravity;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace HellionExtendedServer
{
    public partial class HESGui : Form
    {
        private Timer ObjectManipulationRefreshTimer = new Timer();
        private Timer PlayersRefreshTimer = new Timer();

        public HESGui()
        {
            InitializeComponent();

            DisableControls();

            ServerInstance.Instance.OnServerRunning += Instance_OnServerRunning;
            ServerInstance.Instance.OnServerStopped += Instance_OnServerStopped;

            ServerInstance.Instance.GameServerConfig.Load();
            serverconfig_properties.SelectedObject = ServerInstance.Instance.GameServerConfig;

            serverconfig_properties.Refresh();
        }

        private void DisableControls(bool disable = true)
        {
            cpc_chat_list.ReadOnly = true;
            cpc_chat_list.BackColor = System.Drawing.SystemColors.Window;

            cpc_messagebox.Enabled = !disable;
            cpc_chat_send.Enabled = !disable;

            pc_banplayer.Enabled = !disable;
            pc_kickplayer.Enabled = !disable;

            sc_playerslist_listview.Enabled = !disable;

            server_config_stopserver.Enabled = !disable;
            server_config_startserver.Enabled = disable;

            objectManipulation_grid.Enabled = !disable;
            objectManipulation_treeview.Enabled = !disable;

            server_config_autostart.Checked = Properties.Settings.Default.AutoStart;
            server_config_debugmode.Checked = Properties.Settings.Default.DebugMode;

            cpc_chat_list.AppendText("Waiting for server to start..\r\n");
        }

        #region Events
        private void Instance_OnServerStopped(ZeroGravity.Server server)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                DisableControls();

                ObjectManipulationRefreshTimer.Stop();
                ObjectManipulationRefreshTimer.Enabled = false;

                PlayersRefreshTimer.Stop();
                PlayersRefreshTimer.Enabled = false;

                this.Refresh();
            }));
        }

        public void UpdateChatPlayers()
        {
            if (ServerInstance.Instance.Server == null)
                return;

            if (NetworkManager.Instance == null)
                return;

            if (!ZeroGravity.Server.IsRunning)
                return;

            sc_onlineplayers_label.Text = NetworkManager.Instance.ClientList.Count.ToString();


            foreach (var client in NetworkManager.Instance.ClientList)
            {
                if (client.Value == null)
                    continue;

                if (client.Value.Player == null)
                    continue;

                var player = client.Value.Player;

                var item = new ListViewItem();
                item.Name = player.GUID.ToString();
                item.Tag = client;
                item.Text = $"{player.Name} ({player.SteamId})";

                if(!sc_playerslist_listview.Items.Contains(item))
                    sc_playerslist_listview.Items.Add(item);

                if (!pc_players_listview.Items.Contains(item))
                    pc_players_listview.Items.Add(item);
            }

            // chat players
            foreach (ListViewItem item in sc_playerslist_listview.Items)
            {
                var _client = (NetworkController.Client)item.Tag;
                var _player = _client.Player;

                if (!NetworkManager.Instance.ClientList.Values.Contains(_client))
                {
                    if (sc_playerslist_listview.Items.ContainsKey(_player.GUID.ToString()))
                        sc_playerslist_listview.Items.RemoveByKey(_player.GUID.ToString());
                }
           
            }

            // player control players
            foreach (ListViewItem item in pc_players_listview.Items)
            {
                var _client = (NetworkController.Client)item.Tag;
                var _player = _client.Player;

                if (!NetworkManager.Instance.ClientList.Values.Contains(_client))
                {
                    if (pc_players_listview.Items.ContainsKey(_player.GUID.ToString()))
                        pc_players_listview.Items.RemoveByKey(_player.GUID.ToString());
                }

            }

        }

        private void Instance_OnServerRunning(ZeroGravity.Server server)
        {
            Invoke(new MethodInvoker(delegate
            {
                DisableControls(false);

                UpdatePlayersTree();
                UpdateChatPlayers();

                this.Refresh();
            }));

            ObjectManipulationRefreshTimer.Interval = (1000); // 1 secs
            ObjectManipulationRefreshTimer.Tick += delegate (object sender, EventArgs e)
            {
                UpdatePlayersTree();
            };
            ObjectManipulationRefreshTimer.Enabled = true;
            ObjectManipulationRefreshTimer.Start();

            PlayersRefreshTimer.Interval = (1000); // 1 secs
            PlayersRefreshTimer.Tick += delegate (object sender, EventArgs e)
            {
                UpdateChatPlayers();
            };
            PlayersRefreshTimer.Enabled = true;
            PlayersRefreshTimer.Start();

            
        }
        #endregion Events

        #region Object Manipulation

        public List<MyPlayer> MyPlayers = new List<MyPlayer>();

        public void UpdatePlayersTree()
        {
            if (ServerInstance.Instance.Server == null)
                return;

            if (NetworkManager.Instance == null)
                return;

            if (!ZeroGravity.Server.IsRunning)
                return;

            var treeNodeList = objectManipulation_treeview.Nodes[0].Nodes;

            // Convert the Games Players into something the GUI can use

            var playersList = NetworkManager.Instance.ClientList;

            foreach (var client in NetworkManager.Instance.ClientList)
            {
                var player = client.Value.Player;

                if (!MyPlayers.Exists(x => x.GUID == player.GUID))
                    MyPlayers.Add(new MyPlayer(player));
            }

            // Remove the player
            foreach (var _player in MyPlayers)
            {
                Player player = _player.CurrentPlayer;

                if (player == null)
                {
                    if (MyPlayers.Exists(x => x.GUID == _player.GUID))
                        MyPlayers.Remove(_player);

                    if (treeNodeList.ContainsKey(_player.GUID.ToString()))
                        treeNodeList.RemoveByKey(_player.GUID.ToString());

                    objectManipulation_treeview.Refresh();
                    objectManipulation_grid.Refresh();

                    continue;
                }

                TreeNode node = new TreeNode
                {
                    Name = player.GUID.ToString(),
                    Text = player.Name + $" ({player.SteamId})",

                    Tag = _player
                };

                if (!treeNodeList.ContainsKey(node.Name))
                    treeNodeList.Add(node);
            }

            foreach (TreeNode node in treeNodeList)
            {
                MyPlayer tag = node.Tag as MyPlayer;

                if (tag == null)
                    continue;

                var player = GetPlayerFromGuid(tag.GUID);

                if (player == null)
                {
                    treeNodeList.Remove(node);
                }
            }

            objectManipulation_treeview.Refresh();
            objectManipulation_grid.Refresh();
        }

        private void objectManipulation_treeview_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;

            if (node == null)
                return;

            if (node.Tag == null)
                return;

            objectManipulation_grid.SelectedObject = node.Tag as MyPlayer;
            objectManipulation_grid.Refresh();
        }

        private void objectManipulation_treeview_Click(object sender, EventArgs e)
        {
            UpdatePlayersTree();
        }

        public ZeroGravity.Objects.Player GetPlayerFromGuid(long guid)
        {
            foreach (var player in ServerInstance.Instance.Server.AllPlayers)
            {
                if (player.GUID == guid) return player;
            }
            return null;
        }

        #endregion Object Manipulation

        #region Server Control

        private void server_config_save_Click(object sender, EventArgs e)
        {
            if (ServerInstance.Instance.GameServerConfig.Save())
            {
                StatusBar.Text = "Config Saved.";
            }
        }

        private void server_config_cancel_Click(object sender, EventArgs e)
        {
            ServerInstance.Instance.GameServerConfig.Load();
            serverconfig_properties.SelectedObject = ServerInstance.Instance.GameServerConfig;

            serverconfig_properties.Refresh();
            StatusBar.Text = "Reloaded the config from the GameServer.ini.";
        }

        private void server_config_setdefaults_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to load the default settings?",
                   "Server Config Settings",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                ServerInstance.Instance.GameServerConfig.LoadDefaults();
                serverconfig_properties.SelectedObject = ServerInstance.Instance.GameServerConfig;
                serverconfig_properties.Refresh();
                StatusBar.Text = "Config defaults loaded.";
            }
        }

        private void server_config_reload_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to reload the settings from the GameServer.Ini?",
                   "2. Server Settings Error",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                ServerInstance.Instance.GameServerConfig.Load();
                serverconfig_properties.SelectedObject = ServerInstance.Instance.GameServerConfig;
                serverconfig_properties.Refresh();
                StatusBar.Text = "Config reloaded from GameServer.ini";
            }
        }

        private void server_config_startserver_Click(object sender, EventArgs e)
        {
            if (!ZeroGravity.Server.IsRunning)
            {
                ServerInstance.Instance.Start();
                StatusBar.Text = "Server Started";
            }
            else
                StatusBar.Text = "The server is already running!";
        }

        private void server_config_stopserver_Click(object sender, EventArgs e)
        {
            if (ZeroGravity.Server.IsRunning)
            {
                ServerInstance.Instance.Stop();
                StatusBar.Text = "Server Stopped";
            }
            else
                StatusBar.Text = "The server is already stopped!";
        }

        private void server_config_autostart_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoStart = server_config_autostart.Checked;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.SettingsSaving += Default_SettingsSaving;
        }

        private void server_config_debugmode_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DebugMode = server_config_debugmode.Checked;
            Properties.Settings.Default.Save();
        }

        #endregion Server Control

        #region Chat And Players

        public void AddChatLine(string line)
        {
            if (cpc_chat_list.InvokeRequired)
            {
                cpc_chat_list.Invoke(new MethodInvoker(delegate
                {
                    cpc_chat_list.AppendText(line + Environment.NewLine);
                }));

                return;
            }
            else
                cpc_chat_list.AppendText(line + Environment.NewLine);
        }

        private void cpc_players_kick_Click(object sender, EventArgs e)
        {
        }

        private void cpc_players_ban_Click(object sender, EventArgs e)
        {
        }

        private void cpc_messagebox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter & !String.IsNullOrEmpty(cpc_messagebox.Text))
            {
                NetworkManager.Instance.MessageAllClients(cpc_messagebox.Text, true, true);
                cpc_messagebox.Text = "";

                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void cpc_chat_send_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(cpc_messagebox.Text))
            {
                NetworkManager.Instance.MessageAllClients(cpc_messagebox.Text, true, true);
                cpc_messagebox.Text = "";
            }
        }

        #endregion Chat And Players

        private void Default_SettingsSaving(object sender, CancelEventArgs e)
        {
            StatusBar.Text = "GUI Settings Changed";
        }

        private void objectManipulation_grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        
        }

        private void Tabs_Selected(object sender, TabControlEventArgs e)
        {
            ObjectManipulationRefreshTimer.Enabled = false;
            PlayersRefreshTimer.Enabled = false;

            switch (e.TabPageIndex)
            {
                case 0:
                    break;
                case 1:
                    PlayersRefreshTimer.Enabled = true;

                    break;
                case 2:
                    PlayersRefreshTimer.Enabled = true;
                    break;
                case 3:                    
                        ObjectManipulationRefreshTimer.Enabled = true;                 
                    break;

                default:
                    break;

                    
            }
            
        }
    }
}