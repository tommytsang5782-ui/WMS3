using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Class;
using WMSClient.Utils;

namespace WMSClient
{
    public partial class Welcome : Form
    {
        // 核心连接对象
        private readonly SocketConnect _socketConnect;
        // 取消令牌源（终止异步连接）
        private CancellationTokenSource _cts;
        // 已连接的裸Socket（适配Program.cs）
        private Socket _rawSocket;

        /// <summary>
        /// 构造函数：初始化窗体和连接对象
        /// </summary>
        public Welcome()
        {
            InitializeComponent();
            _socketConnect = new SocketConnect();
            _cts = new CancellationTokenSource();

            // 注册窗体关闭事件
            FormClosing += Welcome_FormClosing;
        }

        /// <summary>
        /// 窗体显示时启动异步连接（非阻塞UI）
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // 启动异步连接（不阻塞UI）
            _ = ConnectLoopAsync(_cts.Token);
        }

        /// <summary>
        /// 窗体关闭：清理所有资源
        /// </summary>
        private void Welcome_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 取消异步任务
            _cts?.Cancel();
            _cts?.Dispose();

            // 释放Socket连接
            //_socketConnect?.Dispose();
            //_rawSocket?.Dispose();
        }

        /// <summary>
        /// 异步连接循环（核心逻辑）
        /// </summary>
        private async Task ConnectLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    bool isConnected = false;
                    try
                    {
                        // 调用SocketConnect的连接方法（需确保该方法已实现）
                        isConnected = await Task.Run(() => _socketConnect.ConnectToServer(), cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // 任务被取消，退出循环
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"连接失败：{ex.Message}");
                        isConnected = false;
                    }

                    // 连接成功：保存裸Socket，关闭欢迎窗体
                    if (isConnected)
                    {
                        _rawSocket = _socketConnect.GetSocket(); // 获取裸Socket
                        UpdateUi(() =>
                        {
                            DialogResult = DialogResult.OK; // 标记连接成功
                            Close();
                        });
                        break;
                    }

                    // 连接失败：显示配置窗体（空实现，避免报错）
                    var configureResult = ShowConfigureDialog();
                    if (configureResult == DialogResult.Cancel)
                    {
                        // 用户取消配置，退出程序
                        UpdateUi(() =>
                        {
                            DialogResult = DialogResult.Cancel;
                            Close();
                        });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"连接循环异常：{ex.Message}");
                UpdateUi(() =>
                {
                    CommonUtils.ShowMessage($"连接服务器异常：{ex.Message}", "错误", MessageBoxIcon.Error);
                    DialogResult = DialogResult.Abort;
                    Close();
                });
            }
        }

        /// <summary>
        /// 显示配置窗体（空实现，避免报错）
        /// </summary>
        private DialogResult ShowConfigureDialog()
        {
            return UpdateUi(() =>
            {
                var configure = new Configure();
                return configure.ShowDialog();
                //// 弹出确认框，替代未实现的Configure窗体
                //var result = MessageBox.Show("连接服务器失败，是否重试？", "提示",
                //    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //return result == DialogResult.Yes ? DialogResult.OK : DialogResult.Cancel;
            });
        }

        /// <summary>
        /// 安全更新UI（跨线程适配）
        /// </summary>
        private T UpdateUi<T>(Func<T> action)
        {
            if (InvokeRequired)
            {
                return (T)Invoke(action);
            }
            return action();
        }

        /// <summary>
        /// 无返回值UI更新
        /// </summary>
        private void UpdateUi(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// 适配Program.cs：获取已连接的裸Socket（小写g，匹配调用）
        /// </summary>
        public Socket getSocketClient()
        {
            return _rawSocket;
        }

        /// <summary>
        /// 备用：获取SocketConnect对象（可选）
        /// </summary>
        public SocketConnect GetSocketConnect()
        {
            return _socketConnect;
        }

    }

    //// ========== 补充空实现：避免Configure窗体报错 ==========
    //public partial class Configure : Form
    //{
    //    public Configure()
    //    {
    //        InitializeComponent();
    //    }

    //     空设计器实现（避免编译错误）
    //    private void InitializeComponent()
    //    {
    //        this.Text = "服务器配置";
    //        this.Size = new Size(300, 200);
    //        var btnOk = new Button { Text = "确定", DialogResult = DialogResult.OK, Location = new Point(80, 100) };
    //        var btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Point(180, 100) };
    //        this.Controls.Add(btnOk);
    //        this.Controls.Add(btnCancel);
    //        this.AcceptButton = btnOk;
    //        this.CancelButton = btnCancel;
    //    }
    //}
}