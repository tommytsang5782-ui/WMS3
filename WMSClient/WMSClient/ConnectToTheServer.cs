using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMSClient
{
    class ConnectToTheServer
    {
        //static Socket SocketClient;
        //static NetworkStream ns;
        //static StreamReader sr;
        //static StreamWriter sw;
        //static Boolean connected = false;
        //static Boolean updatemsg = false;
        //static Login LoginPage;
        //
        //
        //public static void StartConnect(Login LoginPage1)
        //{
        //    LoginPage = LoginPage1;
        //    Thread thread = new Thread(new ThreadStart(SocketClientConnect));
        //    thread.IsBackground = true;
        //    thread.SetApartmentState(ApartmentState.STA);
        //    thread.Start();
        //}
        //
        //public static void SocketClientConnect()
        //{
        //    String ServerIP = Properties.Settings.Default.ServerIP;
        //    String ServerPort = Properties.Settings.Default.ServerPort;
        //    IPAddress ipAddress = IPAddress.Parse(ServerIP);
        //    int port = int.Parse(ServerPort);
        //    IPEndPoint ipe = new IPEndPoint(ipAddress, port);//IPAddress.Parse("192.168.0.136")
        //    SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    Boolean SocketConnected = false;
        //    try
        //    {
        //        SocketClient.Connect(ipe);
        //        SocketConnected = true;
        //        SocketClientConnect_Finally();
        //    }
        //    catch (SocketException e1)
        //    {
        //        Console.WriteLine("Connect error!!" + e1);
        //    }
        //    catch (Exception e1)
        //    {
        //        Console.WriteLine("Connect error!!" + e1);
        //        ServerIP = Properties.Settings.Default.ServerIP;
        //        ServerPort = Properties.Settings.Default.ServerPort;
        //    }
        //    catch
        //    {
        //
        //    }
        //    finally
        //    {
        //        if (!SocketConnected)
        //        {
        //            if (updatemsg)
        //            {
        //                LoginPage.setConnectFailMsg();
        //            }
        //            ChangeConfigure();
        //        }
        //    }
        //}
        //private static void SocketClientConnect_Finally()
        //{
        //    bool blockingState = SocketClient.Blocking;
        //    try
        //    {
        //        byte[] tmp = new byte[1];
        //        SocketClient.Blocking = false;
        //        SocketClient.Send(tmp, 0, 0);
        //        Console.WriteLine("Connected!");
        //
        //        Properties.Settings.Default.Save();
        //
        //        CommuForm commuForm = new CommuForm();
        //        commuForm.Command = "SQL_W";
        //        commuForm.Action = "PCConnect";
        //        SocketClient.Blocking = blockingState;
        //        ns = new NetworkStream(SocketClient);
        //        sw = new StreamWriter(ns);
        //        sw.AutoFlush = true;
        //
        //        string json = JsonConvert.SerializeObject(commuForm);
        //        sw.WriteLine(json);
        //        //this.DialogResult = DialogResult.OK;
        //        LoginPage.getSocket(SocketClient);
        //    }
        //    catch (SocketException _e)
        //    {
        //        // 10035 == WSAEWOULDBLOCK
        //        if (_e.NativeErrorCode.Equals(10035))
        //        {
        //            Console.WriteLine("Still Connected, but the Send would block");
        //        }
        //        else
        //        {
        //            Console.WriteLine("Disconnected: error code {0}!", _e.NativeErrorCode);
        //        }
        //    }
        //    finally
        //    {
        //        SocketClient.Blocking = blockingState;
        //    }
        //    Console.WriteLine("Connected: {0}", SocketClient.Connected);
        //}
        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool SetForegroundWindow(IntPtr hWnd);
        //private static void ChangeConfigure()
        //{
        //    Configure configure = new Configure();
        //    configure.ShowDialog();
        //    SetForegroundWindow(configure.Handle);
        //    if (configure.DialogResult == DialogResult.OK)
        //    {
        //        if (updatemsg)
        //        {
        //            LoginPage.setReconnectMsg();
        //        }
        //        SocketClientConnect();
        //    }
        //    else
        //    {
        //        Environment.Exit(0);
        //        //this.DialogResult = DialogResult.No;
        //    }
        //}
        //public static Socket getSocketClient()
        //{
        //    return SocketClient;
        //}
        //
        //public static void SetMsgLabel(Login LoginPage1)
        //{
        //    updatemsg = true;
        //    LoginPage = LoginPage1;
        //}
        //public bool IsSocketConnected()
        //{
        //    bool connectState = true;
        //    bool blockingState = SocketClient.Blocking;
        //    try
        //    {
        //        byte[] tmp = new byte[1];
        //        SocketClient.Blocking = false;
        //        SocketClient.Send(tmp, 0, 0);
        //        connectState = true;
        //    }
        //    catch (SocketException e)
        //    {
        //        if (e.NativeErrorCode.Equals(10035))
        //        {
        //            connectState = true;
        //        }
        //        else
        //        {
        //            connectState = false;
        //        }
        //    }
        //    finally
        //    {
        //        SocketClient.Blocking = blockingState;
        //    }
        //    return connectState;            
        //}
    }
}
