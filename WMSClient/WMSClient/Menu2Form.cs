using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using WMSClient.ClosedPrescanfolder;
using WMSClient.CustomerGroupfolder;
using WMSClient.Itemfolder;
using WMSClient.Labelfolder;
using WMSClient.PackingListfolder;
using WMSClient.Prescan_;
using WMSClient.Printerfolder;
using WMSClient.ScannedPackingListfolder;
using WMSClient.ScanLabelString_;
using WMSClient.Properties;
using WMSClient.Utils;

namespace WMSClient
{
    /// <summary>Menu form: same actions as Main. Order: Default, By user group, or Custom (user-defined).</summary>
    public partial class Menu2Form : BaseBusinessForm
    {
        private readonly string _userID;
        private const float MenuItemFontSize = 12f;
        private static readonly string[] DefaultOrder = {
            "User", "Mapping", "Label", "Scan Label String", "Prescan", "Packing List",
            "Scanned Packing List", "Closed Prescan", "Customer Group", "Printer", "Item",
            "Packing Mapping", "Scanned Packing Mapping", "Company", "OData Setup", "Setup"
        };

        public Menu2Form(SocketConnect socketConnect, string userID)
            : base(socketConnect)
        {
            _userID = userID ?? "";
            InitializeComponent();
        }

        private void Menu2Form_Load(object sender, EventArgs e)
        {
            comboOrder.Items.Clear();
            comboOrder.Items.Add("Default");
            comboOrder.Items.Add("By user group");
            comboOrder.Items.Add("Custom");
            comboOrder.SelectedIndex = 0;
            comboOrder.SelectedIndexChanged += (s, ev) => BuildMenuItems();
            btnSaveOrder.Click += (s, ev) => SaveCurrentOrderAsCustom();
            BuildMenuItems();
            this.Text = "Menu2";
        }

        private string[] GetOrderedItems()
        {
            if (comboOrder.SelectedIndex == 2 && !string.IsNullOrWhiteSpace(Settings.Default.Menu2CustomOrder))
            {
                var custom = Settings.Default.Menu2CustomOrder.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
                var missing = DefaultOrder.Except(custom).ToArray();
                return custom.Concat(missing).ToArray();
            }
            return (string[])DefaultOrder.Clone();
        }

        private void BuildMenuItems()
        {
            panelMenu.Controls.Clear();
            string[] items = GetOrderedItems();
            foreach (string name in items)
            {
                var btn = new Button
                {
                    Text = name,
                    Size = new Size(280, 40),
                    Margin = new Padding(0, 3, 0, 3),
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    Font = new Font("Segoe UI", MenuItemFontSize),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 248, 255);
                btn.Click += (s, ev) => OpenMenuAction(((Button)s).Text);
                panelMenu.Controls.Add(btn);
            }
        }

        private void SaveCurrentOrderAsCustom()
        {
            var order = panelMenu.Controls.OfType<Button>().Select(b => b.Text).ToArray();
            if (order.Length == 0) return;
            Settings.Default.Menu2CustomOrder = string.Join(",", order);
            Settings.Default.Save();
            comboOrder.SelectedIndex = 2;
            CommonUtils.ShowMessage("Order saved as custom.", "Menu2", MessageBoxIcon.Information);
        }

        private void OpenMenuAction(string name)
        {
            Form f = null;
            switch (name)
            {
                case "User":
                    f = new UserList(_socketConnect, _userID);
                    break;
                case "Mapping":
                    f = new MappingList(_socketConnect, _userID);
                    break;
                case "Label":
                    f = new LabelList(_socketConnect);
                    break;
                case "Scan Label String":
                    f = new ScanLabelStringList(_socketConnect);
                    break;
                case "Prescan":
                    f = new PrescanList(_socketConnect, _userID);
                    break;
                case "Packing List":
                    using (var packingList = new PackingList.Packing_List(_socketConnect))
                        packingList.ShowDialog();
                    return;
                case "Scanned Packing List":
                    f = new ScannedPackingList(_socketConnect, _userID);
                    break;
                case "Closed Prescan":
                    f = new ClosedPrescanList(_socketConnect, _userID);
                    break;
                case "Customer Group":
                    f = new CustomerGroupList(_socketConnect, _userID);
                    break;
                case "Printer":
                    f = new PrinterList(_socketConnect, _userID);
                    break;
                case "Item":
                    f = new ItemList(_socketConnect, _userID);
                    break;
                case "Packing Mapping":
                    f = new PackingMappingList(_socketConnect, _userID);
                    break;
                case "Scanned Packing Mapping":
                    f = new ScannedPackingMappingList(_socketConnect, _userID);
                    break;
                case "Company":
                    f = new CompanyList(_socketConnect, _userID);
                    break;
                case "OData Setup":
                    f = new ODataSetupPage(_socketConnect, _userID);
                    break;
                case "Setup":
                    f = new SetupPage(_socketConnect);
                    break;
                default:
                    return;
            }
            if (f != null)
                f.Show();
        }
    }
}
