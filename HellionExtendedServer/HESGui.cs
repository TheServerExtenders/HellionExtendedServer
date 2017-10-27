using System;
using System.Collections.Generic;
using HellionExtendedServer.Common;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Common.GameServerIni;
using HellionExtendedServer.Common.GameServerIni;
using System.Globalization;

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
            this.Invoke(new MethodInvoker(delegate {

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
            ServerInstance.Instance.GameServerProperties.Save();
            /*
            if ()
            {
                StatusBar.Text = "Config Saved.";
            }else
            {
                DialogResult result = MessageBox.Show("GameServer.Ini does not exist. Would you like to create one ?", 
                    "Server Settings Error", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1  );               
                if(result == DialogResult.Yes)
                {
                    ServerInstance.Instance.Config.LoadDefaults();
                    ServerInstance.Instance.Config.Save(true, false);
                    serverconfig_properties.Refresh();
                    StatusBar.Text = "Config Defaults saved to GameServer.Ini. Change the settings then Save!";
                }
            }
            */
        }

        private void SetSettings()
        {
            Settings.Clear();
            ServerInstance.Instance.GameServerProperties.Load();

            foreach (var property in ServerInstance.Instance.GameServerProperties.Settings)
                Settings.Add(new GameServerProperty(
                    property.Name,
                    new CultureInfo("en-US").TextInfo.ToTitleCase(property.Name.Replace("_", " ")),
                    property.Category, property.Description, property.Value, property.Type, false, true));

            serverconfig_properties.SelectedObject = Settings;

            serverconfig_properties.Refresh();
        }

        private void server_config_cancel_Click(object sender, EventArgs e)
        {
            ServerInstance.Instance.Config.Load();
            serverconfig_properties.Refresh();
        }

        private void server_config_setdefaults_Click(object sender, EventArgs e)
        {
            SetSettings();

            Settings = new GameServerSettings();

            ServerInstance.Instance.GameServerProperties.LoadDefaults();

            foreach (var property in ServerInstance.Instance.GameServerProperties.DefaultSettings)
                Settings.Add(new GameServerProperty(
                    property.Name,
                    new CultureInfo("en-US").TextInfo.ToTitleCase(property.Name.Replace("_", " ")),
                    property.Category, property.Description, property.Value, property.Type, false, true));

            serverconfig_properties.SelectedObject = Settings;

            serverconfig_properties.Refresh();

            StatusBar.Text = "Config Defaults Loaded.";
        }

        private void server_config_reload_Click(object sender, EventArgs e)
        {
            SetSettings();
            StatusBar.Text = "Config reloaded from GameServer.ini";
        }

        private void server_config_startserver_Click(object sender, EventArgs e)
        {
            ServerInstance.Instance.Start();
            StatusBar.Text = "Server Started";
        }

        private void server_config_stopserver_Click(object sender, EventArgs e)
        {
            if (ZeroGravity.Server.IsRunning)
            {
                ServerInstance.Instance.Stop();
                StatusBar.Text = "Server Stopped";
            }
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

                cpc_chat_list.Invoke(new MethodInvoker(delegate {
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
            if(e.KeyCode == Keys.Enter & !String.IsNullOrEmpty(cpc_messagebox.Text))
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

        #endregion Chat and Players

        private void Default_SettingsSaving(object sender, CancelEventArgs e)
        {
            StatusBar.Text = "GUI Settings Changed";
        }


    }
}
