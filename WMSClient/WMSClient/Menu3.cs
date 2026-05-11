using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;

namespace WMSClient
{
    public partial class Menu3 : BaseBusinessForm
    {
        private readonly string _userId;
        private TreeView _menuTree;

        public Menu3()
        {
            InitializeComponent();
        }
        public Menu3(SocketConnect socketConnect, string userId) : base(socketConnect)
        {
            InitializeComponent();
            _userId = userId;
        }
        void BuildMenu(TreeView tree, List<MenuItem> menu)
        {
            tree.Nodes.Clear();
            foreach (var item in menu ?? new List<MenuItem>())
            {
                TreeNode node = new TreeNode(item.Text);
                node.Tag = item;
                AddChildren(node, item.Children);
                tree.Nodes.Add(node);
            }
        }

        private void AddChildren(TreeNode parent, List<MenuItem> children)
        {
            foreach (var child in children ?? new List<MenuItem>())
            {
                var node = new TreeNode(child.Text) { Tag = child };
                AddChildren(node, child.Children);
                parent.Nodes.Add(node);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_menuTree == null)
            {
                _menuTree = new TreeView
                {
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 10F)
                };
                _menuTree.NodeMouseDoubleClick += MenuTree_NodeMouseDoubleClick;
                contentPanel.Controls.Add(_menuTree);
            }

            var cfg = _socketConnect?.GetMetadata("Shell", "Shell", _userId);
            BuildMenu(_menuTree, cfg?.Menu ?? new List<MenuItem>());
            _menuTree.ExpandAll();
        }

        private void MenuTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!(e.Node.Tag is MenuItem item) || string.IsNullOrWhiteSpace(item.FormName))
            {
                return;
            }

            if (string.Equals(item.FormKind, "Crud", StringComparison.OrdinalIgnoreCase))
            {
                new ListPage(_socketConnect, item.FormName, _userId).Show();
            }
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
