namespace WMSClient
{
    partial class Menu2Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnSaveOrder = new System.Windows.Forms.Button();
            this.comboOrder = new System.Windows.Forms.ComboBox();
            this.lblOrder = new System.Windows.Forms.Label();
            this.panelMenu = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).BeginInit();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnSaveOrder);
            this.panelTop.Controls.Add(this.comboOrder);
            this.panelTop.Controls.Add(this.lblOrder);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panelTop.Size = new System.Drawing.Size(304, 44);
            this.panelTop.TabIndex = 1;
            // 
            // btnSaveOrder
            // 
            this.btnSaveOrder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSaveOrder.Location = new System.Drawing.Point(204, 8);
            this.btnSaveOrder.Name = "btnSaveOrder";
            this.btnSaveOrder.Size = new System.Drawing.Size(92, 28);
            this.btnSaveOrder.TabIndex = 2;
            this.btnSaveOrder.Text = "Save as custom";
            this.btnSaveOrder.UseVisualStyleBackColor = true;
            // 
            // comboOrder
            // 
            this.comboOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOrder.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.comboOrder.FormattingEnabled = true;
            this.comboOrder.Location = new System.Drawing.Point(58, 10);
            this.comboOrder.Name = "comboOrder";
            this.comboOrder.Size = new System.Drawing.Size(140, 25);
            this.comboOrder.TabIndex = 1;
            // 
            // lblOrder
            // 
            this.lblOrder.AutoSize = true;
            this.lblOrder.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblOrder.Location = new System.Drawing.Point(8, 12);
            this.lblOrder.Name = "lblOrder";
            this.lblOrder.Size = new System.Drawing.Size(48, 19);
            this.lblOrder.TabIndex = 0;
            this.lblOrder.Text = "Order:";
            // 
            // panelMenu
            // 
            this.panelMenu.AutoScroll = true;
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMenu.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.panelMenu.Location = new System.Drawing.Point(0, 44);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Padding = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.panelMenu.Size = new System.Drawing.Size(304, 307);
            this.panelMenu.TabIndex = 0;
            this.panelMenu.WrapContents = false;
            // 
            // Menu2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 351);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "Menu2Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Menu2";
            this.Load += new System.EventHandler(this.Menu2Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel panelMenu;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblOrder;
        private System.Windows.Forms.ComboBox comboOrder;
        private System.Windows.Forms.Button btnSaveOrder;
    }
}
