namespace WMS
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ProttextBox = new System.Windows.Forms.TextBox();
            this.StartStopBtn = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.IPlistBox = new System.Windows.Forms.ListBox();
            this.MsgTextBox = new System.Windows.Forms.TextBox();
            this.Sendbtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lblReceiveTimeout = new System.Windows.Forms.Label();
            this.nudReceiveTimeoutMs = new System.Windows.Forms.NumericUpDown();
            this.lblSendTimeout = new System.Windows.Forms.Label();
            this.nudSendTimeoutMs = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudReceiveTimeoutMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSendTimeoutMs)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(252, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = " ";
            // 
            // ProttextBox
            // 
            this.ProttextBox.Location = new System.Drawing.Point(215, 10);
            this.ProttextBox.Name = "ProttextBox";
            this.ProttextBox.Size = new System.Drawing.Size(76, 20);
            this.ProttextBox.TabIndex = 3;
            this.ProttextBox.Text = "5123";
            // 
            // StartStopBtn
            // 
            this.StartStopBtn.Location = new System.Drawing.Point(297, 8);
            this.StartStopBtn.Name = "StartStopBtn";
            this.StartStopBtn.Size = new System.Drawing.Size(75, 23);
            this.StartStopBtn.TabIndex = 4;
            this.StartStopBtn.Text = "Start/Stop";
            this.StartStopBtn.UseVisualStyleBackColor = true;
            this.StartStopBtn.Click += new System.EventHandler(this.Startbtn_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(16, 41);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(481, 198);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // IPlistBox
            // 
            this.IPlistBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IPlistBox.FormattingEnabled = true;
            this.IPlistBox.Location = new System.Drawing.Point(517, 3);
            this.IPlistBox.Name = "IPlistBox";
            this.IPlistBox.Size = new System.Drawing.Size(158, 264);
            this.IPlistBox.TabIndex = 6;
            // 
            // MsgTextBox
            // 
            this.MsgTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MsgTextBox.Location = new System.Drawing.Point(16, 300);
            this.MsgTextBox.Name = "MsgTextBox";
            this.MsgTextBox.Size = new System.Drawing.Size(481, 20);
            this.MsgTextBox.TabIndex = 7;
            // 
            // Sendbtn
            // 
            this.Sendbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Sendbtn.Location = new System.Drawing.Point(517, 283);
            this.Sendbtn.Name = "Sendbtn";
            this.Sendbtn.Size = new System.Drawing.Size(158, 53);
            this.Sendbtn.TabIndex = 8;
            this.Sendbtn.Text = "Send";
            this.Sendbtn.UseVisualStyleBackColor = true;
            this.Sendbtn.Click += new System.EventHandler(this.Sendbtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(183, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Prot";
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Image = global::WMS.Properties.Resources.Debug;
            this.button5.Location = new System.Drawing.Point(457, 245);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(40, 39);
            this.button5.TabIndex = 14;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Image = global::WMS.Properties.Resources.Database;
            this.button4.Location = new System.Drawing.Point(80, 245);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(49, 39);
            this.button4.TabIndex = 13;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Image = global::WMS.Properties.Resources.Database;
            this.button3.Location = new System.Drawing.Point(16, 245);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(49, 39);
            this.button3.TabIndex = 12;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Image = global::WMS.Properties.Resources.Database;
            this.button2.Location = new System.Drawing.Point(445, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(49, 39);
            this.button2.TabIndex = 10;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Image = global::WMS.Properties.Resources.Refresh;
            this.button1.Location = new System.Drawing.Point(389, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(49, 39);
            this.button1.TabIndex = 9;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(36, 10);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(141, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // lblReceiveTimeout
            // 
            this.lblReceiveTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblReceiveTimeout.AutoSize = true;
            this.lblReceiveTimeout.Location = new System.Drawing.Point(13, 254);
            this.lblReceiveTimeout.Name = "lblReceiveTimeout";
            this.lblReceiveTimeout.Size = new System.Drawing.Size(98, 13);
            this.lblReceiveTimeout.TabIndex = 15;
            this.lblReceiveTimeout.Text = "Receive timeout (ms)";
            // 
            // nudReceiveTimeoutMs
            // 
            this.nudReceiveTimeoutMs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudReceiveTimeoutMs.Increment = new decimal(new int[] { 60000, 0, 0, 0 });
            this.nudReceiveTimeoutMs.Location = new System.Drawing.Point(117, 252);
            this.nudReceiveTimeoutMs.Maximum = new decimal(new int[] { 86400000, 0, 0, 0 });
            this.nudReceiveTimeoutMs.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudReceiveTimeoutMs.Name = "nudReceiveTimeoutMs";
            this.nudReceiveTimeoutMs.Size = new System.Drawing.Size(90, 20);
            this.nudReceiveTimeoutMs.TabIndex = 16;
            this.nudReceiveTimeoutMs.Value = new decimal(new int[] { 300000, 0, 0, 0 });
            // 
            // lblSendTimeout
            // 
            this.lblSendTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSendTimeout.AutoSize = true;
            this.lblSendTimeout.Location = new System.Drawing.Point(213, 254);
            this.lblSendTimeout.Name = "lblSendTimeout";
            this.lblSendTimeout.Size = new System.Drawing.Size(83, 13);
            this.lblSendTimeout.TabIndex = 17;
            this.lblSendTimeout.Text = "Send timeout (ms)";
            // 
            // nudSendTimeoutMs
            // 
            this.nudSendTimeoutMs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudSendTimeoutMs.Increment = new decimal(new int[] { 10000, 0, 0, 0 });
            this.nudSendTimeoutMs.Location = new System.Drawing.Point(302, 252);
            this.nudSendTimeoutMs.Maximum = new decimal(new int[] { 86400000, 0, 0, 0 });
            this.nudSendTimeoutMs.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudSendTimeoutMs.Name = "nudSendTimeoutMs";
            this.nudSendTimeoutMs.Size = new System.Drawing.Size(90, 20);
            this.nudSendTimeoutMs.TabIndex = 18;
            this.nudSendTimeoutMs.Value = new decimal(new int[] { 60000, 0, 0, 0 });
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 347);
            this.Controls.Add(this.nudSendTimeoutMs);
            this.Controls.Add(this.lblSendTimeout);
            this.Controls.Add(this.nudReceiveTimeoutMs);
            this.Controls.Add(this.lblReceiveTimeout);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Sendbtn);
            this.Controls.Add(this.MsgTextBox);
            this.Controls.Add(this.IPlistBox);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.StartStopBtn);
            this.Controls.Add(this.ProttextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1 v0.2.22";
            ((System.ComponentModel.ISupportInitialize)(this.nudReceiveTimeoutMs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSendTimeoutMs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ProttextBox;
        private System.Windows.Forms.Button StartStopBtn;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ListBox IPlistBox;
        private System.Windows.Forms.TextBox MsgTextBox;
        private System.Windows.Forms.Button Sendbtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lblReceiveTimeout;
        private System.Windows.Forms.NumericUpDown nudReceiveTimeoutMs;
        private System.Windows.Forms.Label lblSendTimeout;
        private System.Windows.Forms.NumericUpDown nudSendTimeoutMs;
    }
}

