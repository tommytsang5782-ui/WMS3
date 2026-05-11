using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMS.Database_Dao;

namespace WMS
{
    internal class Form1Backup2
    {
    }
}
//using ExtensionMethods;
//using Newtonsoft.Json;
//using System;
////  AI  ++++++++++++++
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.SqlTypes;
//using System.Diagnostics;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Runtime.InteropServices;
//using System.Runtime.Serialization.Json;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Forms;
//using System.Windows.Input;
//using WMS.Database_Dao;
//using WMS.Models;
//using WMS.Page;
//using WMS.ReceiveMessage;
////  AI  ++++++++++++++
//namespace WMS
//{
//    public partial class Form1 : Form
//    {
//        // ========== 第一步：DAO 单例封装（核心优化） ==========
//        public static class DaoManager
//        {
//            // 懒加载单例，仅创建一次，全局复用
//            private static readonly Lazy<Dao> _dao = new Lazy<Dao>(() => new Dao());
//            private static readonly Lazy<Dao_ClosedPrescan> _daoClosedPrescan = new Lazy<Dao_ClosedPrescan>(() => new Dao_ClosedPrescan());
//            private static readonly Lazy<Dao_ClosedPrescanInnerCarton> _daoClosedPrescanInnerCarton = new Lazy<Dao_ClosedPrescanInnerCarton>(() => new Dao_ClosedPrescanInnerCarton());
//            private static readonly Lazy<Dao_ClosedPrescanOuterCarton> _daoClosedPrescanOuterCarton = new Lazy<Dao_ClosedPrescanOuterCarton>(() => new Dao_ClosedPrescanOuterCarton());
//            private static readonly Lazy<Dao_Connection> _daoConnection = new Lazy<Dao_Connection>(() => new Dao_Connection());
//            private static readonly Lazy<Dao_CustomerGroup> _daoCustomerGroup = new Lazy<Dao_CustomerGroup>(() => new Dao_CustomerGroup());
//            private static readonly Lazy<Dao_InnerCarton> _daoInnerCarton = new Lazy<Dao_InnerCarton>(() => new Dao_InnerCarton());
//            private static readonly Lazy<Dao_Item> _daoItem = new Lazy<Dao_Item>(() => new Dao_Item());
//            private static readonly Lazy<Dao_LabelHeader> _daoLabelHeader = new Lazy<Dao_LabelHeader>(() => new Dao_LabelHeader());
//            private static readonly Lazy<Dao_LabelLine> _daoLabelLine = new Lazy<Dao_LabelLine>(() => new Dao_LabelLine());
//            private static readonly Lazy<Dao_Mapping> _daoMapping = new Lazy<Dao_Mapping>(() => new Dao_Mapping());
//            private static readonly Lazy<Dao_OuterCarton> _daoOuterCarton = new Lazy<Dao_OuterCarton>(() => new Dao_OuterCarton());
//            private static readonly Lazy<Dao_PackingHeader> _daoPackingHeader = new Lazy<Dao_PackingHeader>(() => new Dao_PackingHeader());
//            private static readonly Lazy<Dao_PackingLine> _daoPackingLine = new Lazy<Dao_PackingLine>(() => new Dao_PackingLine());
//            private static readonly Lazy<Dao_PackingMapping> _daoPackingMapping = new Lazy<Dao_PackingMapping>(() => new Dao_PackingMapping());
//            private static readonly Lazy<Dao_Prescan> _daoPrescan = new Lazy<Dao_Prescan>(() => new Dao_Prescan());
//            private static readonly Lazy<Dao_PrescanInnerCarton> _daoPrescanInnerCarton = new Lazy<Dao_PrescanInnerCarton>(() => new Dao_PrescanInnerCarton());
//            private static readonly Lazy<Dao_PrescanOuterCarton> _daoPrescanOuterCarton = new Lazy<Dao_PrescanOuterCarton>(() => new Dao_PrescanOuterCarton());
//            private static readonly Lazy<Dao_Printer> _daoPrinter = new Lazy<Dao_Printer>(() => new Dao_Printer());
//            private static readonly Lazy<Dao_ScanLabelString> _daoScanLabelString = new Lazy<Dao_ScanLabelString>(() => new Dao_ScanLabelString());
//            private static readonly Lazy<Dao_ScannedPackingHeader> _daoScannedPackingHeader = new Lazy<Dao_ScannedPackingHeader>(() => new Dao_ScannedPackingHeader());
//            private static readonly Lazy<Dao_ScannedPackingLine> _daoScannedPackingLine = new Lazy<Dao_ScannedPackingLine>(() => new Dao_ScannedPackingLine());
//            private static readonly Lazy<Dao_ScannedPackingMapping> _daoScannedPackingMapping = new Lazy<Dao_ScannedPackingMapping>(() => new Dao_ScannedPackingMapping());
//            private static readonly Lazy<Dao_Synchronize> _daoSynchronize = new Lazy<Dao_Synchronize>(() => new Dao_Synchronize());
//            private static readonly Lazy<Dao_Company> _daoCompany = new Lazy<Dao_Company>(() => new Dao_Company());
//            private static readonly Lazy<Dao_User> _daoUser = new Lazy<Dao_User>(() => new Dao_User());
//            private static readonly Lazy<Dao_ODataSetup> _daoODataSetup = new Lazy<Dao_ODataSetup>(() => new Dao_ODataSetup());
//            private static readonly Lazy<Dao_Setup> _daoSetup = new Lazy<Dao_Setup>(() => new Dao_Setup());

//            // 对外提供单例访问
//            public static Dao Dao => _dao.Value;
//            public static Dao_ClosedPrescan DaoClosedPrescan => _daoClosedPrescan.Value;
//            public static Dao_ClosedPrescanInnerCarton DaoClosedPrescanInnerCarton => _daoClosedPrescanInnerCarton.Value;
//            public static Dao_ClosedPrescanOuterCarton DaoClosedPrescanOuterCarton => _daoClosedPrescanOuterCarton.Value;
//            public static Dao_Connection DaoConnection => _daoConnection.Value;
//            public static Dao_CustomerGroup DaoCustomerGroup => _daoCustomerGroup.Value;
//            public static Dao_InnerCarton DaoInnerCarton => _daoInnerCarton.Value;
//            public static Dao_Item DaoItem => _daoItem.Value;
//            public static Dao_LabelHeader DaoLabelHeader => _daoLabelHeader.Value;
//            public static Dao_LabelLine DaoLabelLine => _daoLabelLine.Value;
//            public static Dao_Mapping DaoMapping => _daoMapping.Value;
//            public static Dao_PackingHeader DaoPackingHeader => _daoPackingHeader.Value;
//            public static Dao_PackingLine DaoPackingLine => _daoPackingLine.Value;
//            public static Dao_PackingMapping DaoPackingMapping => _daoPackingMapping.Value;
//            public static Dao_Prescan DaoPrescan => _daoPrescan.Value;
//            public static Dao_PrescanInnerCarton DaoPrescanInnerCarton => _daoPrescanInnerCarton.Value;
//            public static Dao_PrescanOuterCarton DaoPrescanOuterCarton => _daoPrescanOuterCarton.Value;
//            public static Dao_Printer DaoPrinter => _daoPrinter.Value;
//            public static Dao_ScanLabelString DaoScanLabelString => _daoScanLabelString.Value;
//            public static Dao_ScannedPackingHeader DaoScannedPackingHeader => _daoScannedPackingHeader.Value;
//            public static Dao_ScannedPackingLine DaoScannedPackingLine => _daoScannedPackingLine.Value;
//            public static Dao_ScannedPackingMapping DaoScannedPackingMapping => _daoScannedPackingMapping.Value;
//            public static Dao_Synchronize DaoSynchronize => _daoSynchronize.Value;
//            public static Dao_Company DaoCompany => _daoCompany.Value;
//            public static Dao_User DaoUser => _daoUser.Value;
//            public static Dao_ODataSetup DaoODataSetup => _daoODataSetup.Value;
//            public static Dao_Setup DaoSetup => _daoSetup.Value;
//        }


//        // ========== 第二步：全局常量（复用序列化配置） ==========
//        public static class GlobalConfig
//        {
//            // 复用 JSON 序列化配置，避免重复创建
//            public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
//            {
//                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
//                Formatting = Formatting.None // 紧凑格式，减少传输体积
//            };

//            // 批量发送阈值（减少 Sleep 次数）
//            public const int BatchSendThreshold = 50;
//            // 批量发送延迟（毫秒）
//            public const int BatchSendDelay = 100;
//        }

//        Socket ServerSocket = null;
//        List<Socket> SocketList = null;
//        //  AI  ++++++++++++++
//        // 替換原本的 Dictionary 欄位，改為執行緒安全集合
//        ConcurrentDictionary<string, Socket> dic = new ConcurrentDictionary<string, Socket>();
//        ConcurrentDictionary<string, string> dic2 = new ConcurrentDictionary<string, string>();
//        // 取代直接 Abort 的控制用 CancellationTokenSource
//        CancellationTokenSource cts;
//        Thread thread;
//        //  AI  ++++++++++++++

//        Boolean booStart = false;
//        static String DataSource;
//        static String InitialCatalog;
//        static String UserID;
//        static String Password;
//        public static NAV.NAV nav;
//        static Boolean timerStarted;
//        TimersTimer timera = new TimersTimer();
//        Boolean isConnected = false;
//        public Form1()
//        {
//            InitializeComponent();
//            Patrolcar patrolcar = new Patrolcar();
//            patrolcar.dgd_ini.getIni();
//            StartStopBtn.Text = "Start";
//            //IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
//            //IPAddress ipAddress = host.AddressList[0];
//            //foreach (var ip in host.AddressList)
//            //{
//            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
//            //    {
//            //        Console.WriteLine("IP Address = " + ip.ToString());
//            //    }
//            //}
//            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
//            {
//                Console.WriteLine("No Network Available");
//            }

//            IPHostEntry host2 = Dns.GetHostEntry(Dns.GetHostName());

//            var ippaddress = host2
//                .AddressList
//                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
//            //Console.WriteLine(ippaddress);


//            IPtextBox.Text = ippaddress.ToString();
//        }



//        private void Startbtn_Click(object sender, EventArgs e)
//        {
//            if (!booStart)
//            {
//                try
//                {
//                    IPEndPoint IPE = new IPEndPoint(IPAddress.Parse(IPtextBox.Text), Int32.Parse(ProttextBox.Text));
//                    ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                    ServerSocket.Bind(IPE);
//                    ServerSocket.Listen(10);
//                    StartStopBtn.Text = "Stop";
//                    booStart = true;
//                    DataSource = Properties.Settings.Default.DataSource;
//                    InitialCatalog = Properties.Settings.Default.InitialCatalog;
//                    UserID = Properties.Settings.Default.DBUserID;
//                    Password = Properties.Settings.Default.DBPassword;
//                    ShowMsg("服務器已啓動，監聽中...");
//                    Log log = new Log();
//                    List<String> valueTitle = null;
//                    List<String> value = null;
//                    log.EventLog("Server Start", Environment.MachineName, valueTitle, value);
//                    log = null;

//                    // 使用 CancellationTokenSource 控制接收執行緒的結束
//                    cts = new CancellationTokenSource();
//                    thread = new Thread(() => ListenClientConnect(cts.Token));
//                    thread.IsBackground = true;
//                    thread.Start();

//                    if (!timerStarted)
//                    {
//                        timera.InitTimer();
//                        timera.timer_Start();
//                        timerStarted = true;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    ShowMsg("啓動服務器失敗: " + ex.Message);
//                    booStart = false;
//                }
//            }
//            else
//            {
//                // 優雅停止：避免 Thread.Abort()
//                try
//                {
//                    booStart = false;
//                    // 啟動取消訊號，並關閉 ServerSocket 以解除 Accept 的阻塞
//                    cts?.Cancel();
//                    try { ServerSocket?.Close(); } catch { }

//                    // 等待執行緒結束（短暫等待）
//                    if (thread != null && thread.IsAlive)
//                    {
//                        thread.Join(1000);
//                    }

//                    timera.timer_Stop();
//                    StartStopBtn.Text = "Start";

//                    DataSource = "";
//                    InitialCatalog = "";
//                    UserID = "";
//                    Password = "";
//                    ShowMsg("服務器已中斷");
//                    Log log = new Log();
//                    List<String> valueTitle = null;
//                    List<String> value = null;
//                    log.EventLog("Server Stop", Environment.MachineName, valueTitle, value);
//                    log = null;

//                    // 關閉並清除所有已知連線
//                    foreach (var kv in dic)
//                    {
//                        try
//                        {
//                            kv.Value.Shutdown(SocketShutdown.Both);
//                            kv.Value.Close();
//                        }
//                        catch { }
//                    }
//                    dic.Clear();
//                    dic2.Clear();
//                }
//                catch (Exception ex)
//                {
//                    ShowMsg("停止服務器時發生錯誤: " + ex.Message);
//                }
//            }
//        }
//        // 將 ListenClientConnect 改為可接收 CancellationToken
//        private void ListenClientConnect(object tokenObj)
//        {
//            var token = tokenObj as CancellationToken?;
//            try
//            {
//                while (booStart && (token == null || !token.Value.IsCancellationRequested))
//                {
//                    Socket socketClient = null;
//                    try
//                    {
//                        // Accept 會在 ServerSocket.Close() 時拋出例外，從而跳出迴圈
//                        socketClient = ServerSocket.Accept();
//                    }
//                    catch (SocketException)
//                    {
//                        // 可能是 Close() 引起的，若要求取消就跳出
//                        if (token != null && token.Value.IsCancellationRequested) break;
//                        else throw;
//                    }

//                    if (socketClient == null) break;

//                    try
//                    {
//                        string RemoteIP = socketClient.RemoteEndPoint.ToString();
//                        dic.TryAdd(RemoteIP, socketClient);
//                        dic2.TryAdd(RemoteIP, "");
//                        Invoke(new MethodInvoker(() => IPlistBox.Items.Add(RemoteIP)));
//                        ShowMsg(RemoteIP + "已連接");

//                        Thread.Sleep(200);
//                        Thread recieveThread = new Thread(ReceiveMsg);
//                        recieveThread.IsBackground = true;
//                        recieveThread.Start(socketClient);
//                    }
//                    catch (Exception exInner)
//                    {
//                        ShowMsg("接受新連線時發生錯誤: " + exInner.Message);
//                        try { socketClient?.Close(); } catch { }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                // 不要吃掉例外，記錄即可
//                ShowMsg("ListenClientConnect 發生例外: " + ex.Message);
//            }
//        }
//        private static String DELIMITER = "_$";


//        public void ReceiveMsg(object soc)
//        {
//            ThreadPool.QueueUserWorkItem(state =>
//            {
//                Socket socketClient = state as Socket;
//                if (socketClient == null) return;

//                string remoteEndPoint = socketClient.RemoteEndPoint.ToString();
//                MessageText msgTxt = new MessageText();
//                // 关键1：设置超时，避免无限阻塞
//                socketClient.ReceiveTimeout = 5000; // 5秒超时
//                socketClient.SendTimeout = 5000;

//                try
//                {
//                    using (NetworkStream ns = new NetworkStream(socketClient))
//                    using (StreamReader sr = new StreamReader(ns, Encoding.UTF8))
//                    using (StreamWriter sw = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true })
//                    {
//                        // 关键2：用更合理的休眠时间（无数据时休眠100ms，而非10ms）
//                        int idleSleepMs = 100;
//                        // 关键3：增加退出标记的线程安全检查
//                        while (Volatile.Read(ref isConnected) && Volatile.Read(ref booStart))
//                        {
//                            string msg = null;
//                            try
//                            {
//                                // 关键4：直接用 ReadLineAsync + 超时，替代 Peek() 轮询
//                                var readTask = sr.ReadLineAsync();
//                                // 等待读取完成，或等待idleSleepMs（无数据时快速退出）
//                                if (readTask.Wait(idleSleepMs))
//                                {
//                                    msg = readTask.Result;
//                                }
//                                else
//                                {
//                                    continue; // 无数据，继续循环
//                                }

//                                if (string.IsNullOrEmpty(msg))
//                                {
//                                    // 空消息 = 客户端断开连接，退出循环
//                                    isConnected = false;
//                                    continue;
//                                }
//                                ShowMsg($"[{msg}]");

//                                // 解析和处理逻辑（不变）
//                                CommuForm commuForm = JsonConvert.DeserializeObject<CommuForm>(msg);
//                                ShowMsg(commuForm.Action);
//                                ShowMsg(commuForm.Command);
//                                HandleCommand(commuForm, socketClient, sw, remoteEndPoint);
//                            }
//                            catch (TimeoutException)
//                            {
//                                // 读取超时，继续循环（正常逻辑，非异常）
//                                continue;
//                            }
//                            catch (IOException)
//                            {
//                                isConnected = false;
//                                continue;
//                            }
//                            catch (JsonException jex)
//                            {
//                                ShowMsg($"解析收到的訊息失敗: {jex.Message}");
//                                continue;
//                            }
//                        }
//                    }
//                }
//                catch (SocketException _e)
//                {
//                    ShowMsg($"Disconnected: error code {_e.NativeErrorCode}! ");
//                    ShowMsg($"收到來自 --> {remoteEndPoint} 的指令，但執行失敗\r\n");
//                    RemoveListBox(remoteEndPoint);
//                }
//                catch (Exception _e1)
//                {
//                    ShowMsg($"Disconnected: error {_e1.Message}! ");
//                    RemoveListBox(remoteEndPoint);
//                }
//                finally
//                {
//                    // 优雅释放 Socket
//                    try
//                    {
//                        if (socketClient.Connected)
//                        {
//                            socketClient.Shutdown(SocketShutdown.Both);
//                        }
//                        RemoveListBox(remoteEndPoint);

//                    }
//                    catch { }
//                    try { socketClient.Close(); } catch { }

//                    // 无需手动置空单例 DAO（单例全局复用）
//                    msgTxt = null;
//                }
//            }, soc);
//        }

//        //public void recievemsg(object soc)
//        //{
//        //    Socket socketClient = soc as Socket;
//        //    if (socketClient == null) return;

//        //    CommuForm commuForm = new CommuForm("", "", "", "");
//        //    NetworkStream ns = null;
//        //    StreamReader sr = null;
//        //    StreamWriter sw = null;

//        //    // DAO 建立保留（仍建議改為共用/DI，但先保留原邏輯）
//        //    Dao dao = new Dao();
//        //    Dao_ClosedPrescan daoClosedPrescan = new Dao_ClosedPrescan();
//        //    Dao_ClosedPrescanInnerCarton daoClosedPrescanInnerCarton = new Dao_ClosedPrescanInnerCarton();
//        //    Dao_ClosedPrescanOuterCarton daoClosedPrescanOuterCarton = new Dao_ClosedPrescanOuterCarton();
//        //    Dao_Connection daoConnection = new Dao_Connection();
//        //    Dao_CustomerGroup daoCustomerGroup = new Dao_CustomerGroup();
//        //    Dao_InnerCarton daoInnerCarton = new Dao_InnerCarton();
//        //    Dao_Item daoItem = new Dao_Item();
//        //    Dao_LabelHeader daoLabelHeader = new Dao_LabelHeader();
//        //    Dao_LabelLine daoLabelLine = new Dao_LabelLine();
//        //    Dao_Mapping daoMapping = new Dao_Mapping();
//        //    Dao_OuterCarton daoOuterCarton = new Dao_OuterCarton();
//        //    Dao_PackingHeader daoPackingHeader = new Dao_PackingHeader();
//        //    Dao_PackingLine daoPackingLine = new Dao_PackingLine();
//        //    Dao_PackingMapping daoPackingMapping = new Dao_PackingMapping();
//        //    Dao_Prescan daoPrescan = new Dao_Prescan();
//        //    Dao_PrescanInnerCarton daoPrescanInnerCarton = new Dao_PrescanInnerCarton();
//        //    Dao_PrescanOuterCarton daoPrescanOuterCarton = new Dao_PrescanOuterCarton();
//        //    Dao_Printer daoPrinter = new Dao_Printer();
//        //    Dao_ScanLabelString daoScanLabelString = new Dao_ScanLabelString();
//        //    Dao_ScannedPackingHeader daoScannedPackingHeader = new Dao_ScannedPackingHeader();
//        //    Dao_ScannedPackingLine daoScannedPackingLine = new Dao_ScannedPackingLine();
//        //    Dao_ScannedPackingMapping daoScannedPackingMapping = new Dao_ScannedPackingMapping();
//        //    Dao_Synchronize daoSynchronize = new Dao_Synchronize();
//        //    Dao_Company daoCompany = new Dao_Company();
//        //    Dao_User daoUser = new Dao_User();
//        //    Dao_ODataSetup daoODataSetup = new Dao_ODataSetup();
//        //    Dao_Setup daoSetup = new Dao_Setup();
//        //    MessageText MsgTxt = new MessageText();

//        //    DateTime dateTime = DateTime.Now;
//        //    DataTable dt;
//        //    string json = "";
//        //    string msg;
//        //    int i;
//        //    String MACAddress = "";

//        //    try
//        //    {
//        //        ns = new NetworkStream(socketClient);
//        //        sr = new StreamReader(ns, Encoding.UTF8);
//        //        sw = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };

//        //        while (true)
//        //        {
//        //            if (!booStart)
//        //            {
//        //                // 服務已停止，關閉連線
//        //                try { socketClient.Shutdown(SocketShutdown.Both); } catch { }
//        //                try { socketClient.Close(); } catch { }
//        //                break;
//        //            }

//        //            msg = null;
//        //            try
//        //            {
//        //                msg = sr.ReadLine();

//        //                if (string.IsNullOrEmpty(msg))
//        //                {
//        //                    continue;
//        //                }
//        //                    ShowMsg("["+msg+"]");
//        //            }
//        //            catch (IOException)
//        //            {
//        //                // 連線中斷
//        //                msg = null;
//        //            }

//        //            if (string.IsNullOrEmpty(msg))
//        //            {
//        //                continue;

//        //                // client 已關閉或讀取到 EOF，離開迴圈
//        //                ShowMsg("來自 " + socketClient.RemoteEndPoint.ToString() + " 的連線已中斷或收到空訊息。");
//        //                RemoveListBox(socketClient.RemoteEndPoint.ToString());
//        //                try { socketClient.Shutdown(SocketShutdown.Both); } catch { }
//        //                try { socketClient.Close(); } catch { }
//        //                break;
//        //            }
//        //            else
//        //            {
//        //                ShowMsg(msg);
//        //            }

//        //            // 解析 JSON 並處理命令 (原有大段 switch 保留，僾在讀取前加入防範)
//        //            try
//        //            {
//        //                commuForm = JsonConvert.DeserializeObject<CommuForm>(msg);
//        //                ShowMsg(commuForm.Action);
//        //                ShowMsg(commuForm.Command);
//        //            }
//        //            catch (JsonException jex)
//        //            {
//        //                ShowMsg("解析收到的訊息失敗: " + jex.Message);
//        //                continue; // 跳過不合法訊息
//        //            }

//        //                    switch (commuForm.Command)
//        //                    {
//        //                        case "Open":
//        //                            switch (commuForm.Action)
//        //                            {
//        //                                case "List":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "UserList":
//        //                                            UserList userList = new UserList();
//        //                                            userList.OpenUserList(socketClient, this);
//        //                                            break;
//        //                                        case "UserCard":
//        //                                            User_Control userCardControl = new User_Control();
//        //                                            userCardControl.OpenUserCard();
//        //                                            break;
//        //                                    }
//        //                                    break;
//        //                            }
//        //                            break;
//        //                        case "SQL":
//        //                            switch (commuForm.Action)
//        //                            {
//        //                                case "Insert":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "User":
//        //                                            User newuser = new User();
//        //                                            break;
//        //                                        case "Prescan":
//        //                                            Prescan prescan = JsonConvert.DeserializeObject<Prescan>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedInsertperscanRows = dao.Insertperscan(prescan);
//        //                                            Console.WriteLine("Effected Rows : " + effectedInsertperscanRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "Prescan");
//        //                                            break;
//        //                                        case "ScanLabelString":
//        //                                            ScanLabelString scanLabelString = JsonConvert.DeserializeObject<ScanLabelString>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedScanLabelStringRows = dao.InsertScanLabelString(scanLabelString);
//        //                                            Console.WriteLine(effectedScanLabelStringRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(scanLabelString, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ScanLabelString");
//        //                                            break;
//        //                                        case "OuterCarton":
//        //                                            OuterCarton outerCarton = JsonConvert.DeserializeObject<OuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedOuterCartonRows = dao.InsertOuterCarton(outerCarton);
//        //                                            Console.WriteLine(effectedOuterCartonRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(outerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "OuterCarton");
//        //                                            break;
//        //                                        case "OuterCartonWithInnerCarton":
//        //                                            InnerCarton OuterWithinnerCarton = JsonConvert.DeserializeObject<InnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedOuterWithinnerCartonRows = dao.InsertInnerCarton(OuterWithinnerCarton);
//        //                                            Console.WriteLine(effectedOuterWithinnerCartonRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(OuterWithinnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "OuterCartonWithInnerCarton");
//        //                                            break;
//        //                                        case "InnerCarton":
//        //                                            InnerCarton innerCarton = JsonConvert.DeserializeObject<InnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedInnerCartonRows = dao.InsertInnerCarton(innerCarton);
//        //                                            Console.WriteLine(effectedInnerCartonRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(innerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "InnerCarton");
//        //                                            break;
//        //                                        case "PrescanOuterCarton":
//        //                                            PrescanOuterCarton prescanOuterCarton = JsonConvert.DeserializeObject<PrescanOuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedPrescanOuterCartonRows = dao.InsertPrescanOuterCarton(prescanOuterCarton);
//        //                                            Console.WriteLine(effectedPrescanOuterCartonRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanOuterCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "PrescanOuterCarton");
//        //                                            break;
//        //                                        case "PrescanInnerCarton":
//        //                                            PrescanInnerCarton prescanInnerCarton = JsonConvert.DeserializeObject<PrescanInnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedPrescanInnerCartonRows = dao.InsertPrescanInnerCarton(prescanInnerCarton);
//        //                                            Console.WriteLine(effectedPrescanInnerCartonRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "PrescanInnerCarton");
//        //                                            break;
//        //                                        default:
//        //                                            break;
//        //                                    }
//        //                                    break;
//        //                                case "Delete":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "PrescanOuterCarton":
//        //                                            PrescanOuterCarton prescanOuterCarton = JsonConvert.DeserializeObject<PrescanOuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedPrescanOuterCartonRows = dao.deletePrescanOuterCarton(prescanOuterCarton.DocumentNo, prescanOuterCarton.LineNo);
//        //                                            Console.WriteLine(effectedPrescanOuterCartonRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanOuterCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Delete", "PrescanOuterCarton");
//        //                                            break;
//        //                                        case "PrescanInnerCarton":
//        //                                            PrescanInnerCarton prescanInnerCarton = JsonConvert.DeserializeObject<PrescanInnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedPrescanInnerCartonRows = dao.deletePrescanInnerCarton(prescanInnerCarton.DocumentNo, prescanInnerCarton.OuterCartonLineNo, prescanInnerCarton.LineNo);
//        //                                            Console.WriteLine(effectedPrescanInnerCartonRows);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Delete", "PrescanInnerCarton");
//        //                                            break;
//        //                                        default:
//        //                                            break;
//        //                                    }
//        //                                    break;
//        //                                case "Reset":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "Prescan":
//        //                                            Prescan prescan = JsonConvert.DeserializeObject<Prescan>(commuForm.Str.Remove(0, 1));
//        //                                            String DocNo = prescan.DocumentNo;
//        //                                            int effectedPrescanOuterCartonRows = dao.Prescan_Reset(DocNo);
//        //                                            Console.WriteLine(effectedPrescanOuterCartonRows);
//        //                                            break;
//        //                                    }
//        //                                    break;
//        //                                case "Select":
//        //                                    MACAddress = commuForm.Str;
//        //                                    Synchronize synchronize = new Synchronize();
//        //                                    synchronize.MACAddress = MACAddress;
//        //                                    //daoSynchronize.UpdateSynchronize(synchronize);
//        //                                    //int dataQty = 0;
//        //                                    string jsonQty = "";
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "User":
//        //                                            ShowMsg("UserUserUserUserUserUserUserUserUserUserUser 　： User List");
//        //                                            User blankUser = new User();
//        //                                            List<User> userlist = daoUser.SelectUser(blankUser);
//        //                                            //synchronize.user = daoUser.gettimestamp();
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + userlist.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                    var response = ResponseForm<object>.Success("User", jsonQty);
//        //                                    sw.WriteLine(response);// + DELIMITER);
//        //                                            ShowMsg("UserUserUserUserUserUserUserUserUserUserUser 　： " + jsonQty);
//        //                                            ShowMsg("UserUserUserUserUserUserUserUserUserUserUser 　： " + userlist.Count);
//        //                                            i = 0;
//        //                                            foreach (User user in userlist)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "User", "@" + JsonConvert.SerializeObject(user, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                        var response2 = ResponseForm<object>.Success("User", json2);
//        //                                        ShowMsg("UserUser   table 　： " + response2);
//        //                                        ShowMsg("UserUser   table 　： " + response2.table);
//        //                                        ShowMsg("UserUser   Data  　： " + response2.Data);

//        //                                        sw.WriteLine(response2 );
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(200);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： User List");
//        //                                            break;
//        //                                        case "PackingHeader":
//        //                                            ShowMsg("PackingHeaderPackingHeaderPackingHeader: All 　： Packing List");
//        //                                            PackingHeader blankPackingHeader = new PackingHeader();
//        //                                            ShowMsg("PackingHeaderPackingHeaderPackingHeader: All 　： 1");
//        //                                            List<PackingHeader> PackingHeaderlist = daoPackingHeader.SelectPackingHeader(blankPackingHeader);
//        //                                            ShowMsg("PackingHeaderPackingHeaderPackingHeader: All 　： 2");
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + PackingHeaderlist.Count);
//        //                                            ShowMsg("PackingHeaderPackingHeaderPackingHeader: All 　： 3");
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            ShowMsg("PackingHeaderPackingHeaderPackingHeader: All 　： 4");
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            ShowMsg("PackingHeaderPackingHeaderPackingHeader: All 　： Packing List" + jsonQty);
//        //                                            i = 0;
//        //                                            foreach (PackingHeader packingheader in PackingHeaderlist)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Packing Header", "@" + JsonConvert.SerializeObject(packingheader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                List<PackingLine> PackingLinelist2 = daoPackingLine.PackingLinebyDocNo(packingheader.No);
//        //                                                foreach (PackingLine packingLine in PackingLinelist2)
//        //                                                {
//        //                                                    commuForm = new CommuForm("Reply", "Insert", "Packing Line", "@" + JsonConvert.SerializeObject(packingLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                    string json3 = JsonConvert.SerializeObject(commuForm);
//        //                                                    sw.WriteLine(json3 + DELIMITER);
//        //                                                }
//        //                                                i = i + 1;
//        //                                                if (i >= 5)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Packing List");
//        //                                            break;
//        //                                        case "Mapping":
//        //                                            Mapping blankMapping = new Mapping();
//        //                                            List<Mapping> Mappinglist = daoMapping.SelectMapping(blankMapping);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + Mappinglist.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (Mapping mapping in Mappinglist)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Mapping", "@" + JsonConvert.SerializeObject(mapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Mapping List finish");

//        //                                            break;
//        //                                        case "ScanLabelString":
//        //                                            //Scan Label String
//        //                                            ScanLabelString blankScanLabelString = new ScanLabelString();
//        //                                            List<ScanLabelString> scanLabelStringList = daoScanLabelString.SelectScanLabelString(blankScanLabelString);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + scanLabelStringList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (ScanLabelString scanLabelString in scanLabelStringList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Scan Label String", "@" + JsonConvert.SerializeObject(scanLabelString, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Scan Label String List finish");

//        //                                            break;
//        //                                        case "Prescan":
//        //                                            //Prescan
//        //                                            Prescan blankPrescan = new Prescan();
//        //                                            List<Prescan> PrescanList = daoPrescan.SelectPrescan(blankPrescan);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + PrescanList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (Prescan prescan in PrescanList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Prescan", "@" + JsonConvert.SerializeObject(prescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Prescan List finish");

//        //                                            break;
//        //                                        case "OuterCarton":
//        //                                            //Outer Carton
//        //                                            OuterCarton blankOuterCarton = new OuterCarton();
//        //                                            List<OuterCarton> OuterCartonList = daoOuterCarton.SelectOuterCarton(blankOuterCarton);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + OuterCartonList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (OuterCarton outerCarton in OuterCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Outer Carton", "@" + JsonConvert.SerializeObject(outerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Outer Carton List finish");

//        //                                            break;
//        //                                        case "InnerCarton":
//        //                                            //Inner Carton
//        //                                            InnerCarton blankInnerCarton = new InnerCarton();
//        //                                            List<InnerCarton> innerCartonList = daoInnerCarton.SelectInnerCarton(blankInnerCarton);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + innerCartonList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (InnerCarton innerCarton in innerCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Inner Carton", "@" + JsonConvert.SerializeObject(innerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");

//        //                                            break;
//        //                                        case "LabelHeader":
//        //                                            //Label Header
//        //                                            LabelHeader blankLabelHeader = new LabelHeader();
//        //                                            List<LabelHeader> LabelHeaderList = daoLabelHeader.SelectLabelHeader(blankLabelHeader);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + LabelHeaderList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (LabelHeader labelHeader in LabelHeaderList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Label Header", "@" + JsonConvert.SerializeObject(labelHeader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Label Header List finish");
//        //                                            break;
//        //                                        case "LabelLine":
//        //                                            //Label Line
//        //                                            LabelLine blankLabelLine = new LabelLine();
//        //                                            List<LabelLine> labelLineList = daoLabelLine.SelectLabelLine(blankLabelLine);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + labelLineList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (LabelLine labelLine in labelLineList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Label Line", "@" + JsonConvert.SerializeObject(labelLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Label Line finish");
//        //                                            break;
//        //                                        case "PrescanOuterCarton":
//        //                                            //Prescan Outer Carton
//        //                                            PrescanOuterCarton blankPrescanOuterCarton = new PrescanOuterCarton();
//        //                                            List<PrescanOuterCarton> PrescanOuterCartonList = daoPrescanOuterCarton.SelectPrescanOuterCarton(blankPrescanOuterCarton);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + PrescanOuterCartonList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (PrescanOuterCarton prescanOuterCarton in PrescanOuterCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Prescan Outer Carton", "@" + JsonConvert.SerializeObject(prescanOuterCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Prescan Outer Carton List finish");
//        //                                            break;
//        //                                        case "PrescanInnerCarton":
//        //                                            //Prescan Inner Carton
//        //                                            PrescanInnerCarton blankPrescanInnerCarton = new PrescanInnerCarton();
//        //                                            List<PrescanInnerCarton> prescanInnerCartonList = daoPrescanInnerCarton.SelectPrescanInnerCarton(blankPrescanInnerCarton);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + prescanInnerCartonList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (PrescanInnerCarton prescanInnerCarton in prescanInnerCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Prescan Inner Carton", "@" + JsonConvert.SerializeObject(prescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Prescan Inner Carton List finish");
//        //                                            break;
//        //                                        case "PackingMapping":
//        //                                            //Packing Mapping 
//        //                                            PackingMapping blankPackingMapping = new PackingMapping();
//        //                                            List<PackingMapping> packingMappingList = daoPackingMapping.SelectPackingMapping(blankPackingMapping);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + packingMappingList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (PackingMapping packingMapping in packingMappingList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Packing Mapping", "@" + JsonConvert.SerializeObject(packingMapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Packing Mapping List finish");

//        //                                            break;
//        //                                        case "ClosedPrescan":
//        //                                            //Closed Prescan
//        //                                            ClosedPrescan blankClosedPrescan = new ClosedPrescan();
//        //                                            List<ClosedPrescan> ClosedPrescanList = daoClosedPrescan.SelectClosedPrescan(blankClosedPrescan);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + ClosedPrescanList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (ClosedPrescan closedprescan in ClosedPrescanList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Closed Prescan", "@" + JsonConvert.SerializeObject(closedprescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Closed Prescan List finish");

//        //                                            break;
//        //                                        case "ClosedPrescanOuterCarton":
//        //                                            //Closed Prescan Outer Carton
//        //                                            ClosedPrescanOuterCarton blankClosedPrescanOuterCarton = new ClosedPrescanOuterCarton();
//        //                                            List<ClosedPrescanOuterCarton> ClosedPrescanOuterCartonList = daoClosedPrescanOuterCarton.SelectClosedPrescanOuterCarton(blankClosedPrescanOuterCarton);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + ClosedPrescanOuterCartonList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (ClosedPrescanOuterCarton outerCarton in ClosedPrescanOuterCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Closed Prescan Outer Carton", "@" + JsonConvert.SerializeObject(outerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Closed Prescan Outer Carton List finish");


//        //                                            break;
//        //                                        case "ClosedPrescanInnerCarton":
//        //                                            //Closed Prescan Inner Carton
//        //                                            ClosedPrescanInnerCarton blankClosedPrescanInnerCarton = new ClosedPrescanInnerCarton();
//        //                                            List<ClosedPrescanInnerCarton> ClosedPrescanInnerCartonList = daoClosedPrescanInnerCarton.SelectClosedPrescanInnerCarton(blankClosedPrescanInnerCarton);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + ClosedPrescanInnerCartonList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (ClosedPrescanInnerCarton closedPrescanInnerCarton in ClosedPrescanInnerCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Closed Prescan Inner Carton", "@" + JsonConvert.SerializeObject(closedPrescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Closed Prescan Inner Carton List finish");

//        //                                            break;
//        //                                    Boolean initContinue = false;
//        //                                        case "Item":
//        //                                            //Item 
//        //                                            Item blankItem = new Item();
//        //                                            List<Item> itmeList = daoItem.SelectItem(blankItem);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + itmeList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (Item item in itmeList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Item", "@" + JsonConvert.SerializeObject(item, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Item  List finish");

//        //                                            break;
//        //                                        case "CustomerGroup":
//        //                                            //Customer Group 
//        //                                            CustomerGroup blankCustomerGroup = new CustomerGroup();
//        //                                            List<CustomerGroup> custGrpList = daoCustomerGroup.SelectCustomerGroup(blankCustomerGroup);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + custGrpList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (CustomerGroup custGrp in custGrpList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Customer Group", "@" + JsonConvert.SerializeObject(custGrp, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Customer Group finish");

//        //                                            break;
//        //                                        case "Printer":
//        //                                            //Printer 
//        //                                            Printer blankPrinter = new Printer();
//        //                                            List<Printer> printerList = daoPrinter.SelectPrinter(blankPrinter);
//        //                                            commuForm = new CommuForm("Reply", "Count", "Count", "@" + printerList.Count);
//        //                                            jsonQty = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonQty + DELIMITER);
//        //                                            i = 0;
//        //                                            foreach (Printer printer in printerList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Printer", "@" + JsonConvert.SerializeObject(printer, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                Console.WriteLine(json2);

//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                //ShowMsg(json2 + "");//Show send data
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(200);
//        //                                                }
//        //                                            }
//        //                                            Thread.Sleep(500);
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Printer  List finish");
//        //                                            break;

//        //                                        case "All":
//        //                                            String richTextBoxTxt = ""; //richTextBox1.Text;
//        //                                            ShowMsg(richTextBoxTxt + "/r/n" + MsgTxt.initDataMsg(socketClient.RemoteEndPoint.ToString(), "", 1));
//        //                                            MACAddress = commuForm.Str;
//        //                                            DateTime dateTimeAll = new DateTime();
//        //                                            dateTimeAll = DateTime.Now;
//        //                                            User blankUserAll = new User();
//        //                                            List<User> userlistAll = daoUser.SelectUser(blankUserAll);
//        //                                            Synchronize synchronizeAll = new Synchronize();
//        //                                            synchronize.MACAddress = MACAddress;
//        //                                            synchronize.user = daoUser.gettimestamp();
//        //                                            //daoSynchronize.UpdateSynchronize(synchronize);
//        //                                            commuForm = new CommuForm("Reply", "Insert", "User", "@" + JsonConvert.SerializeObject(userlistAll, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                            string jsonG = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(jsonG + DELIMITER);

//        //                                            Thread.Sleep(200);
//        //                                            //sw.WriteLine(json);
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： User List finish");
//        //                                            updatemsg(richTextBoxTxt + "/r/n" + MsgTxt.initDataMsg(socketClient.RemoteEndPoint.ToString(), "User List", 2));


//        //                                            /////////////////////////////////commuForm = new CommuForm("Reply", "Initial", "All", "@OK");
//        //                                            /////////////////////////////////string json5 = JsonConvert.SerializeObject(commuForm);
//        //                                            /////////////////////////////////sw.WriteLine(json5 + DELIMITER);

//        //                                            dao.UpdateSynchronize(MACAddress, dateTime);
//        //                                            break;

//        //                                        case "success":
//        //                                            ShowMsg("finish");
//        //                                            break;
//        //                                        case "All_Update_PL":
//        //                                            initContinue = true;
//        //                                            break;
//        //                                        case "All_Update_User":
//        //                                            initContinue = true;
//        //                                            break;
//        //                                        case "All_Update":

//        //                                            initContinue = false;
//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All Update List");
//        //                                            MACAddress = commuForm.Str;
//        //                                            DateTime NewUpdate_dateTime = new DateTime();
//        //                                            NewUpdate_dateTime = DateTime.Now;
//        //                                            synchronize = daoSynchronize.SelectSynchronize(commuForm.Str);
//        //                                            List<User> sUserList = daoUser.SelectUser_timestamp(synchronize.user.notNulltimestamp());
//        //                                            User user_Synchronize = new User();
//        //                                            i = 0;

//        //                                            commuForm = new CommuForm("Reply", "Insert", "User", "@" + JsonConvert.SerializeObject(sUserList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                            string json23 = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(json23 + DELIMITER);

//        //                                            initContinue = false;
//        //                                            List<PackingHeader> sPackingHeaderList = daoPackingHeader.SelectPackingHeader_timestamp(synchronize.packingHeader.notNulltimestamp());
//        //                                            i = 0;

//        //                                            foreach (PackingHeader packingHeader in sPackingHeaderList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Packing Header", "@" + JsonConvert.SerializeObject(packingHeader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Packing Header List finish");


//        //                                            List<PackingLine> sPackingLineList = daoPackingLine.SelectPackingLine_timestamp(synchronize.packingLine.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (PackingLine packingLine in sPackingLineList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Packing Line", "@" + JsonConvert.SerializeObject(packingLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Packing Line List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            List<Mapping> sMappingList = daoMapping.SelectMapping_timestamp(synchronize.mapping.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (Mapping mapping in sMappingList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Mapping", "@" + JsonConvert.SerializeObject(mapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Mapping List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Scan Label String
//        //                                            List<ScanLabelString> sScanLabelStringList = daoScanLabelString.SelectScanLabelString_timestamp(synchronize.scanLabelString.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (ScanLabelString scanLabelString in sScanLabelStringList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Scan Label String", "@" + JsonConvert.SerializeObject(scanLabelString, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Scan Label String List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Prescan
//        //                                            List<Prescan> sPrescanList = daoPrescan.SelectPrescan_timestamp(synchronize.prescan.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (Prescan prescan in sPrescanList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Prescan", "@" + JsonConvert.SerializeObject(prescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Prescan List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Outer Carton
//        //                                            List<OuterCarton> sOuterCartonList = daoOuterCarton.SelectOuterCarton_timestamp(synchronize.outerCarton.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (OuterCarton outerCarton in sOuterCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Outer Carton", "@" + JsonConvert.SerializeObject(outerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Outer Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Inner Carton
//        //                                            List<InnerCarton> sInnerCartonList = daoInnerCarton.SelectInnerCarton_timestamp(synchronize.innerCarton.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (InnerCarton innerCarton in sInnerCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Inner Carton", "@" + JsonConvert.SerializeObject(innerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Label Header
//        //                                            List<LabelHeader> sLabelHeaderList = daoLabelHeader.SelectLabelHeader_timestamp(synchronize.labelHeader.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (LabelHeader labelHeader in sLabelHeaderList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Label Header", "@" + JsonConvert.SerializeObject(labelHeader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Label Header List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Label Line
//        //                                            List<LabelLine> sLabelLineList = daoLabelLine.SelectLabelLine_timestamp(synchronize.labelLine.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (LabelLine labelLine in sLabelLineList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Label Line", "@" + JsonConvert.SerializeObject(labelLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Label Line List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Prescan Outer Carton
//        //                                            List<PrescanOuterCarton> sPrescanOuterCartonList = daoPrescanOuterCarton.SelectPrescanOuterCarton_timestamp(synchronize.prescanOuterCarton.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (PrescanOuterCarton prescanOuterCarton in sPrescanOuterCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Prescan Outer Carton", "@" + JsonConvert.SerializeObject(prescanOuterCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Prescan Outer Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Prescan Inner Carton
//        //                                            List<PrescanInnerCarton> sPrescanInnerCartonList = daoPrescanInnerCarton.SelectPrescanInnerCarton_timestamp(synchronize.prescanInnerCarton.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (PrescanInnerCarton prescanInnerCarton in sPrescanInnerCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Prescan Inner Carton", "@" + JsonConvert.SerializeObject(prescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Packing Mapping
//        //                                            List<PackingMapping> sPackingMappingList = daoPackingMapping.SelectPackingMapping_timestamp(synchronize.packingMapping.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (PackingMapping packingMapping in sPackingMappingList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Packing Mapping", "@" + JsonConvert.SerializeObject(packingMapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Item
//        //                                            List<Item> sItemList = daoItem.SelectItem_timestamp(synchronize.item.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (Item item in sItemList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Item", "@" + JsonConvert.SerializeObject(item, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Customer Group
//        //                                            List<CustomerGroup> sCustomerGroupList = daoCustomerGroup.SelectCustomerGroup_timestamp(synchronize.customerGroup.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (CustomerGroup customerGroup in sCustomerGroupList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Customer Group", "@" + JsonConvert.SerializeObject(customerGroup, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;

//        //                                    initContinue = false;
//        //                                            //Printer
//        //                                            List<Printer> sPrinterList = daoPrinter.SelectPrinter_timestamp(synchronize.printer.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (Printer printer in sPrinterList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Printer", "@" + JsonConvert.SerializeObject(printer, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Scanned Packing Header
//        //                                            List<ScannedPackingHeader> sScannedPackingHeaderList = daoScannedPackingHeader.SelectScannedPackingHeader_timestamp(synchronize.scannedPackingHeader.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (ScannedPackingHeader scannedPackingHeader in sScannedPackingHeaderList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Scanned Packing Header", "@" + JsonConvert.SerializeObject(scannedPackingHeader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Scanned Packing Line
//        //                                            List<ScannedPackingLine> sScannedPackingLineList = daoScannedPackingLine.SelectScannedPackingLine_timestamp(synchronize.scannedPackingLine.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (ScannedPackingLine scannedPackingLine in sScannedPackingLineList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Scanned Packing Line", "@" + JsonConvert.SerializeObject(scannedPackingLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Scanned Packing Mapping
//        //                                            List<ScannedPackingMapping> sScannedPackingMappingList = daoScannedPackingMapping.SelectScannedPackingMapping_timestamp(synchronize.scannedPackingMapping.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (ScannedPackingMapping scannedPackingMapping in sScannedPackingMappingList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Scanned Packing Mapping", "@" + JsonConvert.SerializeObject(scannedPackingMapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Closed Prescan
//        //                                            List<ClosedPrescan> sClosedPrescanList = daoClosedPrescan.SelectClosedPrescan_timestamp(synchronize.closedPrescan.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (ClosedPrescan closedPrescan in sClosedPrescanList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Closed Prescan", "@" + JsonConvert.SerializeObject(closedPrescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Closed Prescan Outer Carton
//        //                                            List<ClosedPrescanOuterCarton> sClosedPrescanOuterCartonList = daoClosedPrescanOuterCarton.SelectClosedPrescanOuterCarton_timestamp(synchronize.closedPrescanOuterCarton.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (ClosedPrescanOuterCarton closedPrescanOuterCarton in sClosedPrescanOuterCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Closed Prescan Outer Carton", "@" + JsonConvert.SerializeObject(closedPrescanOuterCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            initContinue = false;
//        //                                            //Closed Prescan Inner Carton
//        //                                            List<ClosedPrescanInnerCarton> sClosedPrescanInnerCartonList = daoClosedPrescanInnerCarton.SelectClosedPrescanInnerCarton_timestamp(synchronize.closedPrescanInnerCarton.notNulltimestamp());
//        //                                            i = 0;
//        //                                            foreach (ClosedPrescanInnerCarton closedPrescanInnerCarton in sClosedPrescanInnerCartonList)
//        //                                            {
//        //                                                commuForm = new CommuForm("Reply", "Insert", "Closed Prescan Inner Carton", "@" + JsonConvert.SerializeObject(closedPrescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
//        //                                                string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                                sw.WriteLine(json2 + DELIMITER);
//        //                                                i = i + 1;
//        //                                                if (i >= 30)
//        //                                                {
//        //                                                    i = 0;
//        //                                                    Thread.Sleep(500);
//        //                                                }
//        //                                            }
//        //                                            //ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");
//        //                                            while (!initContinue)
//        //                                            {

//        //                                            }

//        //                                            commuForm.Command = "Reply";
//        //                                            commuForm.Action = "UpdateFinish";
//        //                                            commuForm.Table = "UpdateFinish";
//        //                                            commuForm.Str = "UpdateFinish";
//        //                                            ShowMsg(commuForm.Str + "");
//        //                                            json = JsonConvert.SerializeObject(commuForm);
//        //                                            sw.WriteLine(json);

//        //                                            ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": Synchronize  finish");
//        //                                            dao.UpdateSynchronize(MACAddress, NewUpdate_dateTime);
//        //                                            break;
//        //                                    }
//        //                                    ShowMsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": ALL :  finish");
//        //                                    break;
//        //                                case "Update":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "Prescan":
//        //                                            Prescan prescan = JsonConvert.DeserializeObject<Prescan>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedInsertperscanRows = dao.Updateperscan(prescan);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Prescan");
//        //                                            break;
//        //                                        default:
//        //                                            break;
//        //                                    }
//        //                                    break;
//        //                                case "Finish":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "Standrad Processing":
//        //                                            PackingMapping packingMapping = JsonConvert.DeserializeObject<PackingMapping>(commuForm.Str.Remove(0, 1));
//        //                                            int effectedInsertperscanRows = dao.FinishStandradProcessing(packingMapping.PackingNo, packingMapping.PrescanNo);
//        //                                            break;
//        //                                        default:
//        //                                            break;
//        //                                    }
//        //                                    break;

//        //                                case "Connect":
//        //                                    try
//        //                                    {
//        //                                        String value;
//        //                                        if (dic2.TryGetValue(socketClient.RemoteEndPoint.ToString(), out value))
//        //                                        {
//        //                                            dic2[socketClient.RemoteEndPoint.ToString()] = "Android";
//        //                                        }
//        //                                        else
//        //                                        {
//        //                                    dic2[socketClient.RemoteEndPoint.ToString()] = "Android";
//        //                                    //dic2.Add(socketClient.RemoteEndPoint.ToString(), "Android");
//        //                                }
//        //                                        commuForm = new CommuForm("Reply", "Connect", "Connect", "@OK");
//        //                                        string json2 = JsonConvert.SerializeObject(commuForm);
//        //                                //        sw.WriteLine(json2 + DELIMITER);
//        //                                var response = ResponseForm<object>.Success("Connect", json2);
//        //                                sw.WriteLine(response);
//        //                                ShowMsg(json2 + "");
//        //                                    }
//        //                                    catch (KeyNotFoundException)
//        //                                    {
//        //                                        Console.WriteLine("Key = " + socketClient.RemoteEndPoint.ToString() + " is not found.");
//        //                                    }
//        //                                    break;

//        //                                default:
//        //                                    commuForm.Command = "";
//        //                                    commuForm.Action = "";
//        //                                    commuForm.Table = "";
//        //                                    commuForm.Str = "";
//        //                                    ShowMsg(commuForm.Str + "");
//        //                                    json = JsonConvert.SerializeObject(commuForm);
//        //                                    sw.WriteLine(json);
//        //                                    break;
//        //                            }

//        //                            break;
//        //                        case "New":
//        //                            switch (commuForm.Action)
//        //                            {
//        //                                case "Device":
//        //                                    dao.InitSynchronize(commuForm.Str, DateTime.MinValue);
//        //                                    break;
//        //                            }
//        //                            break;
//        //                        case "SQL_W":
//        //                            int effectedRows = 0;
//        //                            switch (commuForm.Action)
//        //                            {
//        //                                case "Hi":
//        //                                    Byte[] byteNumy = new Byte[2];
//        //                                    byteNumy = System.Text.Encoding.BigEndianUnicode.GetBytes("Hi");
//        //                                    socketClient.Send(byteNumy, byteNumy.Length, SocketFlags.None);
//        //                                    break;
//        //                                case "PCConnect":
//        //                                    try
//        //                                    {
//        //                                        Console.WriteLine("For key = " + socketClient.RemoteEndPoint.ToString() + " , value = {0}.",
//        //                                            dic[socketClient.RemoteEndPoint.ToString()]);
//        //                                        String value;
//        //                                        if (dic2.TryGetValue(socketClient.RemoteEndPoint.ToString(), out value))
//        //                                        {
//        //                                            dic2[socketClient.RemoteEndPoint.ToString()] = "PC";
//        //                                        }
//        //                                    }
//        //                                    catch (KeyNotFoundException)
//        //                                    {
//        //                                        Console.WriteLine("Key = " + socketClient.RemoteEndPoint.ToString() + " is not found.");
//        //                                    }
//        //                                    Byte[] byteNumP = new Byte[1];
//        //                                    byteNumP = System.Text.Encoding.BigEndianUnicode.GetBytes("1");
//        //                                    socketClient.Send(byteNumP, byteNumP.Length, SocketFlags.None);
//        //                                    break;
//        //                                case "Select":
//        //                                    Byte[] byteNumA = new Byte[20];
//        //                                    Byte[] byteNumB = new Byte[1024];
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "User":
//        //                                            User user = JsonConvert.DeserializeObject<User>(commuForm.Str.Remove(0, 1));
//        //                                            List<User> userList = daoUser.SelectUser(user);
//        //                                            json = JsonConvert.SerializeObject(userList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            ShowMsg(json);
//        //                                            break;
//        //                                        case "Item":
//        //                                            Item item = JsonConvert.DeserializeObject<Item>(commuForm.Str.Remove(0, 1));
//        //                                            List<Item> itemList = daoItem.SelectItem(item);
//        //                                            json = JsonConvert.SerializeObject(itemList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "CustomerGroup":
//        //                                            CustomerGroup customerGroup = JsonConvert.DeserializeObject<CustomerGroup>(commuForm.Str.Remove(0, 1));
//        //                                            List<CustomerGroup> customerGroupList = daoCustomerGroup.SelectCustomerGroup(customerGroup);
//        //                                            json = JsonConvert.SerializeObject(customerGroupList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "Printer":
//        //                                            Printer printer = JsonConvert.DeserializeObject<Printer>(commuForm.Str.Remove(0, 1));
//        //                                            List<Printer> printerList = daoPrinter.SelectPrinter(printer);
//        //                                            json = JsonConvert.SerializeObject(printerList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "PackingHeader":
//        //                                            PackingHeader packingHeader = JsonConvert.DeserializeObject<PackingHeader>(commuForm.Str.Remove(0, 1));
//        //                                            List<PackingHeader> packingHeaderList = daoPackingHeader.SelectPackingHeader(packingHeader);
//        //                                            json = JsonConvert.SerializeObject(packingHeaderList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "PackingLine":
//        //                                            PackingLine packingLine = JsonConvert.DeserializeObject<PackingLine>(commuForm.Str.Remove(0, 1));
//        //                                            List<PackingLine> packingLineList = daoPackingLine.SelectPackingLine(packingLine);
//        //                                            json = JsonConvert.SerializeObject(packingLineList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "Mapping":
//        //                                            Mapping mapping = JsonConvert.DeserializeObject<Mapping>(commuForm.Str.Remove(0, 1));
//        //                                            List<Mapping> mappingLine = daoMapping.SelectMapping(mapping);
//        //                                            json = JsonConvert.SerializeObject(mappingLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "LabelHeader":
//        //                                            LabelHeader labelHeader = JsonConvert.DeserializeObject<LabelHeader>(commuForm.Str.Remove(0, 1));
//        //                                            List<LabelHeader> labelHeaderList = daoLabelHeader.SelectLabelHeader(labelHeader);
//        //                                            json = JsonConvert.SerializeObject(labelHeaderList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "LabelLine":
//        //                                            LabelLine labelLine = JsonConvert.DeserializeObject<LabelLine>(commuForm.Str.Remove(0, 1));
//        //                                            List<LabelLine> labelLineList = daoLabelLine.SelectLabelLine(labelLine);
//        //                                            json = JsonConvert.SerializeObject(labelLineList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "ScanLabelString":
//        //                                            ScanLabelString scanLabelString = JsonConvert.DeserializeObject<ScanLabelString>(commuForm.Str.Remove(0, 1));
//        //                                            List<ScanLabelString> scanLabelStringList = daoScanLabelString.SelectScanLabelString(scanLabelString);
//        //                                            json = JsonConvert.SerializeObject(scanLabelStringList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "Prescan":
//        //                                            Prescan prescan = JsonConvert.DeserializeObject<Prescan>(commuForm.Str.Remove(0, 1));
//        //                                            List<Prescan> prescanList = daoPrescan.SelectPrescan(prescan);
//        //                                            json = JsonConvert.SerializeObject(prescanList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "OuterCarton":
//        //                                            OuterCarton outerCarton = JsonConvert.DeserializeObject<OuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            List<OuterCarton> outerCartonList = daoOuterCarton.SelectOuterCarton(outerCarton);
//        //                                            json = JsonConvert.SerializeObject(outerCartonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "InnerCarton":
//        //                                            InnerCarton innerCarton = JsonConvert.DeserializeObject<InnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            List<InnerCarton> innerCartonList = daoInnerCarton.SelectInnerCarton(innerCarton);
//        //                                            json = JsonConvert.SerializeObject(innerCartonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "PrescanOuterCarton":
//        //                                            PrescanOuterCarton prescanOuterCarton = JsonConvert.DeserializeObject<PrescanOuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            List<PrescanOuterCarton> prescanOuterCartonList = daoPrescanOuterCarton.SelectPrescanOuterCarton(prescanOuterCarton);
//        //                                            json = JsonConvert.SerializeObject(prescanOuterCartonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "PrescanInnerCarton":
//        //                                            PrescanInnerCarton prescaninnerCarton = JsonConvert.DeserializeObject<PrescanInnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            List<PrescanInnerCarton> prescaninnerCartonList = daoPrescanInnerCarton.SelectPrescanInnerCarton(prescaninnerCarton);
//        //                                            json = JsonConvert.SerializeObject(prescaninnerCartonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "PackingMapping":
//        //                                            PackingMapping packingMapping = JsonConvert.DeserializeObject<PackingMapping>(commuForm.Str.Remove(0, 1));
//        //                                            List<PackingMapping> packingMappingList = daoPackingMapping.SelectPackingMapping(packingMapping);
//        //                                            json = JsonConvert.SerializeObject(packingMappingList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "ScannedPackingHeader":
//        //                                            ScannedPackingHeader scannedPackingHeader = JsonConvert.DeserializeObject<ScannedPackingHeader>(commuForm.Str.Remove(0, 1));
//        //                                            List<ScannedPackingHeader> scannedPackingHeaderList = daoScannedPackingHeader.SelectScannedPackingHeader(scannedPackingHeader);
//        //                                            json = JsonConvert.SerializeObject(scannedPackingHeaderList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "ScannedPackingLine":
//        //                                            ScannedPackingLine scannedPackingLine = JsonConvert.DeserializeObject<ScannedPackingLine>(commuForm.Str.Remove(0, 1));
//        //                                            List<ScannedPackingLine> scannedPackingLineList = daoScannedPackingLine.SelectScannedPackingLine(scannedPackingLine);
//        //                                            json = JsonConvert.SerializeObject(scannedPackingLineList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "ScannedPackingMapping":
//        //                                            ScannedPackingMapping scannedPackingMapping = JsonConvert.DeserializeObject<ScannedPackingMapping>(commuForm.Str.Remove(0, 1));
//        //                                            List<ScannedPackingMapping> scannedPackingMappingList = daoScannedPackingMapping.SelectScannedPackingMapping(scannedPackingMapping);
//        //                                            json = JsonConvert.SerializeObject(scannedPackingMappingList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "ClosedPrescan":
//        //                                            ClosedPrescan closedPrescan = JsonConvert.DeserializeObject<ClosedPrescan>(commuForm.Str.Remove(0, 1));
//        //                                            List<ClosedPrescan> closedPrescanList = daoClosedPrescan.SelectClosedPrescan(closedPrescan);
//        //                                            json = JsonConvert.SerializeObject(closedPrescanList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "ClosedPrescanOuterCarton":
//        //                                            ClosedPrescanOuterCarton closedPrescanOuterCarton = JsonConvert.DeserializeObject<ClosedPrescanOuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            List<ClosedPrescanOuterCarton> closedPrescanOuterCartonList = daoClosedPrescanOuterCarton.SelectClosedPrescanOuterCarton(closedPrescanOuterCarton);
//        //                                            json = JsonConvert.SerializeObject(closedPrescanOuterCartonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "ClosedPrescanInnerCarton":
//        //                                            ClosedPrescanInnerCarton closedPrescanInnerCarton = JsonConvert.DeserializeObject<ClosedPrescanInnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            List<ClosedPrescanInnerCarton> closedPrescanInnerCartonList = daoClosedPrescanInnerCarton.SelectClosedPrescanInnerCarton(closedPrescanInnerCarton);
//        //                                            json = JsonConvert.SerializeObject(closedPrescanInnerCartonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "Company":
//        //                                            Company company = JsonConvert.DeserializeObject<Company>(commuForm.Str.Remove(0, 1));
//        //                                            List<Company> companies = daoCompany.SelectCompany(company);
//        //                                            json = JsonConvert.SerializeObject(companies, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "ODataSetup":
//        //                                            ODataSetup oDataSetup = JsonConvert.DeserializeObject<ODataSetup>(commuForm.Str.Remove(0, 1));
//        //                                            List<ODataSetup> oDataSetups = daoODataSetup.SelectODataSetup();
//        //                                            json = JsonConvert.SerializeObject(oDataSetups, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                        case "Setup":
//        //                                            Setup setup = JsonConvert.DeserializeObject<Setup>(commuForm.Str.Remove(0, 1));
//        //                                            List<Setup> setups = daoSetup.Select(setup);
//        //                                            json = JsonConvert.SerializeObject(setups, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            break;
//        //                                    }
//        //                                    byteNumB = new Byte[json.Length * 2];
//        //                                    byteNumB = System.Text.Encoding.BigEndianUnicode.GetBytes(json);
//        //                                    byteNumA = System.Text.Encoding.BigEndianUnicode.GetBytes(byteNumB.Length.ToString());
//        //                                    socketClient.Send(byteNumA, byteNumA.Length, SocketFlags.None);
//        //                                    socketClient.Send(byteNumB, byteNumB.Length, SocketFlags.None);
//        //                                    break;
//        //                                case "Update":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "User":
//        //                                            List<User> users = JsonConvert.DeserializeObject<List<User>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoUser.UpdateUser(users[0].UserID, users[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(users[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "User");
//        //                                            break;
//        //                                        case "Item":
//        //                                            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoItem.UpdateItem(items[0].No, items[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(items[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Item");
//        //                                            break;
//        //                                        case "CustomerGroup":
//        //                                            List<CustomerGroup> customerGroups = JsonConvert.DeserializeObject<List<CustomerGroup>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoCustomerGroup.UpdateCustomerGroup(customerGroups[0].Code, customerGroups[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(customerGroups[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "CustomerGroup");
//        //                                            break;
//        //                                        case "Printer":
//        //                                            List<Printer> printers = JsonConvert.DeserializeObject<List<Printer>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrinter.UpdatePrinter(printers[0].Code, printers[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(printers[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Printer");
//        //                                            break;
//        //                                        case "PackingHeader":
//        //                                            break;
//        //                                        case "PackingLine":
//        //                                            break;
//        //                                        case "Mapping":
//        //                                            List<Mapping> mappings = JsonConvert.DeserializeObject<List<Mapping>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoMapping.UpdateMapping(mappings[0].No, mappings[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(mappings[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Mapping");
//        //                                            break;
//        //                                        case "LabelHeader":
//        //                                            List<LabelHeader> labelHeaders = JsonConvert.DeserializeObject<List<LabelHeader>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoLabelHeader.UpdateLabelHeader(labelHeaders[0].Code, labelHeaders[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(labelHeaders[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "LabelHeader");
//        //                                            break;
//        //                                        case "LabelLine":
//        //                                            List<LabelLine> labelLines = JsonConvert.DeserializeObject<List<LabelLine>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoLabelLine.UpdateLabelLine(labelLines[0].Code, labelLines[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(labelLines[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "LabelLine");
//        //                                            break;
//        //                                        case "ScanLabelString":
//        //                                            List<ScanLabelString> scanLabelStrings = JsonConvert.DeserializeObject<List<ScanLabelString>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoScanLabelString.UpdateScanLabelString(scanLabelStrings[0].EntryNo, scanLabelStrings[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(scanLabelStrings[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "ScanLabelString");
//        //                                            break;
//        //                                        case "OuterCarton":
//        //                                            List<OuterCarton> outerCartons = JsonConvert.DeserializeObject<List<OuterCarton>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoOuterCarton.UpdateOuterCarton(outerCartons[0], outerCartons[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(outerCartons[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "OuterCarton");
//        //                                            break;
//        //                                        case "InnerCarton":
//        //                                            List<InnerCarton> innerCartons = JsonConvert.DeserializeObject<List<InnerCarton>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoInnerCarton.UpdateInnerCarton(innerCartons[0], innerCartons[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(innerCartons[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "InnerCarton");
//        //                                            break;
//        //                                        case "Prescan":
//        //                                            List<Prescan> prescans = JsonConvert.DeserializeObject<List<Prescan>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescan.UpdatePrescan(prescans[0], prescans[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescans[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Prescan");
//        //                                            break;
//        //                                        case "PrescanOuterCarton":
//        //                                            List<PrescanOuterCarton> prescanOuterCartons = JsonConvert.DeserializeObject<List<PrescanOuterCarton>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescanOuterCarton.UpdatePrescanOuterCarton(prescanOuterCartons[0], prescanOuterCartons[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanOuterCartons[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "PrescanOuterCarton");
//        //                                            break;
//        //                                        case "PrescanInnerCarton":
//        //                                            List<PrescanInnerCarton> prescaninnerCartons = JsonConvert.DeserializeObject<List<PrescanInnerCarton>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescanInnerCarton.UpdatePrescanInnerCarton(prescaninnerCartons[0], prescaninnerCartons[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescaninnerCartons[1], new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "PrescanInnerCarton");
//        //                                            break;
//        //                                        case "PackingMapping":
//        //                                            List<PackingMapping> packingMappings = JsonConvert.DeserializeObject<List<PackingMapping>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPackingMapping.UpdatePackingMapping(packingMappings[0], packingMappings[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(packingMappings, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "PackingMapping");
//        //                                            break;
//        //                                        case "ScannedPackingHeader":
//        //                                            break;
//        //                                        case "ScannedPackingLine":
//        //                                            break;
//        //                                        case "ScannedPackingMapping":
//        //                                            break;
//        //                                        case "ClosedPrescan":
//        //                                            break;
//        //                                        case "ClosedPrescanOuterCarton":
//        //                                            break;
//        //                                        case "ClosedPrescanInnerCarton":
//        //                                            break;
//        //                                        case "Company":
//        //                                            List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoCompany.UpdateCompany(companies[0].Name, companies[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(companies, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Company");
//        //                                            break;
//        //                                        case "ODataSetup":
//        //                                            List<ODataSetup> oDataSetups = JsonConvert.DeserializeObject<List<ODataSetup>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoODataSetup.UpdateODataSetups(oDataSetups[0].PrimaryKey, oDataSetups[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(oDataSetups, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "ODataSetup");
//        //                                            break;
//        //                                        case "Setup":
//        //                                            List<Setup> setups = JsonConvert.DeserializeObject<List<Setup>>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoSetup.Update(setups[0], setups[1]);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(setups, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Setup");
//        //                                            break;
//        //                                    }
//        //                                    byteNumB = new Byte[effectedRows * 2];
//        //                                    byteNumB = System.Text.Encoding.BigEndianUnicode.GetBytes(effectedRows.ToString());
//        //                                    socketClient.Send(byteNumB, byteNumB.Length, SocketFlags.None);
//        //                                    break;
//        //                                case "Insert":
//        //                                    Byte[] insertReturn = new Byte[4];
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "User":
//        //                                            User user = JsonConvert.DeserializeObject<User>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoUser.Insert(user);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(user, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "User");
//        //                                            break;
//        //                                        case "CustomerGroup":
//        //                                            CustomerGroup customerGroup = JsonConvert.DeserializeObject<CustomerGroup>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoCustomerGroup.InsertCustomerGroup(customerGroup);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(customerGroup, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "CustomerGroup");
//        //                                            break;
//        //                                        case "Printer":
//        //                                            Printer printer = JsonConvert.DeserializeObject<Printer>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrinter.InsertPrinter(printer);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(printer, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "Printer");
//        //                                            break;
//        //                                        case "PackingHeader":
//        //                                            break;
//        //                                        case "PackingLine":
//        //                                            break;
//        //                                        case "Item":
//        //                                            Item item = JsonConvert.DeserializeObject<Item>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoItem.InsertItem(item);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(item, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Item");
//        //                                            break;
//        //                                        case "Mapping":
//        //                                            Mapping mapping = JsonConvert.DeserializeObject<Mapping>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoMapping.InsertMapping(mapping);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(mapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "Mapping");
//        //                                            break;
//        //                                        case "LabelHeader":
//        //                                            LabelHeader labelHeader = JsonConvert.DeserializeObject<LabelHeader>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoLabelHeader.InsertLabelHeader(labelHeader);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(labelHeader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "LabelHeader");
//        //                                            break;
//        //                                        case "LabelLine":
//        //                                            LabelLine labelLine = JsonConvert.DeserializeObject<LabelLine>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoLabelLine.InsertLabelLine(labelLine);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(labelLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "LabelLine");
//        //                                            break;
//        //                                        case "ScanLabelString":
//        //                                            ScanLabelString scanLabelString = JsonConvert.DeserializeObject<ScanLabelString>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoScanLabelString.Insert(scanLabelString);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(scanLabelString, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ScanLabelString");

//        //                                            break;
//        //                                        case "OuterCarton":
//        //                                            OuterCarton outerCarton = JsonConvert.DeserializeObject<OuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoOuterCarton.Insert(outerCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(outerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "OuterCarton");

//        //                                            break;
//        //                                        case "InnerCarton":
//        //                                            InnerCarton innerCarton = JsonConvert.DeserializeObject<InnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoInnerCarton.Insert(innerCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(innerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "InnerCarton");

//        //                                            break;
//        //                                        case "Prescan":
//        //                                            Prescan prescan = JsonConvert.DeserializeObject<Prescan>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescan.InsertPrescan(prescan);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "Prescan");
//        //                                            break;
//        //                                        case "PrescanOuterCarton":
//        //                                            PrescanOuterCarton prescanOuterCarton = JsonConvert.DeserializeObject<PrescanOuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescanOuterCarton.InsertPrescanOuterCarton(prescanOuterCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanOuterCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "PrescanOuterCarton");
//        //                                            break;
//        //                                        case "PrescanInnerCarton":
//        //                                            PrescanInnerCarton prescanInnerCarton = JsonConvert.DeserializeObject<PrescanInnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescanInnerCarton.Insert(prescanInnerCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "PrescanInnerCarton");

//        //                                            break;
//        //                                        case "PackingMapping":
//        //                                            PackingMapping packingMapping = JsonConvert.DeserializeObject<PackingMapping>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPackingMapping.Insert(packingMapping);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(packingMapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "PackingMapping");

//        //                                            break;
//        //                                        case "ScannedPackingHeader":
//        //                                            ScannedPackingHeader scannedPackingHeader = JsonConvert.DeserializeObject<ScannedPackingHeader>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoScannedPackingHeader.Insert(scannedPackingHeader);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(scannedPackingHeader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ScannedPackingHeader");

//        //                                            break;
//        //                                        case "ScannedPackingLine":
//        //                                            ScannedPackingLine scannedPackingLine = JsonConvert.DeserializeObject<ScannedPackingLine>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoScannedPackingLine.Insert(scannedPackingLine);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(scannedPackingLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ScannedPackingLine");

//        //                                            break;
//        //                                        case "ScannedPackingMapping":
//        //                                            ScannedPackingMapping scannedPackingMapping = JsonConvert.DeserializeObject<ScannedPackingMapping>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoScannedPackingMapping.Insert(scannedPackingMapping);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(scannedPackingMapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ScannedPackingMapping");

//        //                                            break;
//        //                                        case "ClosedPrescan":
//        //                                            ClosedPrescan closedPrescan = JsonConvert.DeserializeObject<ClosedPrescan>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoClosedPrescan.Insert(closedPrescan);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(closedPrescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ClosedPrescan");
//        //                                            break;
//        //                                        case "ClosedPrescanOuterCarton":
//        //                                            ClosedPrescanOuterCarton closedPrescanOuterCarton = JsonConvert.DeserializeObject<ClosedPrescanOuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoClosedPrescanOuterCarton.Insert(closedPrescanOuterCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(closedPrescanOuterCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ClosedPrescanOuterCarton");
//        //                                            break;
//        //                                        case "ClosedPrescanInnerCarton":
//        //                                            ClosedPrescanInnerCarton closedPrescanInnerCarton = JsonConvert.DeserializeObject<ClosedPrescanInnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoClosedPrescanInnerCarton.Insert(closedPrescanInnerCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(closedPrescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ClosedPrescanInnerCarton");
//        //                                            break;
//        //                                        case "Company":
//        //                                            Company company = JsonConvert.DeserializeObject<Company>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoCompany.InsertCompany(company);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(company, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "Company");
//        //                                            break;
//        //                                        case "ODataSetup":
//        //                                            ODataSetup oDataSetup = JsonConvert.DeserializeObject<ODataSetup>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoODataSetup.InsertODataSetup(oDataSetup);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(oDataSetup, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "ODataSetup");
//        //                                            break;
//        //                                        case "Setup":
//        //                                            Setup setup = JsonConvert.DeserializeObject<Setup>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoSetup.Insert(setup);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(setup, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "Setup");
//        //                                            break;
//        //                                    }
//        //                                    socketClient.Send(insertReturn, insertReturn.Length, SocketFlags.None);

//        //                                    break;
//        //                                case "Delete":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "User":
//        //                                            User user = JsonConvert.DeserializeObject<User>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoUser.DeleteUser(user);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(user, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Delete", "User");
//        //                                            break;
//        //                                        case "CustomerGroup":
//        //                                            CustomerGroup customerGroup = JsonConvert.DeserializeObject<CustomerGroup>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoCustomerGroup.DeleteCustomerGroup(customerGroup);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(customerGroup, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "CustomerGroup");
//        //                                            break;
//        //                                        case "Printer":
//        //                                            Printer printer = JsonConvert.DeserializeObject<Printer>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrinter.DeletePrinter(printer);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(printer, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "Printer");
//        //                                            break;
//        //                                        case "PackingHeader":
//        //                                            PackingHeader packingHeader = JsonConvert.DeserializeObject<PackingHeader>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPackingHeader.Delete(packingHeader);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(packingHeader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "PackingHeader");
//        //                                            break;
//        //                                        case "PackingLine":
//        //                                            PackingLine packingLine = JsonConvert.DeserializeObject<PackingLine>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPackingLine.Delete(packingLine);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(packingLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "PackingLine");
//        //                                            break;
//        //                                        case "Item":
//        //                                            Item item = JsonConvert.DeserializeObject<Item>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoItem.DeleteItem(item);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(item, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Item");
//        //                                            break;
//        //                                        case "Mapping":
//        //                                            Mapping mapping = JsonConvert.DeserializeObject<Mapping>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoMapping.DeleteMapping(mapping);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(mapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Insert", "Mapping");
//        //                                            break;
//        //                                        case "LabelHeader":
//        //                                            LabelHeader labelHeader = JsonConvert.DeserializeObject<LabelHeader>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoLabelHeader.DeleteLabelHeader(labelHeader);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(labelHeader, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Delete", "LabelHeader");
//        //                                            break;
//        //                                        case "LabelLine":
//        //                                            LabelLine labelLine = JsonConvert.DeserializeObject<LabelLine>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoLabelLine.DeleteLabelLine(labelLine);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(labelLine, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Delete", "LabelLine");
//        //                                            break;
//        //                                        case "ScanLabelString":
//        //                                            ScanLabelString scanLabelString = JsonConvert.DeserializeObject<ScanLabelString>(commuForm.Str.Remove(0, 1));
//        //                                            //effectedRows = daoScanLabelString.Delete(scanLabelString);
//        //                                            //SendMesageToConnectedAndroid(JsonConvert.SerializeObject(scanLabelString, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "ScanLabelString");
//        //                                            break;
//        //                                        case "OuterCarton":
//        //                                            OuterCarton outerCarton = JsonConvert.DeserializeObject<OuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoOuterCarton.Delete(outerCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(outerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "OuterCarton");
//        //                                            break;
//        //                                        case "InnerCarton":
//        //                                            InnerCarton innerCarton = JsonConvert.DeserializeObject<InnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoInnerCarton.Delete(innerCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(innerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "InnerCarton");
//        //                                            break;
//        //                                        case "Prescan":
//        //                                            Prescan prescan = JsonConvert.DeserializeObject<Prescan>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescan.Delete(prescan);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescan, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Prescan");
//        //                                            break;
//        //                                        case "PrescanOuterCarton":
//        //                                            PrescanOuterCarton prescanOuterCarton = JsonConvert.DeserializeObject<PrescanOuterCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescanOuterCarton.DeletePrescanOuterCarton(prescanOuterCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanOuterCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Delete", "PrescanOuterCarton");
//        //                                            break;
//        //                                        case "PrescanInnerCarton":
//        //                                            PrescanInnerCarton prescanInnerCarton = JsonConvert.DeserializeObject<PrescanInnerCarton>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPrescanInnerCarton.Delete(prescanInnerCarton);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(prescanInnerCarton, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "PrescanInnerCarton");
//        //                                            break;
//        //                                        case "PackingMapping":
//        //                                            PackingMapping packingMapping = JsonConvert.DeserializeObject<PackingMapping>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoPackingMapping.Delete(packingMapping);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(packingMapping, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "PackingMapping");
//        //                                            break;
//        //                                        case "Company":
//        //                                            Company company = JsonConvert.DeserializeObject<Company>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoCompany.DeleteCompany(company);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(company, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Company");
//        //                                            break;
//        //                                        case "Setup":
//        //                                            Setup setup = JsonConvert.DeserializeObject<Setup>(commuForm.Str.Remove(0, 1));
//        //                                            effectedRows = daoSetup.Delete(setup);
//        //                                            SendMesageToConnectedAndroid(JsonConvert.SerializeObject(setup, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Update", "Setup");
//        //                                            break;
//        //                                    }
//        //                                    byteNumB = new Byte[effectedRows * 2];
//        //                                    byteNumB = System.Text.Encoding.BigEndianUnicode.GetBytes(effectedRows.ToString());
//        //                                    socketClient.Send(byteNumB, byteNumB.Length, SocketFlags.None);
//        //                                    break;
//        //                                case "Login":
//        //                                    switch (commuForm.Table)
//        //                                    {
//        //                                        case "User":
//        //                                            User loginUser = JsonConvert.DeserializeObject<User>(commuForm.Str.Remove(0, 1));
//        //                                            dt = dao.GetUser(loginUser.UserID);
//        //                                            String loginmsg = "Fail";
//        //                                            if (dt.Rows.Count > 0)
//        //                                            {
//        //                                                dt.TableName = "USER";
//        //                                                List<User> userlist = new List<User>();
//        //                                                userlist = (from DataRow dr in dt.Rows
//        //                                                            select new User()
//        //                                                            {
//        //                                                                UserID = dr["User ID"].ToString(),
//        //                                                                Password = dr["Password"].ToString()
//        //                                                            }).ToList();
//        //                                                foreach (User user in userlist)
//        //                                                {
//        //                                                    if (loginUser.Password == user.Password)
//        //                                                    {
//        //                                                        loginmsg = "OK";
//        //                                                    }
//        //                                                    else
//        //                                                    {
//        //                                                        loginmsg = "Incorrect Password";
//        //                                                    }
//        //                                                }
//        //                                            }
//        //                                            else
//        //                                            {
//        //                                                loginmsg = "User does not exist";
//        //                                            }
//        //                                            json = JsonConvert.SerializeObject(loginmsg, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
//        //                                            Byte[] byteNum = new Byte[json.Length];
//        //                                            byteNum = System.Text.Encoding.BigEndianUnicode.GetBytes(json);
//        //                                            socketClient.Send(byteNum, byteNum.Length, SocketFlags.None);
//        //                                            break;
//        //                                    }
//        //                                    break;
//        //                            }
//        //                            break;

//        //                    }


//        //        }
//        //    }
//        //    catch (SocketException _e)
//        //    {
//        //        ShowMsg("Disconnected: error code {0}! " + _e.NativeErrorCode);
//        //        ShowMsg("收到來自 --> " + socketClient.RemoteEndPoint.ToString() + " 的指令，但執行失敗\r\n");
//        //        RemoveListBox(socketClient.RemoteEndPoint.ToString());
//        //        //try { socketClient.Shutdown(SocketShutdown.Both); } catch { }
//        //        //try { socketClient.Close(); } catch { }
//        //    }
//        //    catch (Exception _e1)
//        //    {
//        //        ShowMsg("Disconnected: error code {0}! " + _e1.Message);
//        //        RemoveListBox(socketClient.RemoteEndPoint.ToString());
//        //        //try { socketClient.Shutdown(SocketShutdown.Both); } catch { }
//        //        //try { socketClient.Close(); } catch { }
//        //    }
//        //    finally
//        //    {
//        //        // 釋放資源
//        //        try { sr?.Close(); } catch { }
//        //        try { sw?.Close(); } catch { }
//        //        try { ns?.Close(); } catch { }
//        //        try { socketClient?.Close(); } catch { }

//        //        dao = null;
//        //        daoClosedPrescan = null;
//        //        daoClosedPrescanInnerCarton = null;
//        //        daoClosedPrescanOuterCarton = null;
//        //        daoConnection = null;
//        //        daoCustomerGroup = null;
//        //        daoItem = null;
//        //        daoLabelHeader = null;
//        //        daoLabelLine = null;
//        //        daoMapping = null;
//        //        daoPackingHeader = null;
//        //        daoPackingLine = null;
//        //        daoPackingMapping = null;
//        //        daoPrescan = null;
//        //        daoPrescanInnerCarton = null;
//        //        daoPrescanOuterCarton = null;
//        //        daoPrinter = null;
//        //        daoScanLabelString = null;
//        //        daoScannedPackingHeader = null;
//        //        daoScannedPackingLine = null;
//        //        daoScannedPackingMapping = null;
//        //        daoSynchronize = null;
//        //        daoCompany = null;
//        //        daoUser = null;
//        //        daoODataSetup = null;
//        //        daoSetup = null;
//        //        MsgTxt = null;
//        //    }
//        //}

//        // ========== 第四步：提取命令处理逻辑（简化主方法） ==========
//        private void HandleCommand(CommuForm commuForm, Socket socketClient, StreamWriter sw, string remoteEndPoint)
//        {
//            int batchCount = 0;
//            switch (commuForm.Command)
//            {
//                case "Open":
//                    HandleOpenCommand(commuForm, socketClient);
//                    break;
//                case "SQL":
//                    HandleSqlCommand(commuForm, socketClient, sw, ref batchCount);
//                    break;
//                case "New":
//                    HandleNewCommand(commuForm);
//                    break;
//                case "SQL_W":
//                    HandleSqlWCommand(commuForm, socketClient, sw);
//                    break;
//                default:
//                    // 空命令处理
//                    commuForm = new CommuForm("", "", "", "");
//                    string json = JsonConvert.SerializeObject(commuForm, GlobalConfig.JsonSettings);
//                    sw.WriteLine(json);
//                    break;
//            }
//        }

//        // ========== 第五步：拆分各命令处理方法（示例：SQL 命令处理） ==========
//        private void HandleSqlCommand(CommuForm commuForm, Socket socketClient, StreamWriter sw, ref int batchCount)
//        {
//            switch (commuForm.Action)
//            {
//                case "Insert":
//                    HandleInsertCommand(commuForm, socketClient, sw, ref batchCount);
//                    break;
//                case "Delete":
//                    HandleDeleteCommand(commuForm, socketClient, sw, ref batchCount);
//                    break;
//                case "Reset":
//                    HandleResetCommand(commuForm);
//                    break;
//                case "Select":
//                    // 优化：查询分页（核心！减少内存占用）
//                    HandleSelectCommand(commuForm, sw, ref batchCount, pageSize: GlobalConfig.BatchSendThreshold);
//                    break;
//                case "Update":
//                    HandleUpdateCommand(commuForm, socketClient, sw);
//                    break;
//                case "Finish":
//                    HandleFinishCommand(commuForm);
//                    break;
//                case "Connect":
//                    HandleConnectCommand(commuForm, sw);
//                    break;
//            }
//        }
//        private void HandleOpenCommand(CommuForm commuForm, Socket socketClient)
//        {

//        }

//        private void HandleNewCommand(CommuForm commuForm)
//        {

//        }

//        private void HandleSqlWCommand(CommuForm commuForm, Socket socketClient, StreamWriter sw)
//        {

//        }

//        private void HandleInsertCommand(CommuForm commuForm, Socket socketClient, StreamWriter sw, ref int batchCount)
//        {

//        }
//        private void HandleDeleteCommand(CommuForm commuForm, Socket socketClient, StreamWriter sw, ref int batchCount)
//        {

//        }
//        private void HandleResetCommand(CommuForm commuForm)
//        {

//        }
//        private void HandleUpdateCommand(CommuForm commuForm, Socket socketClient, StreamWriter sw)
//        {

//        }
//        private void HandleFinishCommand(CommuForm commuForm)
//        {

//        }
//        private void HandleConnectCommand(CommuForm commuForm, StreamWriter sw)
//        {

//        }

//        // 分页查询处理（核心优化：避免加载全量数据）
//        private void HandleSelectCommand(CommuForm commuForm, StreamWriter sw, ref int batchCount, int pageSize)
//        {
//            string macAddress = commuForm.Str;
//            switch (commuForm.Table)
//            {
//                case "User":
//                    ShowMsg("查询用户列表（分页）");
//                    // 优化1：分页查询，而非查全部
//                    //List<User> userList = DaoManager.DaoUser.SelectUserByPage(pageIndex: 1, pageSize: pageSize); // 需新增分页查询方法
//                    // 发送总数
//                    User blankUser = new User();
//                    List<User> userList = DaoManager.DaoUser.SelectUser(blankUser);
//                    CommuForm countForm = new CommuForm("Reply", "Count", "Count", $"@{userList.Count}");
//                    string jsonQty = JsonConvert.SerializeObject(countForm, GlobalConfig.JsonSettings);
//                    sw.WriteLine(jsonQty + DELIMITER);

//                    // 批量发送
//                    foreach (User user in userList)
//                    {
//                        SendBatchData(sw, "Reply", "Insert", "User", user, ref batchCount);
//                    }
//                    break;

//                case "PackingHeader":
//                    //ShowMsg("查询装箱头列表（分页）");
//                    //List<PackingHeader> packingHeaderList = DaoManager.DaoPackingHeader.SelectPackingHeaderByPage(1, pageSize);
//                    //countForm = new CommuForm("Reply", "Count", "Count", $"@{packingHeaderList.Count}");
//                    //jsonQty = JsonConvert.SerializeObject(countForm, GlobalConfig.JsonSettings);
//                    //sw.WriteLine(jsonQty + DELIMITER);
//                    //
//                    //foreach (PackingHeader header in packingHeaderList)
//                    //{
//                    //    SendBatchData(sw, "Reply", "Insert", "Packing Header", header, ref batchCount);
//                    //    // 关联数据也分页
//                    //    List<PackingLine> lineList = DaoManager.DaoPackingLine.PackingLinebyDocNoPage(header.No, 1, pageSize);
//                    //    foreach (PackingLine line in lineList)
//                    //    {
//                    //        SendBatchData(sw, "Reply", "Insert", "Packing Line", line, ref batchCount);
//                    //    }
//                    //}
//                    break;

//                    // 其他表同理，都改为分页查询...
//            }
//        }

//        // 批量发送数据（减少 Sleep 次数，优化 CPU）
//        private void SendBatchData<T>(StreamWriter sw, string reply, string action, string table, T data, ref int batchCount)
//        {
//            CommuForm form = new CommuForm(reply, action, table, $"@{JsonConvert.SerializeObject(data, GlobalConfig.JsonSettings)}");
//            string json = JsonConvert.SerializeObject(form, GlobalConfig.JsonSettings);
//            sw.WriteLine(json + DELIMITER);

//            batchCount++;
//            // 达到阈值才休眠，减少休眠次数
//            if (batchCount >= GlobalConfig.BatchSendThreshold)
//            {
//                Thread.Sleep(GlobalConfig.BatchSendDelay);
//                batchCount = 0;
//            }
//        }



//        public List<T> Deserialize<T>(string path)
//        {
//            return JsonConvert.DeserializeObject<List<T>>(path);
//        }

//        public void RemoveListBox(string ip)
//        {
//            BeginInvoke(new Action(() =>
//            {
//                int index = IPlistBox.FindString(ip);
//                if (index >= 0 && index < IPlistBox.Items.Count)
//                {
//                    IPlistBox.Items.RemoveAt(index);
//                }
//                Socket removedSocket;
//                string removedType;
//                dic.TryRemove(ip, out removedSocket);
//                dic2.TryRemove(ip, out removedType);
//            }));
//        }

//        public void ShowMsg(string p)
//        {
//            BeginInvoke(new Action(() =>
//            {
//                richTextBox1.AppendText(p + "\r\n");
//            }));
//        }
//        public void updatemsg(string p)
//        {
//            BeginInvoke(new Action(() =>
//            {
//                richTextBox1.AppendText(p + "\r\n");
//            }));
//        }

//        private void Sendbtn_Click(object sender, EventArgs e)
//        {
//            try
//            {

//            }
//            catch
//            {
//                MsgTextBox.Text = IPlistBox.SelectedItem.ToString();
//            }
//        }
//        private byte[] GetKeepAliveSetting(int onOff, int keepAliveTime, int keepAliveInterval)
//        {
//            byte[] buffer = new byte[12];
//            BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
//            BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
//            BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
//            return buffer;
//        }

//        public void SendMesageToConnectedAndroid(string msg, Socket socketClient, string action, string table)
//        {
//            CommuForm commuForm = new CommuForm("Reply", action, table, "@" + msg);
//            string json3 = JsonConvert.SerializeObject(commuForm);

//            // 若 dic2 為空則不做任何動作
//            if (!IsNullOrEmpty2(dic2))
//            {
//                foreach (var x in dic2)
//                {
//                    try
//                    {
//                        if ((x.Key != socketClient.RemoteEndPoint.ToString()) && (x.Value == "Android"))
//                        {
//                            Socket socketClient2;
//                            if (dic.TryGetValue(x.Key, out socketClient2) && socketClient2 != null)
//                            {
//                                using (var ns2 = new NetworkStream(socketClient2, false))
//                                using (var sw2 = new StreamWriter(ns2, Encoding.UTF8) { AutoFlush = true })
//                                {
//                                    sw2.WriteLine(json3 + DELIMITER);
//                                }
//                                ShowMsg("Send to --> " + x.Key + ": All 　：" + json3);
//                                Thread.Sleep(200);
//                            }
//                        }
//                        else
//                        {
//                            // 傳回來源socket
//                            using (var ns2 = new NetworkStream(socketClient, false))
//                            using (var sw2 = new StreamWriter(ns2, Encoding.UTF8) { AutoFlush = true })
//                            {
//                                sw2.WriteLine(json3);
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        ShowMsg("SendMesageToConnectedAndroid 傳送失敗: " + ex.Message);
//                    }
//                }
//            }
//        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//            Dao dao = new Dao();
//            dao.SyncPackingList();
//        }

//        private void button2_Click(object sender, EventArgs e)
//        {
//            DatabaseSetup dbSetup = new DatabaseSetup();
//            dbSetup.ShowDialog();
//        }
//        public void getDBSetup(ref String _DataSource, ref String _InitialCatalog, ref String _UserID, ref String _Password)
//        {
//            _DataSource = DataSource;
//            _InitialCatalog = InitialCatalog;
//            _UserID = UserID;
//            _Password = Password;
//        }

//        private void button3_Click(object sender, EventArgs e)
//        {
//            DatabaseAction dbAction = new DatabaseAction();
//            dbAction.ShowDialog();
//        }

//        private void button4_Click(object sender, EventArgs e)
//        {
//            ODataSetupPage oDataSetupPage = new ODataSetupPage();
//            oDataSetupPage.ShowDialog();
//        }

//        private void button5_Click(object sender, EventArgs e)
//        {

//        }

//        private void richTextBox1_TextChanged(object sender, EventArgs e)
//        {
//            richTextBox1.SelectionStart = richTextBox1.TextLength;
//            richTextBox1.ScrollToCaret();
//        }
//        public static bool IsNullOrEmpty2(ConcurrentDictionary<string, string> Dictionary)
//        {
//            return (Dictionary == null || Dictionary.Count < 1);
//        }
//    }
//}
