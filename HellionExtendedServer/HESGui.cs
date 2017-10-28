﻿using HellionExtendedServer.Common.GameServerIni;
using HellionExtendedServer.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HellionExtendedServer
{
    public partial class HESGui : Form
    {
        private GameServerSettings Settings = new GameServerSettings();

        public HESGui()
        {
            InitializeComponent();

            cpc_chat_list.Enabled = true;
            cpc_chat_list.ReadOnly = true;

            cpc_messagebox.Enabled = false;
            cpc_chat_send.Enabled = false;
            cpc_players_ban.Enabled = false;
            cpc_players_kick.Enabled = false;

            server_config_stopserver.Enabled = false;
            server_config_startserver.Enabled = true;

            server_config_autostart.Checked = Properties.Settings.Default.AutoStart;
            server_config_debugmode.Checked = Properties.Settings.Default.DebugMode;

            ServerInstance.Instance.OnServerRunning += Instance_OnServerRunning;
            ServerInstance.Instance.OnServerStopped += Instance_OnServerStopped;

            cpc_chat_list.AppendText("Waiting for server to start..\r\n");

            
            SetSettings();
        }

        private void Instance_OnServerStopped(ZeroGravity.Server server)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                cpc_messagebox.Enabled = false;
                cpc_chat_send.Enabled = false;

                server_config_stopserver.Enabled = false;
                server_config_startserver.Enabled = true;

                this.Refresh();
            }));
        }

        private void Instance_OnServerRunning(ZeroGravity.Server server)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                cpc_messagebox.Enabled = true;
                cpc_chat_send.Enabled = true;
                server_config_stopserver.Enabled = true;
                server_config_startserver.Enabled = false;

                this.Refresh();
            }));
        }

        #region Server Control

        private void server_config_save_Click(object sender, EventArgs e)
        {
            //List<Setting> tmp = new List<Setting>();
            foreach (GameServerProperty property in Settings)
            {
                if (ServerInstance.Instance.GameServerProperties.Settings.Exists(x => x.Name == property.Name))
                {
                    Setting setting = ServerInstance.Instance.GameServerProperties.Settings.Find(x => x.Name == property.Name);

                    if (property.Type == setting.Type)
                    {
                        if (property.Value != setting.DefaultValue)
                        {

                            AddChatLine(string.Format($"Changing value of {setting.Name} from {setting.Value} to {property.Value}"));

                            setting.Value = property.Value;
                        
                        }
                    }
                }
            }


            if (ServerInstance.Instance.GameServerProperties.Save())
            {
                StatusBar.Text = "Config Saved.";
            }
            else
            {
                DialogResult result = MessageBox.Show("GameServer.Ini does not exist. Would you like to create one ?",
                    "Server Settings Error",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    ServerInstance.Instance.GameServerProperties.LoadDefaults();
                    ServerInstance.Instance.GameServerProperties.Save();
                    serverconfig_properties.Refresh();
                    StatusBar.Text = "Config Defaults saved to GameServer.Ini. Change the settings then Save!";
                }
            }
        }

        private void SetSettings()
        {
            Settings.Clear();

            foreach (var setting in ServerInstance.Instance.GameServerProperties.Settings)
                Settings.Add(new GameServerProperty(
                    setting.Name,
                    new CultureInfo("en-US").TextInfo.ToTitleCase(setting.Name.Replace("_", " ")),
                    setting.Category, setting.Description, setting.Value, setting.Type, false, true));

            serverconfig_properties.SelectedObject = Settings;

            serverconfig_properties.Refresh();
        }

        private void server_config_cancel_Click(object sender, EventArgs e)
        {
            if (ServerInstance.Instance.GameServerProperties.Settings == null)
                ServerInstance.Instance.GameServerProperties.Load();

            SetSettings();
            StatusBar.Text = "Reloaded the config from the GameServer.ini.";
        }

        private void server_config_setdefaults_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to load the default settings?",
                   "Server Config Settings",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                ServerInstance.Instance.GameServerProperties.LoadDefaults();

                SetSettings();
                StatusBar.Text = "Config defaults loaded.";
            }
        }

        private void server_config_reload_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to reload the settings from the GameServer.Ini?",
                   "Server Settings Error",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                ServerInstance.Instance.GameServerProperties.Load();
                SetSettings();
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
    }
}