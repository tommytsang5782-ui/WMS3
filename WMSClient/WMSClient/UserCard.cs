using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static WMSClient.Class.SocketConnect;


namespace WMSClient
{
    public partial class UserCard : BaseBusinessForm
    {
        String LoginUserID, UserID, Password, _editUserID;
        Boolean newuser = false;
        List<User> dataList = new List<User>();
        BindingSource bindingSource = new BindingSource();
        User _user = new User();


        public UserCard(SocketConnect socketConnect, String userID, String editUserID, Boolean new_user)
            : base(socketConnect)
        {
            InitializeComponent();

            //該頁的特別設置
            this.Text = UserID + "  - User Card";
            LoginUserID = userID;
            newuser = new_user;


            //載入數據
            if (!newuser)
            {
                _editUserID = editUserID;
                LoadData();
            }
        }

        private void LoadData()
        {
            User user = new User();
            try
            {
                user.UserID = _editUserID;
                var queryList = new List<User> { user };
                String a = SendSocketMessage(SQLOption.Select,queryList);
                dataList = JsonConvert.DeserializeObject<List<User>>(a);
                if (dataList.Count > 0)
                {
                    User userA = dataList.First();
                    _user = dataList.First();
                    UserID = userA.UserID;
                    Password = userA.Password;
                    UserID_UserCard.Text = UserID;
                    Password_UserCard.Text = Password;
                }
                else
                {
                    MessageBox.Show("未找到该用户的信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("用户信息加载失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        //protected override bool ProcessDialogKey(Keys keyData)
        //{
        //    //按鈕"ESC"關閉頁面
        //    if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
        //    {
        //        this.Close();
        //        return true;
        //    }
        //    return base.ProcessDialogKey(keyData);
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            // 基础校验：用户ID和密码不能为空
            if (string.IsNullOrEmpty(UserID_UserCard.Text.Trim()) || string.IsNullOrEmpty(Password_UserCard.Text.Trim()))
            {
                MessageBox.Show("用户ID和密码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newUserId = UserID_UserCard.Text.Trim();
            string newPwd = Password_UserCard.Text.Trim();

            // 无修改则直接关闭
            if (!newuser && newUserId == UserID && newPwd == Password)
            {
                this.Close();
                return;
            }

            User user = new User();
            user.UserID = newUserId;
            user.Password = newPwd;
            user.LastModifyUser = LoginUserID;
            user.LastModifyDate = DateTime.Now;

            try
            {
                if (newuser)
                {
                    // 新增前校验用户是否存在
                    user.CreateUser = LoginUserID;
                    user.CreationDate = DateTime.Now;
                    var checkQuery = new List<User> { user };
                    String checkResult = SendSocketMessage(SQLOption.Select, checkQuery);
                    var checkList = JsonConvert.DeserializeObject<List<User>>(checkResult);
                    if (checkList.Count > 0)
                    {
                        MessageBox.Show(checkList[0].UserID);
                        MessageBox.Show(checkList[1].UserID);
                        MessageBox.Show("User " + newUserId + " already exists.");
                        return;
                    }
                    // 执行新增
                    var insertList = new List<User> { user };
                    SendSocketMessage(SQLOption.Insert, insertList);
                    MessageBox.Show("用户新增成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // 编辑：保留原始创建信息（无需赋值空值，服务端应做保留处理）
                    List<User> userList = new List<User>() { _user, user };
                    SendSocketMessage(SQLOption.Update,userList);
                    MessageBox.Show("用户编辑成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                string tip = newuser ? "新增" : "编辑";
                MessageBox.Show("用户" + tip + "失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //if ((UserID_UserCard.Text != UserID) || (Password_UserCard.Text != Password))
            //{
            //    User user = new User();
            //    List<User> userList = new List<User>();
            //    String createuser = "";
            //    String a = "";
            //    DateTime creationdate = DateTime.MinValue;

            //    user.UserID = UserID_UserCard.Text;
            //    user.Password = Password_UserCard.Text;

            //    if (newuser)
            //    {
            //        user.CreateUser = LoginUserID;
            //        user.CreationDate = DateTime.Now;
            //    }
            //    else
            //    {
            //        user.CreateUser = createuser;
            //        user.CreationDate = creationdate;
            //    }
            //    user.LastModifyUser = LoginUserID;
            //    user.LastModifyDate = DateTime.Now;
            //    userList.Add(user);

            //    if (newuser)
            //    {
            //        a = SendSocketMessage("Select", "User", JsonConvert.SerializeObject(user));
            //        dataList = JsonConvert.DeserializeObject<List<User>>(a);
            //        if (dataList.Count > 0)
            //        {
            //            MessageBox.Show("User " + UserID_UserCard.Text+ " already exists.");
            //            return;
            //        }
                            
            //        a = SendSocketMessage("Insert", "User", JsonConvert.SerializeObject(user));
            //    }
            //    else
            //    {
            //        a = SendSocketMessage("Update", "User", JsonConvert.SerializeObject(userList));
            //    }
            //}
            //this.Close();
        }
    }
}
