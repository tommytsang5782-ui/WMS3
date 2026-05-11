using System;
using System.Drawing;
using System.Windows.Forms;
using WMSClient.Class;

namespace WMSClient.Base
{
    /// <summary>
    /// Base form for list/card pages: fixed toolbar (New, Edit, Delete, Refresh, Clear Filter) and grid.
    /// Subclasses add columns and optional extra menu items without replacing the fixed ones.
    /// </summary>
    public class BaseListForm : BaseBusinessForm
    {
        protected MenuStrip FixedMenuStrip => _menuStrip;
        protected DataGridView ListGrid => _dataGridView;
        private MenuStrip _menuStrip;
        private DataGridView _dataGridView;

        protected BaseListForm() { }

        public BaseListForm(SocketConnect socketConnect) : base(socketConnect)
        {
            if (IsDesignMode) return;
            BuildFixedLayout();
        }

        /// <summary>Build fixed toolbar and grid. Override OnAddColumns / OnAddExtraMenuItems to add per-page content.</summary>
        private void BuildFixedLayout()
        {
            _menuStrip = new MenuStrip();
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = SystemColors.Window,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };

            var mNew = new ToolStripMenuItem("New") { Name = "toolStripNew" };
            var mEdit = new ToolStripMenuItem("Edit") { Name = "toolStripEdit" };
            var mDelete = new ToolStripMenuItem("Delete") { Name = "toolStripDelete" };
            var mRefresh = new ToolStripMenuItem("Refresh") { Name = "toolStripRefresh" };
            var mClearFilter = new ToolStripMenuItem("Clear Filter") { Name = "toolStripClearFilter" };

            mNew.Click += (s, e) => OnNewClick();
            mEdit.Click += (s, e) => OnEditClick();
            mDelete.Click += (s, e) => OnDeleteClick();
            mRefresh.Click += (s, e) => OnRefreshClick();
            mClearFilter.Click += (s, e) => OnClearFilterClick();

            _menuStrip.Items.Add(mNew);
            _menuStrip.Items.Add(mEdit);
            _menuStrip.Items.Add(mDelete);
            _menuStrip.Items.Add(mRefresh);
            _menuStrip.Items.Add(mClearFilter);

            Controls.Add(_dataGridView);
            Controls.Add(_menuStrip);
            _dataGridView.BringToFront();
            _menuStrip.BringToFront();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsDesignMode) return;
            OnAddColumns();
            OnAddExtraMenuItems();
        }

        /// <summary>Override to add grid columns. Fixed columns (if any) should be added first; then add per-page columns.</summary>
        protected virtual void OnAddColumns() { }

        /// <summary>Override to add extra menu items to the fixed toolbar (items are appended, fixed ones are not removed).</summary>
        protected virtual void OnAddExtraMenuItems() { }

        protected virtual void OnNewClick() { }
        protected virtual void OnEditClick() { }
        protected virtual void OnDeleteClick() { }
        protected virtual void OnRefreshClick() { }
        protected virtual void OnClearFilterClick() { }
    }
}
