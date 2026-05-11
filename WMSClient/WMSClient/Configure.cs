using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMSClient
{
    public partial class Configure : Form
    {
        public Configure()
        {
            InitializeComponent();
            ServerIP.Text = Properties.Settings.Default.ServerIP;
            ServerPort.Text = Properties.Settings.Default.ServerPort;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 校验端口是否为数字
            if (!int.TryParse(ServerPort.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("端口号必须是1-65535之间的数字！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 校验IP格式（简单校验）
            if (string.IsNullOrWhiteSpace(ServerIP.Text) || !ServerIP.Text.Contains("."))
            {
                MessageBox.Show("请输入有效的服务器IP地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Properties.Settings.Default.ServerIP = ServerIP.Text.Trim();
            Properties.Settings.Default.ServerPort = ServerPort.Text.Trim();
            Properties.Settings.Default.Save(); // 持久化保存

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
