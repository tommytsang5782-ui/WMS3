using System;
using System.Net.Sockets;
using System.Windows.Forms;
using WMSClient.Class;

namespace WMSClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 全局异常捕获：捕获所有未处理的异常
            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show($"程序发生未处理异常：{e.Exception.Message}\n{e.Exception.StackTrace}",
                    "致命错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // 主流程：欢迎窗体 → 登录 → 主窗体
            SocketConnect socketConnect = null;
            try
            {
                // ========== 步骤1：打开欢迎窗体，获取Socket连接 ==========
                using (var welcome = new Welcome())
                {
                    // 显示欢迎窗体，等待用户操作
                    if (welcome.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show("用户取消连接，程序将退出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // 获取裸Socket并校验连接状态
                    socketConnect = welcome.GetSocketConnect();
                    if (socketConnect == null || !socketConnect.IsConnected())
                    {
                        MessageBox.Show("Socket连接失败，程序将退出", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //// 封装为SocketConnect实例（统一管理）
                    //socketConnect = new SocketConnect();
                    //socketConnect.SetSocket(rawSocket); // 需在SocketConnect中实现SetSocket
                    //socketConnect.initStream();        // 需在SocketConnect中实现initStream
                }

                // ========== 步骤2：打开登录窗体，校验登录状态 ==========
                string userId = string.Empty;
                using (var login = new Login(socketConnect))
                {
                    var loginResult = login.ShowDialog();

                    // 登录取消/失败
                    if (loginResult != DialogResult.OK)
                    {
                        socketConnect?.Dispose(); // 释放连接
                        return;
                    }

                    // 获取用户ID并校验
                    userId = login.getUserID();
                    if (string.IsNullOrEmpty(userId))
                    {
                        socketConnect?.Dispose(); // 释放连接
                        MessageBox.Show("用户ID为空，程序将退出", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // ========== 步骤3：启动主窗体 ==========
                Application.Run(new Main(socketConnect, userId));
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"网络连接异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                socketConnect?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"程序启动失败：{ex.Message}\n{ex.StackTrace}", "致命错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                socketConnect?.Dispose();
            }
        }
    }
}