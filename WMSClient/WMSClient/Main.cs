using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WMSClient.Prescan_;
using WMSClient.ClosedPrescanfolder;
using WMSClient.ScannedPackingListfolder;
using WMSClient.PackingListfolder;
using WMSClient.Class;
using WMSClient.Base;

namespace WMSClient
{
    public partial class Main : BaseBusinessForm
    {
        private readonly string _userID;
        private readonly Dictionary<string, Func<Form>> _customFormRegistry;
        public Main()
        {
            //InitializeComponent();
        }

        public Main(SocketConnect socketConnect, String userID) : 
            base(socketConnect)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(userID))
            {
                throw new ArgumentNullException(nameof(userID), "User ID cannot be empty.");
            }

            _userID = userID;
            listBox1.Items.Add("Welcome " + _userID);
            _customFormRegistry = BuildCustomFormRegistry();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyShellLayoutStyle();
            BuildMetadataMenu();
        }

        private void ApplyShellLayoutStyle()
        {
            Text = "WMS Shell";
            MinimumSize = new System.Drawing.Size(980, 640);
            listBox1.Height = 38;
            listBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            flowLayoutPanel1.Padding = new Padding(8);
            flowLayoutPanel1.WrapContents = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.AutoScroll = true;
        }

        private Dictionary<string, Func<Form>> BuildCustomFormRegistry()
        {
            return new Dictionary<string, Func<Form>>(StringComparer.OrdinalIgnoreCase)
            {
                ["PackingList"] = () => new PackingList.Packing_List(_socketConnect),
                ["PrescanList"] = () => new PrescanList(_socketConnect, _userID),
                ["PackingMappingList"] = () => new PackingMappingList(_socketConnect, _userID),
                ["ScannedPackingList"] = () => new ScannedPackingList(_socketConnect, _userID),
                ["ClosedPrescanList"] = () => new ClosedPrescanList(_socketConnect, _userID),
                ["ODataSetupPage"] = () => new ODataSetupPage(_socketConnect, _userID),
                ["SetupPage"] = () => new SetupPage(_socketConnect)
            };
        }

        private void BuildMetadataMenu()
        {
            var shellConfig = _socketConnect.GetMetadata("Shell", "Shell", _userID);
            if (shellConfig?.Menu == null || shellConfig.Menu.Count == 0)
            {
                return;
            }

            groupBox1.Visible = false;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            btnMenu2.Visible = false;
            flowLayoutPanel1.Controls.Clear();

            foreach (var root in shellConfig.Menu)
            {
                if (root.Children == null || root.Children.Count == 0) continue;
                var group = new GroupBox
                {
                    Text = root.Text,
                    Width = 440,
                    Height = 68 + 50 * ((root.Children.Count + 1) / 2),
                    Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                    Padding = new Padding(12)
                };

                int left = 10;
                int top = 22;
                int count = 0;
                foreach (var item in root.Children)
                {
                    var btn = new Button
                    {
                        Text = item.Text,
                        Tag = item,
                        Width = 200,
                        Height = 40,
                        Left = left,
                        Top = top,
                        Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular)
                    };
                    btn.Click += DynamicMenuClick;
                    group.Controls.Add(btn);

                    count++;
                    if (count % 2 == 0)
                    {
                        left = 10;
                        top += 40;
                    }
                    else
                    {
                        left = 220;
                    }
                }

                flowLayoutPanel1.Controls.Add(group);
            }
        }

        private void DynamicMenuClick(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is MenuItem item)) return;

            if (string.Equals(item.FormKind, "Custom", StringComparison.OrdinalIgnoreCase))
            {
                if (_customFormRegistry.TryGetValue(item.FormName, out var factory))
                {
                    factory().Show();
                }
                return;
            }

            if (!string.IsNullOrWhiteSpace(item.FormName))
            {
                new ListPage(_socketConnect, item.FormName, _userID).Show();
            }
        }

        private void button1_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "User", _userID).Show();
        private void button2_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "Mapping", _userID).Show();

        private void button5_Click(object sender, EventArgs e) => DynamicOpenCustom("PrescanList");

        private void button3_Click(object sender, EventArgs e)
        {
            using (var packingList = new PackingList.Packing_List(_socketConnect))
            {
                DialogResult result = packingList.ShowDialog();
                // 自动释放资源，无需手动置空
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e) => DynamicOpenCustom("ScannedPackingList");

        private void button8_Click(object sender, EventArgs e) => DynamicOpenCustom("ClosedPrescanList");
        private void button6_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "ScanLabelString", _userID).Show();
        private void button7_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "LabelHeader", _userID).Show();

        private void button10_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "Item", _userID).Show();

        private void button11_Click(object sender, EventArgs e)
        {
        }

        private void button12_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "CustomerGroup", _userID).Show();

        private void button4_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "Printer", _userID).Show();

        private void button13_Click(object sender, EventArgs e) => DynamicOpenCustom("PackingMappingList");

        private void button14_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "ScannedPackingMapping", _userID).Show();

        private void button15_Click(object sender, EventArgs e)
        {
            _socketConnect.SendSqlRaw("ScanLabelString", Class.SocketConnect.SQLOption.Delete, "{\"DocumentNo\":\"abcd\"}");
        }

        private void button16_Click(object sender, EventArgs e) => new ListPage(_socketConnect, "Company", _userID).Show();

        private void button17_Click(object sender, EventArgs e) => DynamicOpenCustom("ODataSetupPage");

        private void button18_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            DialogResult result = form3.ShowDialog();
            if (result == DialogResult.OK)
            {
                form3 = null;
            }
        }

        private void button20_Click(object sender, EventArgs e) => DynamicOpenCustom("SetupPage");

        private void DynamicOpenCustom(string formKey)
        {
            if (_customFormRegistry.TryGetValue(formKey, out var factory))
            {
                factory().Show();
            }
        }

        /// <summary>Open Menu2 (list mode, like Android).</summary>
        private void btnMenu2_Click(object sender, EventArgs e)
        {
            // Legacy entry point kept for designer compatibility.
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Menu page should not close with ESC.
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
