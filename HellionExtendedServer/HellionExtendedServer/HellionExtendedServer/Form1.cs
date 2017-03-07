using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HellionExtendedServer.Managers;

namespace HellionExtendedServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            serverconfig_properties.SelectedObject = ServerInstance.Instance.Config;

            if (Properties.Settings.Default.AutoStart)
                ServerInstance.Instance.Start();
            
            server_config_autostart.Checked = Properties.Settings.Default.AutoStart;
            server_config_debugmode.Checked = Properties.Settings.Default.DebugMode;
        }

        private void server_config_save_Click(object sender, EventArgs e)
        {
            if (ServerInstance.Instance.Config.Save())
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
            
        }

        private void server_config_cancel_Click(object sender, EventArgs e)
        {
            ServerInstance.Instance.Config.Load();
            serverconfig_properties.Refresh();
        }

        private void server_config_setdefaults_Click(object sender, EventArgs e)
        {
            ServerInstance.Instance.Config.LoadDefaults();
            serverconfig_properties.Refresh();
            StatusBar.Text = "Config Defaults Loaded.";
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

        private void Default_SettingsSaving(object sender, CancelEventArgs e)
        {
            StatusBar.Text = "GUI Settings Changed";
        }


        private void server_config_debugmode_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DebugMode = server_config_debugmode.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
