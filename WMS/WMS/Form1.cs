using ExtensionMethods;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using WMS.Database_Dao;
using WMS.Metadata;
using WMS.Models;
using WMS.Page;
using WMS.ReceiveMessage;
namespace WMS
{
    public partial class Form1 : Form
    {
        Socket ServerSocket = null;
        // ?????Dictionary ? RemoteEndPoint ? key
        ConcurrentDictionary<string, Socket> dic = new ConcurrentDictionary<string, Socket>();
        ConcurrentDictionary<string, string> dic2 = new ConcurrentDictionary<string, string>();
        // ?? Thread.Abort??? CancellationTokenSource
        CancellationTokenSource cts;
        Thread thread;

        Boolean booStart = false;
        static String DataSource;
        static String InitialCatalog;
        static String UserID;
        static String Password;
        public static NAV.NAV nav;
        static Boolean timerStarted;
        TimersTimer timera = new TimersTimer();


        public Form1()
        {
            InitializeComponent();
            Patrolcar patrolcar = new Patrolcar();
            patrolcar.dgd_ini.getIni();
            StartStopBtn.Text = "Start";
            //IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = host.AddressList[0];
            //foreach (var ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        Console.WriteLine("IP Address = " + ip.ToString());
            //    }
            //}
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Console.WriteLine("No Network Available");
            }

            ////   LoadIpAddressesToComboBox();++++++++++++++++++++++++++++++   ////
            //IPHostEntry host2 = Dns.GetHostEntry(Dns.GetHostName());

            //var ippaddress = host2
            //    .AddressList
            //    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            ////Console.WriteLine(ippaddress);

            //IPtextBox.Text = ippaddress.ToString();

            LoadIpAddressesToComboBox();
            ////   LoadIpAddressesToComboBox();++++++++++++++++++++++++++++++   ////

            // 注入連線設定供 DAO 層使用，避免 Dao_Connection 依賴 Form1
            DbConnectionConfig.Set(
                Properties.Settings.Default.DataSource ?? "",
                Properties.Settings.Default.InitialCatalog ?? "",
                Properties.Settings.Default.DBUserID ?? "",
                Properties.Settings.Default.DBPassword ?? "");
            // Load socket timeout settings into UI (defaults: 5 min receive, 1 min send)
            int recv = Properties.Settings.Default.SocketReceiveTimeoutMs;
            int send = Properties.Settings.Default.SocketSendTimeoutMs;
            if (recv >= 1000) nudReceiveTimeoutMs.Value = Math.Min(recv, (int)nudReceiveTimeoutMs.Maximum);
            if (send >= 1000) nudSendTimeoutMs.Value = Math.Min(send, (int)nudSendTimeoutMs.Maximum);
        }

        private void LoadIpAddressesToComboBox()
        {
            try
            {
                // ????????? IP ??
                comboBox1.Items.Clear();

                // ?????????? IP
                IPHostEntry host2 = Dns.GetHostEntry(Dns.GetHostName());

                // ?? IPv4?AddressFamily.InterNetwork ? IPv4?
                var ipv4Addresses = host2.AddressList
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                    .ToList();

                // ???? IPv4 ??
                if (ipv4Addresses.Count == 0)
                {
                    comboBox1.Items.Add("??? IPv4 ??");
                }
                else
                {
                    // ??? IPv4 ?? comboBox1
                    foreach (var ip in ipv4Addresses)
                    {
                        comboBox1.Items.Add(ip.ToString());
                    }
                    // ?????? IP
                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // ?? IP ????????
                comboBox1.Items.Clear();
                comboBox1.Items.Add($"???? IP?{ex.Message}");
                MessageBox.Show($"?????? IP ???{ex.Message}", "??",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Startbtn_Click(object sender, EventArgs e)
        {
            if (!booStart)
            {
                try
                {
                    // Persist socket timeout settings for this and future runs
                    int recv = (int)nudReceiveTimeoutMs.Value;
                    int send = (int)nudSendTimeoutMs.Value;
                    if (recv >= 1000 && send >= 1000)
                    {
                        Properties.Settings.Default.SocketReceiveTimeoutMs = recv;
                        Properties.Settings.Default.SocketSendTimeoutMs = send;
                        Properties.Settings.Default.Save();
                    }
                    IPEndPoint IPE = new IPEndPoint(IPAddress.Parse(comboBox1.Text), Int32.Parse(ProttextBox.Text));
                    ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    ServerSocket.Bind(IPE);
                    ServerSocket.Listen(10);
                    StartStopBtn.Text = "Stop";
                    booStart = true;
                    DataSource = Properties.Settings.Default.DataSource;
                    InitialCatalog = Properties.Settings.Default.InitialCatalog;
                    UserID = Properties.Settings.Default.DBUserID;
                    Password = Properties.Settings.Default.DBPassword;
                    DbConnectionConfig.Set(DataSource, InitialCatalog, UserID, Password);
                    showmsg("服務器已啓動，監聽中...");
                    Log log = new Log();
                    List<String> valueTitle = null;
                    List<String> value = null;
                    log.EventLog("Server Start", Environment.MachineName, valueTitle, value);
                    log = null;

                    InitializeTableTypeMapping();


                    // ?? CancellationTokenSource ??????????
                    cts = new CancellationTokenSource();
                    thread = new Thread(() => ListenClientConnect(cts.Token));
                    thread.IsBackground = true;
                    thread.Start();

                    if (!timerStarted)
                    {
                        timera.InitTimer();
                        timera.timer_Start();
                        timerStarted = true;
                    }
                }
                catch (Exception ex)
                {
                    showmsg("??????????: " + ex.Message);
                    booStart = false;
                }
            }
            else
            {
                // ??????? Thread.Abort()
                try
                {
                    booStart = false;
                    // ?????????? ServerSocket ??? Accept ???
                    cts?.Cancel();
                    try { ServerSocket?.Close(); } catch { }

                    // ??????????
                    if (thread != null && thread.IsAlive)
                    {
                        thread.Join(1000);
                    }

                    timera.timer_Stop();
                    StartStopBtn.Text = "Start";

                    DataSource = "";
                    InitialCatalog = "";
                    UserID = "";
                    Password = "";
                    showmsg("??????");
                    Log log = new Log();
                    List<String> valueTitle = null;
                    List<String> value = null;
                    log.EventLog("Server Stop", Environment.MachineName, valueTitle, value);
                    log = null;

                    // ???????
                    foreach (var kv in dic)
                    {
                        try
                        {
                            kv.Value.Shutdown(SocketShutdown.Both);
                            kv.Value.Close();
                        }
                        catch { }
                    }
                    dic.Clear();
                    dic2.Clear();
                    IPlistBox.Items.Clear();
                }
                catch (Exception ex)
                {
                    showmsg("??????????: " + ex.Message);
                }
            }
        }
        // ? ListenClientConnect ????? CancellationToken
        private void ListenClientConnect(object tokenObj)
        {
            var token = tokenObj as CancellationToken?;
            try
            {
                while (booStart && (token == null || !token.Value.IsCancellationRequested))
                {
                    Socket socketClient = null;
                    try
                    {
                        // Accept ?? ServerSocket.Close() ????????????
                        socketClient = ServerSocket.Accept();
                    }
                    catch (SocketException)
                    {
                        // ??? Close() ????????????
                        if (token != null && token.Value.IsCancellationRequested) break;
                        else throw;
                    }

                    if (socketClient == null) break;

                    try
                    {
                        int recvMs = Properties.Settings.Default.SocketReceiveTimeoutMs;
                        int sendMs = Properties.Settings.Default.SocketSendTimeoutMs;
                        if (recvMs < 1000) recvMs = 300000;
                        if (sendMs < 1000) sendMs = 60000;
                        socketClient.ReceiveTimeout = recvMs;
                        socketClient.SendTimeout = sendMs;
                        string RemoteIP = socketClient.RemoteEndPoint.ToString();
                        dic.TryAdd(RemoteIP, socketClient);
                        dic2.TryAdd(RemoteIP, "");
                        Invoke(new MethodInvoker(() => IPlistBox.Items.Add(RemoteIP)));
                        showmsg(RemoteIP + "???");

                        Thread.Sleep(200);
                        Thread recieveThread = new Thread(recievemsg);
                        recieveThread.IsBackground = true;
                        recieveThread.Start(socketClient);
                    }
                    catch (Exception exInner)
                    {
                        showmsg("??????????: " + exInner.Message);
                        try { socketClient?.Close(); } catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                // ???????????
                showmsg("ListenClientConnect ????: " + ex.Message);
            }
        }
        public void recievemsg(object soc)
        {
            Socket socketClient = soc as Socket;
            if (socketClient == null) return;

            CommuForm commuForm = new CommuForm("", "", "", "");
            NetworkStream ns = null;
            StreamReader sr = null;
            StreamWriter sw = null;
            string RemoteEndPointtxt = socketClient.RemoteEndPoint.ToString();
            MessageText MsgTxt = new MessageText();

            DateTime dateTime = DateTime.Now;
            string json = "";
            string msg;

            try
            {
                ns = new NetworkStream(socketClient);
                sr = new StreamReader(ns, Encoding.BigEndianUnicode);
                sw = new StreamWriter(ns, Encoding.BigEndianUnicode) { AutoFlush = true };

                while (true)
                {
                    if (!booStart)
                    {
                        // ??????????
                        RemoveListBox(socketClient.RemoteEndPoint.ToString());
                        try { socketClient.Shutdown(SocketShutdown.Both); } catch { }
                        try { socketClient.Close(); } catch { }
                        break;
                    }

                    msg = null;
                    try
                    {
                        msg = sr.ReadLine();

                        if (string.IsNullOrEmpty(msg))
                        {
                            Thread.Sleep(200);
                            continue;
                        }
                        showmsg("[" + msg + "]");
                    }
                    catch (IOException)
                    {
                        // ????
                        showmsg("[Disconnect]");
                        msg = null;
                        break;
                    }

                    if (string.IsNullOrEmpty(msg))
                    {
                        continue;
                    }
                    else
                    {
                    }

                    // ?? JSON ?????
                    try
                    {
                        commuForm = JsonConvert.DeserializeObject<CommuForm>(msg);
                        showmsg(commuForm.Action);
                        showmsg(commuForm.Command);
                    }
                    catch (JsonException jex)
                    {
                        showmsg("Failed to parse the received message: " + jex.Message);
                        continue; // ???????
                    }

                    switch (commuForm.Command)
                    {
                        case "Open":
                            switch (commuForm.Action)
                            {
                                case "List":
                                    switch (commuForm.Table)
                                    {
                                        case "UserList":
                                            UserList userList = new UserList();
                                            userList.OpenUserList(socketClient, this);
                                            break;
                                        case "UserCard":
                                            User_Control userCardControl = new User_Control();
                                            userCardControl.OpenUserCard();
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case "SQL":
                            //string remoteIp = socketClient.RemoteEndPoint.ToString();
                            //bool isAndroid = dic2.TryGetValue(remoteIp, out string cType) && cType == "Android";
                            //if (!isAndroid)
                            //{
                            int effectedRows = 0;
                            string responseJson = string.Empty;
                            if (string.IsNullOrEmpty(DbConnectionConfig.DataSource))
                            {
                                responseJson = SerializeToJson(new { Code = -2, Msg = "Database not configured. Please configure and start the WMS server." });
                                SendUnifiedResponse(socketClient, responseJson);
                            }
                            else
                            {
                                try
                                {
                                    // ✨ 核心改造：通用處理所有表，自動適配單個/集合
                                    responseJson = ProcessData(commuForm.Table, commuForm.Action, commuForm.Str.Remove(0, 1));
                                }
                                catch (Exception ex)
                                {
                                    responseJson = SerializeToJson(new { Code = -2, Msg = GetDbErrorMessage(ex) });
                                }
                                SendUnifiedResponse(socketClient, responseJson);
                            }
                            break;

                        case "GetConfig":
                            string pageCode = !string.IsNullOrWhiteSpace(commuForm.Table) ? commuForm.Table : "Shell";
                            var config = GetPageConfig(pageCode);
                            SendUnifiedResponse(socketClient, SerializeToJson(config));
                            break;
                        case "Reset":
                            switch (commuForm.Table)
                            {
                                case "Prescan":
                                    Prescan prescan = JsonConvert.DeserializeObject<Prescan>(commuForm.Str.Remove(0, 1));
                                    String DocNo = prescan.DocumentNo;
                                    int effectedPrescanOuterCartonRows = DaoManager.dao.Prescan_Reset(DocNo);
                                    Console.WriteLine(effectedPrescanOuterCartonRows);
                                    break;
                            }
                            break;
                        case "Finish":
                            switch (commuForm.Table)
                            {
                                case "Standrad Processing":
                                    PackingMapping packingMapping = JsonConvert.DeserializeObject<PackingMapping>(commuForm.Str.Remove(0, 1));
                                    int effectedInsertperscanRows = DaoManager.dao.FinishStandradProcessing(packingMapping.PackingNo, packingMapping.PrescanNo);

                                    break;
                                default:
                                    break;
                            }
                            break;

                        case "Connect":
                            try
                            {
                                String value;
                                if (dic2.TryGetValue(socketClient.RemoteEndPoint.ToString(), out value))
                                {
                                    dic2[socketClient.RemoteEndPoint.ToString()] = "Android";
                                }
                                else
                                {
                                    dic2[socketClient.RemoteEndPoint.ToString()] = "Android";
                                    //dic2.Add(socketClient.RemoteEndPoint.ToString(), "Android");
                                }
                                commuForm = new CommuForm("Reply", "Connect", "Connect", "@OK");
                                json = JsonConvert.SerializeObject(commuForm);
                                //        sw.WriteLine(json2 + DELIMITER);
                                object response = string.Empty;

                                response = ResponseForm<object>.Success("Connect", json);
                                SendUnifiedResponse(socketClient, (string)response);
                                showmsg(json + "");
                            }
                            catch (KeyNotFoundException)
                            {
                                Console.WriteLine("Key = " + socketClient.RemoteEndPoint.ToString() + " is not found.");
                            }
                            break;

                        default:
                            commuForm.Command = "";
                            commuForm.Action = "";
                            commuForm.Table = "";
                            commuForm.Str = "";
                            showmsg(commuForm.Str + "");
                            json = JsonConvert.SerializeObject(commuForm);
                            SendUnifiedResponse(socketClient, json);
                            break;
                        case "New":
                            switch (commuForm.Action)
                            {
                                case "Device":
                                    DaoManager.dao.InitSynchronize(commuForm.Str, DateTime.MinValue);
                                    break;
                            }
                            break;
                    }

                }
            }
            catch (SocketException _e)
            {
                showmsg("Disconnected: error code {0}! " + _e.NativeErrorCode);
                showmsg("???? --> " + socketClient.RemoteEndPoint.ToString() + " ?????????\r\n");
                RemoveListBox(socketClient.RemoteEndPoint.ToString());
                //try { socketClient.Shutdown(SocketShutdown.Both); } catch { }
                //try { socketClient.Close(); } catch { }
            }
            catch (Exception _e1)
            {
                showmsg("Disconnected: error code {0}! " + _e1.Message);
                RemoveListBox(RemoteEndPointtxt);
                //try { socketClient.Shutdown(SocketShutdown.Both); } catch { }
                //try { socketClient.Close(); } catch { }
            }
            finally
            {
                RemoveListBox(RemoteEndPointtxt);
                // ????
                try { sr?.Close(); } catch { }
                try { sw?.Close(); } catch { }
                try { ns?.Close(); } catch { }
                try { socketClient?.Close(); } catch { }

                MsgTxt = null;
            }
        }
        public List<T> Deserialize<T>(string path)
        {
            return JsonConvert.DeserializeObject<List<T>>(path);
        }

        public void RemoveListBox(string ip)
        {
            BeginInvoke(new Action(() =>
            {
                int index = IPlistBox.FindString(ip);
                if (index >= 0 && index < IPlistBox.Items.Count)
                {
                    IPlistBox.Items.RemoveAt(index);
                }
                Socket removedSocket;
                string removedType;
                dic.TryRemove(ip, out removedSocket);
                dic2.TryRemove(ip, out removedType);
            }));
        }

        public void showmsg(string p)
        {
            BeginInvoke(new Action(() =>
            {
                richTextBox1.AppendText(p + "\r\n");
            }));
        }
        public void updatemsg(string p)
        {
            BeginInvoke(new Action(() =>
            {
                richTextBox1.AppendText(p + "\r\n");
            }));
        }

        private void Sendbtn_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch
            {
                MsgTextBox.Text = IPlistBox.SelectedItem.ToString();
            }
        }
        //private CommuForm Deserialize(string json)
        //{
        //    //DataTable dt = new DataTable();
        //    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        //    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(CommuForm));
        //    return ser.ReadObject(ms) as CommuForm;
        //}

        //private String Serialize(object dt)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    DataContractJsonSerializer ser = new DataContractJsonSerializer(dt.GetType());
        //    ser.WriteObject(ms, dt);
        //    byte[] json = ms.ToArray();
        //    return Encoding.UTF8.GetString(json);
        //}
        private byte[] GetKeepAliveSetting(int onOff, int keepAliveTime, int keepAliveInterval)
        {
            byte[] buffer = new byte[12];
            BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
            BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
            BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
            return buffer;
        }

        //public void SendMesageToConnectedAndroid(string msg, Socket socketClient, string action, string table)
        //{
        //    CommuForm commuForm = new CommuForm("Reply", action, table, "@" + msg);
        //    string json3 = JsonConvert.SerializeObject(commuForm);

        //    // ? dic2 ???????? Android ???
        //    if (!IsNullOrEmpty2(dic2))
        //    {
        //        foreach (var x in dic2)
        //        {
        //            try
        //            {
        //                if ((x.Key != socketClient.RemoteEndPoint.ToString()) && (x.Value == "Android"))
        //                {
        //                    Socket socketClient2;
        //                    if (dic.TryGetValue(x.Key, out socketClient2) && socketClient2 != null)
        //                    {
        //                        using (var ns2 = new NetworkStream(socketClient2, false))
        //                        using (var sw2 = new StreamWriter(ns2, Encoding.BigEndianUnicode) { AutoFlush = true })
        //                        {
        //                            sw2.WriteLine(json3); // ? PC ???????? JSON
        //                        }
        //                        showmsg("Send to --> " + x.Key + ": All ?? " + json3);
        //                        Thread.Sleep(200);
        //                    }
        //                }
        //                else
        //                {
        //                    // ????? socket ????? JSON
        //                    using (var ns2 = new NetworkStream(socketClient, false))
        //                    using (var sw2 = new StreamWriter(ns2, Encoding.BigEndianUnicode) { AutoFlush = true })
        //                    {
        //                        sw2.WriteLine(json3);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                showmsg("SendMesageToConnectedAndroid ????: " + ex.Message);
        //            }
        //        }
        //    }
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            Dao dao = new Dao();
            dao.SyncPackingList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DatabaseSetup dbSetup = new DatabaseSetup();
            dbSetup.ShowDialog();
        }
        public void getDBSetup(ref String _DataSource, ref String _InitialCatalog, ref String _UserID, ref String _Password)
        {
            _DataSource = DataSource;
            _InitialCatalog = InitialCatalog;
            _UserID = UserID;
            _Password = Password;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DatabaseAction dbAction = new DatabaseAction();
            dbAction.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ODataSetupPage oDataSetupPage = new ODataSetupPage();
            oDataSetupPage.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
        }
        public static bool IsNullOrEmpty2(ConcurrentDictionary<string, string> Dictionary)
        {
            return (Dictionary == null || Dictionary.Count < 1);
        }

        /// <summary>
        /// ????? 4 ????? + BigEndianUnicode ??? JSON?????? PC/Android ????? JSON?
        /// </summary>
        /// <param name="socket">?? Socket</param>
        /// <param name="data">?????????? JSON?</param>
        private void SendUnifiedResponse(Socket socket, string data)
        {
            if (socket == null || !socket.Connected || string.IsNullOrEmpty(data)) return;

            try
            {
                // 1. ? BigEndianUnicode ?????
                byte[] dataBytes = Encoding.BigEndianUnicode.GetBytes(data);
                // 2. ?? 4 ??????????
                int dataSize = dataBytes.Length;
                byte[] lengthBytes = BitConverter.GetBytes(dataSize);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes); // ?????
                }
                // 3. ????????
                //socket.Send(lengthBytes, 2, SocketFlags.None);
                socket.Send(lengthBytes, 0, 4, SocketFlags.None);
                socket.Send(dataBytes, 0, dataBytes.Length, SocketFlags.None);
                Console.WriteLine($"SendUnifiedResponse DataSize={dataSize} Data={data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendUnifiedResponse ErrorMessage:{ex.Message}");
            }
        }


        private string SerializeToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                NullValueHandling = NullValueHandling.Include
            });
        }


        // 先定義全域CommonDao實例（複用連接，避免重複創建）
        private readonly CommonDao _commonDao = new CommonDao();
        private readonly EntityPageMetadataBuilder _metadataBuilder = new EntityPageMetadataBuilder();

        // 表名-實體類型映射（需提前初始化，如：_tableTypeMapping["Company"] = typeof(Company);）
        private readonly Dictionary<string, Type> _tableTypeMapping = new Dictionary<string, Type>();

        private string ProcessData(string tableName, string action, string dataJson)
        {
            try
            {
                // 1. 校驗表名映射
                if (!_tableTypeMapping.TryGetValue(tableName, out Type entityType))
                {
                    Console.WriteLine(_tableTypeMapping.Count);
                    foreach (var kvp in _tableTypeMapping)
                    {
                        Console.WriteLine($"資料表名稱: {kvp.Key}, 對應類別: {kvp.Value.FullName}");
                    }

                    return SerializeToJson(new { Code = -1, Msg = $"未知表名：{tableName}", EffectedRows = 0 });
                }

                // 2. 自動反序列化：相容單個物件/List集合
                object data = DeserializeAuto(entityType, dataJson);
                if (data == null)
                {
                    return SerializeToJson(new { Code = -2, Msg = $"反序列化失敗：{tableName}，數據={dataJson}", EffectedRows = 0 });
                }

                // 3. 提取核心操作物件（相容單個/List）
                object mainObj = GetFirstItemFromData(data, entityType); // 主物件（條件/插入/刪除）
                object updateObj = null; // Update專用：更新值對象

                // 4. 處理Update的第二個參數（條件+更新值）
                if (action == "Update")
                {
                    if (data is IList list && list.Count == 2)
                    {
                        mainObj = list[0]; // 條件物件
                        updateObj = list[1]; // 更新值對象
                    }
                    else
                    {
                        return SerializeToJson(new { Code = -3, Msg = "Update請求格式錯誤：必須是 [condition, entity]（且僅2個物件）", EffectedRows = 0 });
                    }

                    if (IsPrimaryKeyDefault(mainObj, entityType))
                    {
                        return SerializeToJson(new { Code = -5, Msg = "Update失敗：condition主鍵缺失或為預設值", EffectedRows = 0 });
                    }

                    if (IsPrimaryKeyDefault(updateObj, entityType))
                    {
                        return SerializeToJson(new { Code = -6, Msg = "Update失敗：entity主鍵缺失或為預設值", EffectedRows = 0 });
                    }

                    object condPk = GetPrimaryKeyValue(mainObj, entityType);
                    object entityPk = GetPrimaryKeyValue(updateObj, entityType);
                    if (!object.Equals(condPk, entityPk))
                    {
                        return SerializeToJson(new { Code = -7, Msg = "Update失敗：禁止修改主鍵欄位，請保持condition與entity主鍵一致", EffectedRows = 0 });
                    }
                }

                // 5. 根據Action調用CommonDao的泛型方法
                int effectedRows = 0;
                object resultData = null;

                // 獲取主鍵類型（通過BaseEntity<TKey>的泛型參數）
                Type keyType = GetPrimaryKeyType(entityType);
                if (keyType == null)
                {
                    return SerializeToJson(new { Code = -4, Msg = $"實體{entityType.Name}未繼承BaseEntity<TKey>", EffectedRows = 0 });
                }

                // 反射調用CommonDao的泛型方法（核心改造）
                switch (action)
                {
                    case "Select":
                        // 調用CommonDao.Select<TEntity, TKey>(tableName, mainObj)
                        resultData = CallCommonDaoGenericMethod(
                            "Select",
                            new[] { entityType, keyType },
                            new[] { tableName, mainObj });
                        break;

                    case "Insert":
                        // 調用CommonDao.Insert<TEntity, TKey>(tableName, mainObj)
                        effectedRows = (int)CallCommonDaoGenericMethod(
                            "Insert",
                            new[] { entityType, keyType },
                            new[] { tableName, mainObj });
                        break;

                    case "Update":
                        // 調用CommonDao.Update<TEntity, TKey>(tableName, mainObj, updateObj)
                        effectedRows = (int)CallCommonDaoGenericMethod(
                            "Update",
                            new[] { entityType, keyType },
                            new[] { tableName, mainObj, updateObj });
                        break;

                    case "Delete":
                        // 調用CommonDao.Delete<TEntity, TKey>(tableName, mainObj)
                        effectedRows = (int)CallCommonDaoGenericMethod(
                            "Delete",
                            new[] { entityType, keyType },
                            new[] { tableName, mainObj });
                        break;

                    default:
                        return SerializeToJson(new { Code = -1, Msg = $"未知操作：{action}", EffectedRows = 0 });
                }

                // 6. 統一返回回應
                return action == "Select"
                    ? SerializeToJson(resultData ?? new List<object>())
                    : SerializeToJson(new { Code = 0, Msg = "OK", EffectedRows = effectedRows });
            }
            catch (Exception ex)
            {
                return SerializeToJson(new { Code = -2, Msg = GetDbErrorMessage(ex), EffectedRows = 0 });
            }
        }


        /// <summary>
        /// 反射調用DAO方法
        /// </summary>
        private object CallDaoMethod(object daoInstance, string methodName, Type[] paramTypes, object[] paramValues)
        {
            MethodInfo method = daoInstance.GetType().GetMethod(methodName, paramTypes);
            if (method == null)
            {
                throw new MissingMethodException(daoInstance.GetType().Name, methodName);
            }
            return method.Invoke(daoInstance, paramValues);
        }

        #region 輔助方法：適配CommonDao泛型調用
        /// <summary>
        /// 獲取實體的主鍵類型（從BaseEntity<TKey>中提取）
        /// </summary>
        private Type GetPrimaryKeyType(Type entityType)
        {
            // 檢查是否繼承BaseEntity<TKey>
            Type baseType = entityType.BaseType;
            while (baseType != null && !baseType.IsGenericType)
            {
                baseType = baseType.BaseType;
            }
            return baseType?.GetGenericArguments()[0]; // 返回TKey類型（string/int等）
        }

        /// <summary>
        /// 反射調用CommonDao的泛型方法
        /// </summary>
        private object CallCommonDaoGenericMethod(string methodName, Type[] genericTypes, object[] parameters)
        {
            // 獲取CommonDao的泛型方法定義
            MethodInfo method = typeof(CommonDao)
                .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                ?.MakeGenericMethod(genericTypes);

            if (method == null)
            {
                throw new InvalidOperationException($"CommonDao無泛型方法：{methodName}<{string.Join(",", genericTypes.Select(t => t.Name))}>");
            }

            // 調用方法並返回結果
            return method.Invoke(_commonDao, parameters);
        }
        #endregion

        ///#region 原有輔助方法（保留，無需修改）
        /// <summary>
        /// 自動反序列化：相容單個物件/List集合
        /// </summary>
        private object DeserializeAuto(Type entityType, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            // JSON 陣列（例如 Update 的 [條件, 新值]）勿先反序列化成單一實體，否則會得到錯誤型別再進 catch 仍失敗
            if (json.TrimStart().StartsWith("["))
            {
                Type listType = typeof(List<>).MakeGenericType(entityType);
                return JsonConvert.DeserializeObject(json, listType);
            }

            try
            {
                return JsonConvert.DeserializeObject(json, entityType);
            }
            catch
            {
                Type listType = typeof(List<>).MakeGenericType(entityType);
                return JsonConvert.DeserializeObject(json, listType);
            }
        }

        /// <summary>
        /// 提取第一個資料物件（相容單個/List）
        /// </summary>
        private object GetFirstItemFromData(object data, Type entityType)
        {
            if (data == null) return Activator.CreateInstance(entityType);

            // 單個物件直接返回
            if (data.GetType() == entityType) return data;

            // List集合取第一個元素
            if (data is IList list && list.Count > 0) return list[0];

            // 預設返回空實例
            return Activator.CreateInstance(entityType);
        }

        private object GetPrimaryKeyValue(object entityObj, Type entityType)
        {
            if (entityObj == null || entityType == null) return null;
            PropertyInfo pkProp = entityType.GetProperty("_PrimaryKey", BindingFlags.Public | BindingFlags.Instance);
            if (pkProp == null) return null;
            return pkProp.GetValue(entityObj);
        }

        private bool IsPrimaryKeyDefault(object entityObj, Type entityType)
        {
            object pkValue = GetPrimaryKeyValue(entityObj, entityType);
            if (pkValue == null) return true;

            Type t = pkValue.GetType();
            if (!t.IsValueType)
            {
                if (pkValue is string s) return string.IsNullOrWhiteSpace(s);
                return false;
            }

            object def = Activator.CreateInstance(t);
            return pkValue.Equals(def);
        }

        /// <summary>
        /// 序列化JSON（統一格式）
        /// </summary>
        private string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                NullValueHandling = NullValueHandling.Include
            });
        }

        /// <summary>
        /// 資料庫錯誤資訊格式化（合併版）
        /// 1. 處理SQL登錄失敗的特殊提示
        /// 2. 拼接內部異常資訊，便於調試
        /// 3. 相容空異常、通用異常的友好提示
        /// </summary>
        /// <param name="ex">異常物件</param>
        /// <returns>使用者友好 + 調試友好的錯誤資訊</returns>
        private string GetDbErrorMessage(Exception ex)
        {
            // 空異常兜底
            if (ex == null)
            {
                return "資料庫操作失敗：未知錯誤";
            }

            // 1. 優先處理SQL登錄失敗的特殊場景（友好提示）
            var sqlEx = ex as SqlException;
            if (sqlEx != null && ex.Message.IndexOf("Login failed", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string loginErrorMsg = "資料庫登錄失敗，請檢查WMS伺服器資料庫配置（資料來源、用戶名、密碼）。";
                // 若有內部異常，補充調試資訊
                if (ex.InnerException != null)
                {
                    loginErrorMsg += $" | 內部原因：{ex.InnerException.Message}";
                }
                return loginErrorMsg;
            }

            // 2. 通用資料庫錯誤：拼接主異常 + 內部異常
            string baseMsg = $"資料庫操作失敗：{ex.Message}";
            if (ex.InnerException != null)
            {
                baseMsg += $" | 內部錯誤：{ex.InnerException.Message}";
            }

            return baseMsg;
        }
        private void InitializeTableTypeMapping()
        {
            _tableTypeMapping.Clear();
            _tableTypeMapping["User"] = typeof(User);
            _tableTypeMapping["Company"] = typeof(Company);
            _tableTypeMapping["CustomerGroup"] = typeof(CustomerGroup);
            _tableTypeMapping["Item"] = typeof(Item);
            _tableTypeMapping["PackingHeader"] = typeof(PackingHeader);
            _tableTypeMapping["PackingLine"] = typeof(PackingLine);
            _tableTypeMapping["Mapping"] = typeof(Mapping);
            _tableTypeMapping["ScanLabelString"] = typeof(ScanLabelString);
            _tableTypeMapping["Prescan"] = typeof(Prescan);
            _tableTypeMapping["OuterCarton"] = typeof(OuterCarton);
            _tableTypeMapping["InnerCarton"] = typeof(InnerCarton);
            _tableTypeMapping["PackingMapping"] = typeof(PackingMapping);
            _tableTypeMapping["LabelHeader"] = typeof(LabelHeader);
            _tableTypeMapping["LabelLine"] = typeof(LabelLine);
            _tableTypeMapping["ScannedPackingHeader"] = typeof(ScannedPackingHeader);
            _tableTypeMapping["ScannedPackingLine"] = typeof(ScannedPackingLine);
            _tableTypeMapping["ScannedPackingMapping"] = typeof(ScannedPackingMapping);
            _tableTypeMapping["ClosedPrescan"] = typeof(ClosedPrescan);
            _tableTypeMapping["ClosedPrescanOuterCarton"] = typeof(ClosedPrescanOuterCarton);
            _tableTypeMapping["ClosedPrescanInnerCarton"] = typeof(ClosedPrescanInnerCarton);
            _tableTypeMapping["PrescanOuterCarton"] = typeof(PrescanOuterCarton);
            _tableTypeMapping["PrescanInnerCarton"] = typeof(PrescanInnerCarton);
            _tableTypeMapping["Printer"] = typeof(Printer);
            _tableTypeMapping["ODataSetup"] = typeof(ODataSetup);
            _tableTypeMapping["Setup"] = typeof(Setup);
            _tableTypeMapping["Synchronize"] = typeof(Synchronize);
        }

        private List<MenuItem> BuildShellMenu()
        {
            return new List<MenuItem>
            {
                new MenuItem
                {
                    Text = "Master Data",
                    FormKind = "Crud",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Text = "User", FormName = "User", FormKind = "Crud" },
                        new MenuItem { Text = "Company", FormName = "Company", FormKind = "Crud" },
                        new MenuItem { Text = "Customer Group", FormName = "CustomerGroup", FormKind = "Crud" },
                        new MenuItem { Text = "Item", FormName = "Item", FormKind = "Crud" },
                        new MenuItem { Text = "Printer", FormName = "Printer", FormKind = "Crud" },
                        new MenuItem { Text = "Mapping", FormName = "Mapping", FormKind = "Crud" }
                    }
                },
                new MenuItem
                {
                    Text = "Operations",
                    FormKind = "Custom",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Text = "Packing List", FormName = "PackingList", FormKind = "Custom" },
                        new MenuItem { Text = "Prescan", FormName = "PrescanList", FormKind = "Custom" },
                        new MenuItem { Text = "Packing Mapping", FormName = "PackingMappingList", FormKind = "Custom" },
                        new MenuItem { Text = "Scanned Packing List", FormName = "ScannedPackingList", FormKind = "Custom" },
                        new MenuItem { Text = "Closed Prescan", FormName = "ClosedPrescanList", FormKind = "Custom" }
                    }
                },
                new MenuItem
                {
                    Text = "Setup",
                    FormKind = "Custom",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Text = "OData Setup", FormName = "ODataSetupPage", FormKind = "Custom" },
                        new MenuItem { Text = "Setup", FormName = "SetupPage", FormKind = "Custom" }
                    }
                }
            };
        }

        public PageConfig GetPageConfig(string pageCode)
        {
            return _metadataBuilder.BuildPageConfig(pageCode, _tableTypeMapping, BuildShellMenu);
        }

    }

}
