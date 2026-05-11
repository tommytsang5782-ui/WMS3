namespace WMSClient
{
    partial class UserCard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OKbtn_UserCard = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.UserID_UserCard = new System.Windows.Forms.TextBox();
            this.Password_UserCard = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // OKbtn_UserCard
            // 
            this.OKbtn_UserCard.Location = new System.Drawing.Point(78, 144);
            this.OKbtn_UserCard.Name = "OKbtn_UserCard";
            this.OKbtn_UserCard.Size = new System.Drawing.Size(90, 34);
            this.OKbtn_UserCard.TabIndex = 0;
            this.OKbtn_UserCard.Text = "OK";
            this.OKbtn_UserCard.UseVisualStyleBackColor = true;
            this.OKbtn_UserCard.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "User ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // UserID_UserCard
            // 
            this.UserID_UserCard.Location = new System.Drawing.Point(78, 41);
            this.UserID_UserCard.Name = "UserID_UserCard";
            this.UserID_UserCard.Size = new System.Drawing.Size(153, 20);
            this.UserID_UserCard.TabIndex = 3;
            // 
            // Password_UserCard
            // 
            this.Password_UserCard.Location = new System.Drawing.Point(78, 91);
            this.Password_UserCard.Name = "Password_UserCard";
            this.Password_UserCard.Size = new System.Drawing.Size(153, 20);
            this.Password_UserCard.TabIndex = 4;
            // 
            // UserCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(250, 206);
            this.Controls.Add(this.Password_UserCard);
            this.Controls.Add(this.UserID_UserCard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OKbtn_UserCard);
            this.Name = "UserCard";
            this.Text = "UserCard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKbtn_UserCard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox UserID_UserCard;
        private System.Windows.Forms.TextBox Password_UserCard;
    }
}