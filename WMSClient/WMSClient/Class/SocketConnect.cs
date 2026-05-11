using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows;
using WMSClient.Properties; // 引入設定檔

namespace WMSClient.Class
{
    /// <summary>
    /// Socket用戶端連接類（執行緒安全，自動釋放資源）
    /// </summary>
    public class SocketConnect : IDisposable
    {
        // 私有成員變數，禁止外部直接修改
        private Socket _socket;
        private NetworkStream _networkStream;
        private StreamWriter _streamWriter;

        // 配置項（唯讀，一次載入）
        private readonly string _serverIp;
        private readonly int _serverPort;

        // 預設超時時間（可根據業務調整）
        private const int DefaultTimeout = 10000; // 10秒
        // 某些環境首包前會混入 2 bytes，允許一次性重對齊（避免每次都硬丟）
        private bool _hasRealignedInitialLengthPrefix = false;

        /// <summary>
        /// 構造函數：載入配置項
        /// </summary>
        public SocketConnect()
        {
            // 一次性載入配置，避免重複讀取
            //_serverIp = Properties.Settings.Default.ServerIP;
            //_serverPort = int.Parse(Properties.Settings.Default.ServerPort);
        }

        public enum SQLOption
        {
            // 显式指定数值（也可以省略，默认从0开始递增）
            Select = 1,
            Insert = 2,
            Update = 3,
            Delete = 4,
            Get = 5,
            Set = 6
        }


        /// <summary>
        /// 連接伺服器（改進版）
        /// </summary>
        /// <returns>是否連接成功</returns>
        public bool ConnectToServer()
        {
            // 先關閉已有連接
            Disconnect();
            // 從設定檔讀取IP和埠
            string _serverIp = Settings.Default.ServerIP ?? "127.0.0.1";
            if (!int.TryParse(Settings.Default.ServerPort, out int _serverPort))
            {
                _serverPort = 8888; // 默認埠
            }

            try
            {
                

                IPAddress ipAddress = IPAddress.Parse(_serverIp);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, _serverPort);

                // 創建Socket並設置基礎屬性
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = DefaultTimeout,
                    SendTimeout = DefaultTimeout
                };

                // 使用 BeginConnect + WaitOne 實現帶超時的連接（.NET Framework 4.7.2 相容）
                IAsyncResult result = _socket.BeginConnect(endPoint, null, null);
                bool connectSuccess = result.AsyncWaitHandle.WaitOne(DefaultTimeout);

                if (!connectSuccess || !_socket.Connected)
                {
                    Disconnect();
                    Console.WriteLine("連接逾時或伺服器拒絕連接");
                    return false;
                }

                try
                {
                    _socket.EndConnect(result);
                }
                catch (SocketException ex)
                {
                    Disconnect();
                    Console.WriteLine($"完成連接時發生錯誤：{ex.Message} (錯誤碼：{ex.NativeErrorCode})");
                    return false;
                }

                // 勿在連線建立後依 _socket.Available 清空接收緩衝：
                // TCP 可能已送達回應的前幾個位元組（常見為 4-byte 長度前綴的前 2 bytes），
                // 若整段讀走會破壞「長度 + UTF-16BE JSON」框架，導致首包（如 Login / User 表）解析失敗。
                //try
                //{
                //    if (_networkStream.DataAvailable)
                //    {
                //        int strlen = (int)_networkStream.Length;
                //        byte[] tempBuffer = new byte[strlen];

                //        int n = _networkStream.Read(tempBuffer, 0, strlen);
                //        _socket.Receive(tempBuffer);
                //        Console.WriteLine($"清空连接初期残留数据 22：{BitConverter.ToString(tempBuffer)}");

                //    }
                //}
                //catch (SocketException ex)
                //{
                //}


                InitStream();
                _hasRealignedInitialLengthPrefix = false;

                Console.WriteLine("成功連接到伺服器：{0}:{1}", _serverIp, _serverPort);

                //// 發送連接通知
                //if (_socket.Connected)
                //{
                //   SendMessage("PCConnect", "", "");
                //}
                return true;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"IP/埠格式錯誤：{ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket連接錯誤：{ex.Message} (錯誤碼：{ex.NativeErrorCode})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"連接失敗：{ex.Message}");
            }

            Disconnect();
            return false;
        }

        /// <summary>
        /// 初始化網路流（改進版）
        /// </summary>
        private void InitStream()
        {
            if (_socket == null || !_socket.Connected)
                throw new InvalidOperationException("Socket未連接，無法初始化流");

            _networkStream = new NetworkStream(_socket, true); // 關閉流時自動關閉Socket
            _networkStream.ReadTimeout = DefaultTimeout;
            _networkStream.WriteTimeout = DefaultTimeout;

            // 僅用 StreamWriter 發送請求；接收一律用 _networkStream.Read（勿再掛 StreamReader，避免與二進位長度幀混用）
            _streamWriter = new StreamWriter(_networkStream, Encoding.BigEndianUnicode)
            {
                AutoFlush = true // 自動刷新緩衝區
            };
        }

        public void SetSocket(Socket socket)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        /// <summary>
        /// 發送消息並接收響應（核心邏輯重構）
        /// </summary>
        /// <param name="action">操作類型（如Select/PCConnect）</param>
        /// <param name="table">資料表名</param>
        /// <param name="msg">消息內容</param>
        /// <returns>伺服器回應內容</returns>
        public string SendMessage<T>(SQLOption action, T data)
        {
            // 校驗連接狀態
            if (!IsConnected())
            {
                Console.WriteLine("Socket未連接，無法發送消息");
                return string.Empty;
            }

            try
            {
                // 1. 自动获取表名（无需手动传入，可选）
                string tableName = GetTableName<T>();

                // 2. 自动序列化：单个对象/集合都能正确序列化为JSON
                string dataJson = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                });
                // 構建請求對象並序列化為JSON
                var commuForm = action == SQLOption.Get
                    ? new CommuForm
                    {
                        Command = "GetConfig",
                        Table = tableName,
                        Str = "@" + dataJson
                    }
                    : new CommuForm
                    {
                        Command = "SQL",
                        Action = action.ToString(),
                        Table = tableName,
                        Str = "@" + dataJson
                    };
                string jsonRequest = JsonConvert.SerializeObject(commuForm);

                // 發送JSON消息
                _streamWriter.WriteLine(jsonRequest);
                Console.WriteLine($"Send {action} request：Table={tableName}，Data={jsonRequest}");

                return ReceiveLargeData(); // 處理大資料接收
            }
            catch (IOException ex)
            {
                Console.WriteLine($"流操作錯誤：{ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket通信錯誤：{ex.Message}");
                Disconnect(); // 通信異常時主動斷開
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發送/接收消息失敗：{ex.Message}");
            }

            return string.Empty;
        }

        public string SendSqlRaw(string tableName, SQLOption action, string jsonPayload)
        {
            if (!IsConnected())
            {
                Console.WriteLine($"SendSqlRaw skipped: socket not connected (Table={tableName}, Action={action})");
                return string.Empty;
            }

            var commuForm = new CommuForm
            {
                Command = "SQL",
                Action = action.ToString(),
                Table = tableName,
                Str = "@" + (jsonPayload ?? "{}")
            };
            _streamWriter.WriteLine(JsonConvert.SerializeObject(commuForm));
            return ReceiveLargeData();
        }

        public PageConfig GetMetadata(string pageCode, string scope = "Page", string userId = "")
        {
            if (!IsConnected())
            {
                return null;
            }

            var requestPayload = new
            {
                scope = scope,
                userId = userId ?? string.Empty
            };

            var commuForm = new CommuForm
            {
                Command = "GetConfig",
                Table = pageCode,
                Str = "@" + JsonConvert.SerializeObject(requestPayload)
            };

            _streamWriter.WriteLine(JsonConvert.SerializeObject(commuForm));
            string response = ReceiveLargeData();
            if (string.IsNullOrWhiteSpace(response))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<PageConfig>(response);
        }

        /// <summary>
        /// 辅助方法：自动获取表名（支持单个对象/集合）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>表名（如User/Setup/OrderList）</returns>
        private string GetTableName<T>()
        {
            Type type = typeof(T);

            // 处理集合类型（List<User> → User）
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                type = type.GetGenericArguments()[0]; // 获取集合的泛型参数（如User）
            }

            // 返回类型名称作为表名（与你的业务表名保持一致）
            string tableName = type.Name;
            return tableName;
        }

        // 備註：
        // 舊邏輯是「若第一次 Read 不是 4 位元組，就放棄本次並清空緩衝區，再重新讀 4 位元組」，
        // 在協定（伺服器先送 4 位長度、再送 JSON）下會丟掉原本已讀到的前半段長度，
        // 造成把「長度後半 + JSON 開頭」誤判成超大長度（如 8060962）。目前改為「累積讀滿 4 位元組再解析」。
        // 若未來確定需要還原舊行為，可依需求改回「放棄本次、重新讀 4 位元組」模式。
        //private string ReceiveLargeData()
        //{
        //    try
        //    {
        //        // 第一步：累積讀滿 4 位元組長度前綴
        //        byte[] lengthBuffer = new byte[4];
        //        int totalLengthRead = 0;
        //        while (totalLengthRead < 4)
        //        {
        //            int n = _networkStream.Read(lengthBuffer, totalLengthRead, 4 - totalLengthRead);
        //            if (n == 0)
        //            {
        //                Console.WriteLine("讀取長度時連接斷開");
        //                return string.Empty;
        //            }
        //            totalLengthRead += n;
        //        }

        //        // 第二步：解析長度（大端序，與服務端一致）
        //        if (BitConverter.IsLittleEndian)
        //            Array.Reverse(lengthBuffer);
        //        int dataSize = BitConverter.ToInt32(lengthBuffer, 0);

        //        // Update/Insert/Delete 回應應為小 JSON（約數十位元組）；Select 可能較大。異常大長度視為解析錯誤或串流錯位
        //        if (dataSize <= 0 || dataSize > 1024 * 1024 * 10) // 最大 10MB
        //        {
        //            Console.WriteLine($"無效的數據長度：{dataSize}（需 >0 且 ≤10MB）");
        //            return string.Empty;
        //        }

        //        // 第三步：累積讀取 dataSize 位元組的 JSON 資料
        //        byte[] dataBuffer = new byte[dataSize];
        //        int totalDataRead = 0;
        //        while (totalDataRead < dataSize)
        //        {
        //            int n = _networkStream.Read(dataBuffer, totalDataRead, dataSize - totalDataRead);
        //            if (n == 0)
        //            {
        //                Console.WriteLine("讀取實際資料時連接斷開，資料不完整");
        //                return string.Empty;
        //            }
        //            totalDataRead += n;
        //        }
        //        // 轉換為字串並清理空字元，與服務端編碼（BigEndianUnicode）匹配
        //        string response = Encoding.BigEndianUnicode.GetString(dataBuffer, 0, totalDataRead).TrimEnd('\0');
        //        Console.WriteLine($"接收大資料回應成功（總長度：{dataSize}），實際有效字元數：{response.Length}");
        //        return response;
        //    }
        //    catch (IOException ex)
        //    {
        //        Console.WriteLine($"讀取大資料時流操作錯誤：{ex.Message}");
        //    }
        //    catch (SocketException ex)
        //    {
        //        Console.WriteLine($"讀取大資料時Socket錯誤：{ex.Message}（錯誤碼：{ex.NativeErrorCode}）");
        //        Disconnect(); // 通信異常，主動斷開連接
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"讀取大數據失敗：{ex.Message}");
        //    }
        //    return string.Empty;
        //}

        private string ReceiveLargeData()
        {
            try
            {
                // 長度前綴為 BigEndian 32-bit，小於 64K 時前兩個 byte 常為 0x00。
                // 絕不可在讀長度前「跳過 0x00」：會吃掉合法長度的高位元組，串流錯位後
                // 下一個 4-byte 常被誤讀成 FE FF 00 00（UTF-16 BOM 開頭）→ 解析出約 -16842752 等亂值。
                byte[] lengthBuffer = new byte[4];
                int totalLengthRead = 0;
                int currentRead = 0;
                bool isReadSuccess = false;

                while (!isReadSuccess)
                {
                    try
                    {
                        if (totalLengthRead >= 4)
                        {
                            isReadSuccess = true;
                            break;
                        }

                        int need = 4 - totalLengthRead;
                        currentRead = _networkStream.Read(lengthBuffer, totalLengthRead, need);
                        Console.WriteLine($"嘗試讀取長度前綴，本次讀取：{currentRead}字節，累積：{totalLengthRead + Math.Max(currentRead, 0)}字節");

                        if (currentRead == 0)
                        {
                            Console.WriteLine("讀取長度時連接斷開，未讀取到任何位元組");
                            return string.Empty;
                        }
                        totalLengthRead += currentRead;
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"讀取長度時流異常：{ex.Message}，重新嘗試...");
                        continue;
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"讀取長度時Socket異常：{ex.Message}（錯誤碼：{ex.NativeErrorCode}）");
                        Disconnect();
                        return string.Empty;
                    }
                }

                if (totalLengthRead != 4)
                {
                    Console.WriteLine($"無法讀取資料長度，實際讀取：{totalLengthRead}位元組（預期4位元組）");
                    return string.Empty;
                }

                // 第二步：解析資料長度（大端序轉換，與服務端保持一致）
                int dataSize = ParseBigEndianInt32(lengthBuffer);
                Console.WriteLine($"解析出的實際資料長度：{dataSize}位元組");

                // 兼容「首包前混入 2 bytes」：當首包長度異常時，丟掉前2 bytes、補讀2 bytes重組一次
                if ((dataSize <= 0 || dataSize > 1024 * 1024 * 10) && !_hasRealignedInitialLengthPrefix)
                {
                    Console.WriteLine($"首包長度異常，嘗試一次性丟棄前2字節重對齊。原始長度前綴：{BitConverter.ToString(lengthBuffer)}");
                    byte[] extra2 = new byte[2];
                    int readExtra = 0;
                    while (readExtra < 2)
                    {
                        int n = _networkStream.Read(extra2, readExtra, 2 - readExtra);
                        if (n == 0)
                        {
                            Console.WriteLine("重對齊時連接斷開（無法補讀2字節）");
                            return string.Empty;
                        }
                        readExtra += n;
                    }

                    // 丟掉 lengthBuffer[0..1]，保留[2..3]，再接上補讀的2 bytes
                    lengthBuffer[0] = lengthBuffer[2];
                    lengthBuffer[1] = lengthBuffer[3];
                    lengthBuffer[2] = extra2[0];
                    lengthBuffer[3] = extra2[1];

                    dataSize = ParseBigEndianInt32(lengthBuffer);
                    _hasRealignedInitialLengthPrefix = true;
                    Console.WriteLine($"重對齊後長度前綴：{BitConverter.ToString(lengthBuffer)}，解析長度：{dataSize}");
                }

                if (dataSize <= 0 || dataSize > 1024 * 1024 * 10)
                {
                    Console.WriteLine($"無效的數據長度：{dataSize}（需大於0且≤10MB）");
                    return string.Empty;
                }

                // 第三步：讀取實際JSON資料
                byte[] dataBuffer = new byte[dataSize];
                int totalDataRead = 0;
                while (totalDataRead < dataSize)
                {
                    currentRead = _networkStream.Read(dataBuffer, totalDataRead, dataSize - totalDataRead);
                    if (currentRead == 0)
                    {
                        Console.WriteLine("讀取實際資料時連接斷開，資料不完整");
                        return string.Empty;
                    }
                    totalDataRead += currentRead;
                }

                string response = Encoding.BigEndianUnicode.GetString(dataBuffer, 0, totalDataRead).TrimEnd('\0');
                // UTF-16 BE 位元組順序 FE FF 解碼為 U+FEFF（BOM）；若出現在 JSON 前會導致反序列化失敗
                if (response.Length > 0 && response[0] == '\uFEFF')
                    response = response.TrimStart('\uFEFF');
                Console.WriteLine($"接收大資料回應成功（總長度：{dataSize}），實際有效字元數：{response.Length}");
                return response;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"讀取大資料時流操作錯誤：{ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"讀取大資料時Socket錯誤：{ex.Message}（錯誤碼：{ex.NativeErrorCode}）");
                Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取大數據失敗：{ex.Message}");
            }
            return string.Empty;
        }

        private static int ParseBigEndianInt32(byte[] source4)
        {
            if (source4 == null || source4.Length != 4)
            {
                throw new ArgumentException("source4 must be exactly 4 bytes", nameof(source4));
            }

            if (BitConverter.IsLittleEndian)
            {
                byte[] reversed = (byte[])source4.Clone();
                Array.Reverse(reversed);
                return BitConverter.ToInt32(reversed, 0);
            }

            return BitConverter.ToInt32(source4, 0);
        }

        //2026.04.10
        //private string ReceiveLargeData()
        //{
        //    try
        //    {
        //        // ========== 第一步：过滤连接初期的脏数据/空字节（核心修复1） ==========
        //        byte[] tempBuffer = new byte[1];
        //        bool hasCleanedDirtyData = false;
        //        while (true)
        //        {
        //            // 检查是否有可读取的数据
        //            if (_networkStream.DataAvailable)
        //            {
        //                int n = _networkStream.Read(tempBuffer, 0, 1);
        //                if (n == 0) break; // 连接断开

        //                // 过滤空字节（0x00），非空字节放回流中
        //                if (tempBuffer[0] != 0x00)
        //                {
        //                    _networkStream.Position -= 1; // 放回流中供后续读取
        //                    if (hasCleanedDirtyData)
        //                    {
        //                        Console.WriteLine("脏数据过滤完成，开始读取有效长度数据");
        //                    }
        //                    break;
        //                }
        //                else
        //                {
        //                    Console.WriteLine($"过滤无效空字节：0x{tempBuffer[0]:X2}");
        //                    hasCleanedDirtyData = true;
        //                }
        //            }
        //            else
        //            {
        //                break; // 无数据可读取，退出过滤
        //            }
        //        }

        //        // 第一步：讀取資料長度（4位元組表示長度）- 核心修復：迴圈補讀，確保讀取到4位元組
        //        byte[] lengthBuffer = new byte[4];
        //        int totalLengthRead = 0; // 已讀取的總位元組數
        //        int currentRead = 0;     // 單次讀取的位元組數
        //        bool isReadSuccess = false;
        //        // 迴圈補讀：直到讀取滿4位元組 或 讀取到0位元組（連接斷開）
        //        while (!isReadSuccess)
        //        {
        //            try
        //            {
        //                // 每次讀取前清空緩衝區，避免無效資料殘留
        //                Array.Clear(lengthBuffer, 0, lengthBuffer.Length);
        //                // 核心：單次嘗試讀取完整4位元組（偏移量0，讀取長度4）
        //                currentRead = _networkStream.Read(lengthBuffer, 0, 4);
        //                Console.WriteLine($"嘗試讀取4字節長度，本次讀取：{currentRead}字節");

        //                // 判定1：讀取到0位元組 → 連接斷開，直接返回
        //                if (currentRead == 0)
        //                {
        //                    Console.WriteLine("讀取長度時連接斷開，未讀取到任何位元組");
        //                    return string.Empty;
        //                }
        //                // 判定2：單次讀取≠4位元組 → 放棄本次，列印日誌後繼續迴圈重新讀取
        //                else if (currentRead != 4)
        //                {
        //                    Console.WriteLine($"讀取長度非4字節（{currentRead}字節），放棄本次，重新讀取...");
        //                    continue;
        //                }
        //                // 判定3：單次讀取=4位元組 → 讀取成功，退出迴圈
        //                else
        //                {
        //                    Console.WriteLine("單次成功讀取4位元組有效長度首碼");
        //                    isReadSuccess = true;
        //                    totalLengthRead = 4;
        //                }
        //            }
        //            catch (IOException ex)
        //            {
        //                Console.WriteLine($"讀取長度時流異常：{ex.Message}，重新嘗試...");
        //                continue;
        //            }
        //            catch (SocketException ex)
        //            {
        //                Console.WriteLine($"讀取長度時Socket異常：{ex.Message}（錯誤碼：{ex.NativeErrorCode}）");
        //                Disconnect();
        //                return string.Empty;
        //            }
        //        }
        //        // 驗證：確保累計讀取到4位元組（迴圈結束後必然滿足，做冗餘校驗）
        //        if (totalLengthRead != 4)
        //        {
        //            Console.WriteLine($"無法讀取資料長度，實際讀取：{totalLengthRead}位元組（預期4位元組）");
        //            return string.Empty;
        //        }

        //        // 第二步：解析資料長度（大端序轉換，與服務端保持一致）
        //        // ========== 核心修復：替換原有錯誤的長度解析邏輯（僅修改此段） ==========
        //        int dataSize = 0;
        //        // 服務端發送4位元組大端序，用戶端需根據系統位元組序判斷是否反轉（與服務端嚴格對稱）
        //        if (BitConverter.IsLittleEndian)
        //        {
        //            // 小端序系統（x86/x64，絕大多數伺服器/用戶端）：複製陣列後反轉，避免修改原緩衝區
        //            byte[] reversedLength = (byte[])lengthBuffer.Clone();
        //            Array.Reverse(reversedLength);
        //            dataSize = BitConverter.ToInt32(reversedLength, 0);
        //        }
        //        else
        //        {
        //            // 大端序系統（極少場景）：直接解析
        //            dataSize = BitConverter.ToInt32(lengthBuffer, 0);
        //        }
        //        // ======================================================================
        //        Console.WriteLine($"解析出的實際資料長度：{dataSize}位元組");

        //        // 長度合法性校驗：排除無效長度，防止惡意資料/解析錯誤
        //        if (dataSize <= 0 || dataSize > 1024 * 1024 * 10) // 放寬到10MB，適配大數量資料
        //        {
        //            Console.WriteLine($"無效的數據長度：{dataSize}（需大於0且≤10MB）");
        //            return string.Empty;
        //        }

        //        // 第三步：讀取實際JSON資料 - 同樣使用迴圈補讀，確保讀取到完整的dataSize位元組
        //        byte[] dataBuffer = new byte[dataSize];
        //        int totalDataRead = 0;
        //        while (totalDataRead < dataSize)
        //        {
        //            currentRead = _networkStream.Read(dataBuffer, totalDataRead, dataSize - totalDataRead);
        //            if (currentRead == 0)
        //            {
        //                Console.WriteLine("讀取實際資料時連接斷開，資料不完整");
        //                return string.Empty;
        //            }
        //            totalDataRead += currentRead;
        //        }
        //        // 轉換為字串並清理空字元，與服務端編碼（BigEndianUnicode）匹配
        //        string response = Encoding.BigEndianUnicode.GetString(dataBuffer, 0, totalDataRead).TrimEnd('\0');
        //        Console.WriteLine($"接收大資料回應成功（總長度：{dataSize}），實際有效字元數：{response.Length}");
        //        return response;
        //    }
        //    catch (IOException ex)
        //    {
        //        Console.WriteLine($"讀取大資料時流操作錯誤：{ex.Message}");
        //    }
        //    catch (SocketException ex)
        //    {
        //        Console.WriteLine($"讀取大資料時Socket錯誤：{ex.Message}（錯誤碼：{ex.NativeErrorCode}）");
        //        Disconnect(); // 通信異常，主動斷開連接
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"讀取大數據失敗：{ex.Message}");
        //    }
        //    return string.Empty;
        //}
        /// <summary>
        /// 接收伺服器回應（統一協定：先 4 位元組長度，再資料）
        /// </summary>
        //private string ReceiveLargeData()
        //{
        //    try
        //    {
        //        // 1. 讀取 4 位元組長度（大端序）
        //        byte[] lengthBuffer = new byte[4];
        //        int lengthRead = _networkStream.Read(lengthBuffer, 0, 4);
        //        if (lengthRead != 4)
        //        {
        //            Console.WriteLine("無法讀取資料長度");
        //            return string.Empty;
        //        }
        //        if (BitConverter.IsLittleEndian)
        //            Array.Reverse(lengthBuffer);
        //        int dataSize = BitConverter.ToInt32(lengthBuffer, 0);
        //        if (dataSize <= 0 || dataSize > 4 * 1024 * 1024) // 最大 4MB
        //        {
        //            Console.WriteLine($"無效的數據長度：{dataSize}");
        //            return string.Empty;
        //        }
        //        // 2. 讀取實際資料
        //        byte[] dataBuffer = new byte[dataSize];
        //        int totalRead = 0;
        //        while (totalRead < dataSize)
        //        {
        //            int bytesRead = _networkStream.Read(dataBuffer, totalRead, dataSize - totalRead);
        //            if (bytesRead <= 0) break;
        //            totalRead += bytesRead;
        //        }
        //        if (totalRead != dataSize)
        //        {
        //            Console.WriteLine($"資料不完整：期望 {dataSize}，收到 {totalRead}");
        //            return string.Empty;
        //        }
        //        string response = Encoding.BigEndianUnicode.GetString(dataBuffer, 0, totalRead).TrimEnd('\0');
        //        Console.WriteLine($"接收回應成功，長度：{response.Length}");
        //        return response;
        //    }
        //    catch (IOException ex)
        //    {
        //        Console.WriteLine($"讀取回應時流操作錯誤：{ex.Message}");
        //    }
        //    catch (SocketException ex)
        //    {
        //        Console.WriteLine($"讀取回應時Socket錯誤：{ex.Message}（錯誤碼：{ex.NativeErrorCode}）");
        //        Disconnect();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"讀取回應失敗：{ex.Message}");
        //    }
        //    return string.Empty;
        //}





        //private string ReceiveLargeData()
        //{
        //    // 第一步：讀取資料長度（先接收4位元組表示長度）
        //    byte[] lengthBuffer = new byte[4];
        //    int lengthRead = _networkStream.Read(lengthBuffer, 0, 4);
        //    Console.WriteLine(lengthRead);

        //    if (lengthRead != 4)
        //    {
        //        Console.WriteLine("無法讀取資料長度");
        //        return string.Empty;
        //    }

        //    // 解析資料長度（大端序轉換）
        //    int dataSize = BitConverter.ToInt32(lengthBuffer.Reverse().ToArray(), 0);
        //    if (dataSize <= 0 || dataSize > 1024 * 1024) // 限制最大1MB，防止惡意資料
        //    {
        //        Console.WriteLine($"無效的數據長度：{dataSize}");
        //        return string.Empty;
        //    }

        //    // 第二步：讀取實際資料
        //    byte[] dataBuffer = new byte[dataSize];
        //    int totalRead = 0;
        //    while (totalRead < dataSize)
        //    {
        //        int bytesRead = _networkStream.Read(dataBuffer, totalRead, dataSize - totalRead);
        //        if (bytesRead <= 0) break; // 連接中斷
        //        totalRead += bytesRead;
        //    }

        //    // 轉換為字串並清理空字元
        //    string response = Encoding.BigEndianUnicode.GetString(dataBuffer, 0, totalRead).TrimEnd('\0');
        //    Console.WriteLine($"接收大資料回應（長度：{dataSize}）：{response}");
        //    return response;
        //}
        public void initStream()
        {
            if (_socket != null && _socket.Connected)
            {
                _networkStream = new NetworkStream(_socket);
            }
        }

        /// <summary>
        /// 可靠的連接狀態檢測（改進版）
        /// </summary>
        /// <returns>是否已連接</returns>
        public bool IsConnected()
        {
            // 勿用 Poll+Available 推斷半關閉：在 TCP 仍活著時常誤判為斷線，導致 SendSqlRaw 不送出（Delete 等無反應）
            return _socket != null && _socket.Connected;
        }

        /// <summary>
        /// 斷開連接（釋放資源）
        /// </summary>
        public void Disconnect()
        {
            // 按順序釋放資源
            _streamWriter?.Dispose();
            _networkStream?.Dispose();

            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    try
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                    }
                    catch { } // 忽略關閉時的異常
                }
                _socket.Close();
                _socket = null;
            }

            Console.WriteLine("已斷開與伺服器的連接");
        }

        /// <summary>
        /// 實現IDisposable介面，自動釋放資源
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
        public Socket GetSocket()
        {
            // 只有連接有效時才返回Socket，避免返回無效實例
            return IsConnected() ? _socket : null;
        }

        /// <summary>
        /// 通信格式類（內部使用）
        /// </summary>
        private class CommuForm
        {
            public string Command { get; set; }
            public string Action { get; set; }
            public string Table { get; set; }
            public string Str { get; set; }
        }
        public class ApiResponse
        {
            public int Code { get; set; }
            public string Msg { get; set; }
        }
    }
}
