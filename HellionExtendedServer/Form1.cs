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
        }

        private void server_config_save_Click(object sender, EventArgs e)
        {
            ServerInstance.Instance.Config.Save();
        }

        private void server_config_cancel_Click(object sender, EventArgs e)
        {
            ServerInstance.Instance.Config.Load();
        }

        private void server_config_setdefaults_Click(object sender, EventArgs e)
        {
            ServerInstance.Instance.Config.LoadDefaults();
        }



        private void server_config_startserver_Click(object sender, EventArgs e)
        {

        }

        private void server_config_stopserver_Click(object sender, EventArgs e)
        {

        }


        private void server_config_autostart_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoStart = server_config_autostart.Checked;
            Properties.Settings.Default.Save();
        }

        private void server_config_debugmode_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DebugMode = server_config_debugmode.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
