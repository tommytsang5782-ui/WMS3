using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WMSClient.Class;
using WMSClient.Utils;
using static WMSClient.Class.SocketConnect;

namespace WMSClient
{
    public partial class CardPage : Form
    {
        private readonly SocketConnect _socketConnect;
        private readonly PageConfig _pageConfig;
        private readonly Dictionary<string, object> _seedData;
        /// <summary>新增模式；設計器建構式未傳入時預設為 true。</summary>
        private bool _isNewRecord = true;
        private string _currentUserId = string.Empty;
        private readonly Dictionary<string, Control> _inputMap = new Dictionary<string, Control>();

        public CardPage()
        {
            InitializeComponent();
            WireEscapeToCancel();
        }

        public CardPage(SocketConnect socketConnect, PageConfig pageConfig, Dictionary<string, object> seedData, bool isNewRecord, string currentUserId = null)
        {
            InitializeComponent();
            WireEscapeToCancel();
            _socketConnect = socketConnect;
            _pageConfig = pageConfig;
            _seedData = seedData ?? new Dictionary<string, object>();
            _isNewRecord = isNewRecord;
            _currentUserId = currentUserId ?? string.Empty;
            Text = (_pageConfig?.TableName ?? "Card") + " Card";
        }

        private static bool IsAuditFieldName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return name.Equals("CreateUser", StringComparison.OrdinalIgnoreCase)
                || name.Equals("CreationDate", StringComparison.OrdinalIgnoreCase)
                || name.Equals("LastModifyUser", StringComparison.OrdinalIgnoreCase)
                || name.Equals("LastModifyDate", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsKeyFieldName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            foreach (var key in _pageConfig?.KeyFields ?? new List<string>())
            {
                if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>Insert：四個審計欄位皆為目前使用者與時間；Update：只更新最後修改者與時間，且不送建立欄位以免覆寫。</summary>
        private void ApplyAuditToPayload(JObject payload, bool isInsert)
        {
            if (payload == null) return;
            var user = _currentUserId ?? string.Empty;
            var now = DateTime.Now;
            if (isInsert)
            {
                payload["CreateUser"] = user;
                payload["CreationDate"] = new JValue(now);
                payload["LastModifyUser"] = user;
                payload["LastModifyDate"] = new JValue(now);
            }
            else
            {
                payload.Remove("CreateUser");
                payload.Remove("CreationDate");
                payload["LastModifyUser"] = user;
                payload["LastModifyDate"] = new JValue(now);
            }
        }

        private void WireEscapeToCancel()
        {
            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Escape) return;
                e.Handled = true;
                DialogResult = DialogResult.Cancel;
                Close();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_pageConfig == null) return;
            BuildCardPage(_pageConfig.CardPage);
        }

        private void BuildCardPage(CardPageConfig cardPage)
        {
            panel1.Controls.Clear();
            footerPanel.Controls.Clear();
            _inputMap.Clear();
            titleLabel.Text = (_pageConfig?.TableName ?? "Card") + " Details";
            int top = 18;
            panel1.AutoScroll = true;

            foreach (var field in cardPage.Fields ?? new List<FieldInfo>())
            {
                var lbl = new Label();
                lbl.Text = string.IsNullOrWhiteSpace(field.Label) ? field.Field : field.Label;
                lbl.Top = top;
                lbl.Left = 20;
                lbl.Width = 180;
                lbl.Font = new System.Drawing.Font("Segoe UI", 9F);
                panel1.Controls.Add(lbl);

                var txt = new TextBox();
                txt.Name = field.Field;
                txt.Top = top;
                txt.Left = 210;
                txt.Width = 260;
                txt.Font = new System.Drawing.Font("Segoe UI", 9F);
                txt.ReadOnly = field.ReadOnly || IsAuditFieldName(field.Field) || (!_isNewRecord && IsKeyFieldName(field.Field));
                if (_seedData.TryGetValue(field.Field, out var value) && value != null)
                {
                    txt.Text = value.ToString();
                }
                if (_isNewRecord && IsAuditFieldName(field.Field))
                {
                    if (field.Type != null && field.Type.IndexOf("DateTime", StringComparison.OrdinalIgnoreCase) >= 0)
                        txt.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    else
                        txt.Text = _currentUserId ?? string.Empty;
                }
                panel1.Controls.Add(txt);
                _inputMap[field.Field] = txt;

                top += 35;
            }

            var save = new Button
            {
                Text = "Save",
                Width = 110,
                Height = 36,
                Left = footerPanel.Width - 240,
                Top = 14,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold)
            };
            save.Click += (s, e) => SaveAndClose();
            footerPanel.Controls.Add(save);

            var cancel = new Button
            {
                Text = "Cancel",
                Width = 110,
                Height = 36,
                Left = footerPanel.Width - 120,
                Top = 14,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Font = new System.Drawing.Font("Segoe UI", 9F)
            };
            cancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            footerPanel.Controls.Add(cancel);
        }

        /// <summary>依 metadata 型別寫入 JSON，避免空字串塞進 DateTime/數值導致伺服器反序列化失敗。</summary>
        private static void AppendFieldToPayload(JObject payload, FieldInfo meta, string rawText)
        {
            if (meta == null || string.IsNullOrEmpty(meta.Field)) return;

            string t = string.IsNullOrWhiteSpace(meta.Type) ? "String" : meta.Type.Trim();
            string text = rawText?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(text))
            {
                if (t.IndexOf("DateTime", StringComparison.OrdinalIgnoreCase) >= 0)
                    return;
                if (t.Equals("Boolean", StringComparison.OrdinalIgnoreCase) || t.Equals("Bool", StringComparison.OrdinalIgnoreCase))
                {
                    payload[meta.Field] = false;
                    return;
                }
                if (t.IndexOf("Byte", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    payload[meta.Field] = 0;
                    return;
                }
                if (t.IndexOf("Int", StringComparison.OrdinalIgnoreCase) >= 0 || t.IndexOf("Long", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    payload[meta.Field] = 0;
                    return;
                }
                if (t.IndexOf("Decimal", StringComparison.OrdinalIgnoreCase) >= 0
                    || t.IndexOf("Double", StringComparison.OrdinalIgnoreCase) >= 0
                    || t.IndexOf("Single", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    payload[meta.Field] = 0m;
                    return;
                }
                payload[meta.Field] = string.Empty;
                return;
            }

            if (t.IndexOf("Byte", StringComparison.OrdinalIgnoreCase) >= 0
                && byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bv))
            {
                payload[meta.Field] = bv;
                return;
            }
            if ((t.IndexOf("Int", StringComparison.OrdinalIgnoreCase) >= 0 || t.IndexOf("Long", StringComparison.OrdinalIgnoreCase) >= 0)
                && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var iv))
            {
                payload[meta.Field] = iv;
                return;
            }
            if (t.IndexOf("Decimal", StringComparison.OrdinalIgnoreCase) >= 0
                && decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
            {
                payload[meta.Field] = dec;
                return;
            }
            if (t.IndexOf("Double", StringComparison.OrdinalIgnoreCase) >= 0
                && double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var dbl))
            {
                payload[meta.Field] = dbl;
                return;
            }
            if (t.IndexOf("DateTime", StringComparison.OrdinalIgnoreCase) >= 0
                && DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dt))
            {
                payload[meta.Field] = dt;
                return;
            }
            if (t.Equals("Boolean", StringComparison.OrdinalIgnoreCase) || t.Equals("Bool", StringComparison.OrdinalIgnoreCase))
            {
                if (bool.TryParse(text, out var bo))
                {
                    payload[meta.Field] = bo;
                    return;
                }
            }

            payload[meta.Field] = text;
        }

        private JObject BuildPayloadFromForm()
        {
            var payload = new JObject();
            foreach (var field in _pageConfig.CardPage.Fields ?? new List<FieldInfo>())
            {
                bool isLockedField = field.ReadOnly || IsAuditFieldName(field.Field) || (!_isNewRecord && IsKeyFieldName(field.Field));

                // 先以 seed 建完整 entity：有值送原值，無值送 null（不省略欄位）
                if (_seedData.TryGetValue(field.Field, out var seedValue))
                    payload[field.Field] = JToken.FromObject(CommonUtils.CellValueForJsonPreserveNull(seedValue));
                else
                    payload[field.Field] = JValue.CreateNull();

                // 只覆蓋可編輯欄位；不可編輯欄位保留 seed 原值
                if (!isLockedField && _inputMap.TryGetValue(field.Field, out Control ctl))
                {
                    AppendFieldToPayload(payload, field, ctl.Text);
                }
            }
            return payload;
        }

        private void SaveAndClose()
        {
            // 存檔瞬間刷新審計欄位：New 四欄皆當下；Edit 只刷新最後修改兩欄（勿改建立者/建立日）
            foreach (var kv in _inputMap)
            {
                if (!IsAuditFieldName(kv.Key) || !(kv.Value is TextBox tb)) continue;
                if (!_isNewRecord)
                {
                    if (!kv.Key.Equals("LastModifyUser", StringComparison.OrdinalIgnoreCase)
                        && !kv.Key.Equals("LastModifyDate", StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                if (kv.Key.IndexOf("Date", StringComparison.OrdinalIgnoreCase) >= 0)
                    tb.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                else if (kv.Key.IndexOf("User", StringComparison.OrdinalIgnoreCase) >= 0)
                    tb.Text = _currentUserId ?? string.Empty;
            }

            var payload = BuildPayloadFromForm();
            ApplyAuditToPayload(payload, _isNewRecord);

            if (!_isNewRecord)
            {
                var keyObj = new JObject();
                foreach (string key in _pageConfig.KeyFields ?? new List<string>())
                {
                    if (_seedData.TryGetValue(key, out var keyValue))
                    {
                        keyObj[key] = JToken.FromObject(CommonUtils.CellValueForJson(keyValue));
                    }
                    else if (_inputMap.TryGetValue(key, out Control keyCtl))
                    {
                        // fallback：metadata key 在 seed 缺失時，直接用畫面 key 欄位值組 condition
                        keyObj[key] = JToken.FromObject(CommonUtils.CellValueForJson(keyCtl.Text));
                    }
                }  

                if (!keyObj.HasValues)
                {
                    CommonUtils.ShowMessage("Update failed: missing key values in condition object.", Text, MessageBoxIcon.Warning);
                    return;
                }

                // 禁止改主鍵：以 condition 的 key 值覆蓋 payload 內 key 欄位
                foreach (var p in keyObj.Properties())
                {
                    payload[p.Name] = p.Value;
                }

                var updateList = new JArray { keyObj, payload };
                var updateJson = updateList.ToString(Formatting.None);
                if (!updateJson.TrimStart().StartsWith("["))
                {
                    CommonUtils.ShowMessage("Internal error: Update payload must be a JSON array.", Text, MessageBoxIcon.Error);
                    return;
                }
                var updateResp = _socketConnect.SendSqlRaw(_pageConfig.TableName, SQLOption.Update, updateJson);
                if (!CommonUtils.TryAssertSqlCommandOk(updateResp, Text)) return;
            }
            else
            {
                var insertJson = payload.ToString(Formatting.None);
                var insertResp = _socketConnect.SendSqlRaw(_pageConfig.TableName, SQLOption.Insert, insertJson);
                if (!CommonUtils.TryAssertSqlCommandOk(insertResp, Text)) return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
