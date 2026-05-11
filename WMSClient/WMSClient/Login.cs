using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using WMSClient.Utils;
using static WMSClient.Class.SocketConnect;

namespace WMSClient
{
    public partial class Login : BaseBusinessForm
    {
        private List<User> _dataList;
        private string _userId;

        /// <summary>Login form constructor (receives SocketConnect from base).</summary>
        public Login(SocketConnect socketConnect)
            : base(socketConnect)
        {
            InitializeComponent();
            Load += Login_Load;
            AcceptButton = button1;
        }

        /// <summary>Form load: load user list automatically.</summary>
        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                LoadUserList();
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage($"Failed to load user list: {ex.Message}", "Error", MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>Parse user list response: server returns JSON array on success, or object {"Code","Msg"} on error.</summary>
        private List<User> ParseUserListResponse(string response)
        {
            try
            {
                JToken token = JToken.Parse(response);
                if (token is JArray arr)
                {
                    return arr.ToObject<List<User>>() ?? new List<User>();
                }
                if (token is JObject obj)
                {
                    string msg = obj["Msg"]?.Value<string>() ?? "Unknown error";
                    CommonUtils.ShowMessage($"Server error: {msg}", "Error", MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (JsonException ex)
            {
                CommonUtils.ShowMessage($"Failed to parse user list: {ex.Message}", "Error", MessageBoxIcon.Error);
                return null;
            }
            return new List<User>();
        }

        /// <summary>Load user list (uses base _socketConnect).</summary>
        private void LoadUserList()
        {
            listBox1.Items.Clear();

            User userQuery = new User();
            //List<User> userListQuery = new List<User>{ userQuery };
            //string jsonQuery = JsonConvert.SerializeObject(userListQuery);

            string response = SendSocketMessage(SQLOption.Select, userQuery);

            if (string.IsNullOrWhiteSpace(response))
            {
                CommonUtils.ShowMessage("Server returned no data.", "Info", MessageBoxIcon.Warning);
                _dataList = new List<User>();
                return;
            }

            _dataList = ParseUserListResponse(response);
            if (_dataList == null)
                return;

            if (_dataList.Count == 0)
            {
                CommonUtils.ShowMessage("No users found.", "Info", MessageBoxIcon.Warning);
                return;
            }

            foreach (User user in _dataList)
            {
                if (!string.IsNullOrEmpty(user.UserID))
                {
                    listBox1.Items.Add(user.UserID);
                }
            }

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        /// <summary>Login button: validate selection and close with OK.</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                CommonUtils.ShowMessage("Please select a user to log in.", "Info", MessageBoxIcon.Warning);
                return;
            }

            _userId = listBox1.SelectedItem.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>Returns the logged-in user ID.</summary>
        public string getUserID()
        {
            return _userId;
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            _dataList?.Clear();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                CommonUtils.ShowMessage("Please select a user to log in.", "Info", MessageBoxIcon.Warning);
                return;
            }

            _userId = listBox1.SelectedItem.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Login page should not close by ESC.
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}