using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMSClient.Filter
{
    public partial class FilterForm : Form
    {
        String[] TitleList;
        String[] usedTitleList;
        int[] usedTitleList_int;
        int SeqNo = 1;
        public FilterForm(String[] TitleList)
        {
            InitializeComponent();
            this.TitleList = TitleList;
            this.AutoScroll = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.None;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(this.Size.Width - 25, this.Size.Height);
            CreateNewPanel();
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateNewPanel();
        }
        private void CreateNewPanel()
        {
            Panel panel = new Panel();
            panel.MaximumSize = new Size(MaximumSize.Width, 25);
            panel.AutoSize = true;
            panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Button button = new Button();
            button.MaximumSize = new Size(25, 25);
            button.Image = Properties.Resources.Cancel;
            button.Click += (sender2, args) =>
            {
                flowLayoutPanel1.Controls.Remove(panel);
            };

            ComboBox comboBox_1 = new ComboBox();
            comboBox_1.MinimumSize = new Size(150, 20);
            comboBox_1.Location = new System.Drawing.Point(button.Location.X + button.Width + 10, 0);
            Array array = TitleList;
            Array array2 = usedTitleList;
            if (!(array == null || array.Length == 0))
            foreach (string Title in TitleList)
            {
                    /*
                    string TitleStr = Title;
                    if (!(array == null || array.Length == 0))
                        if (array.Equals(TitleStr))
                            TitleStr = "";
                    if (TitleStr != "")
                        comboBox_1.Items.Add(TitleStr);
                        */
                    Boolean find = false;
                    if (!(usedTitleList == null))
                    {
                        foreach (string Title2 in usedTitleList)
                        {
                            if (Title2.ToString() == Title.ToString())
                            {
                                find = true;
                            }
                        }
                    }
                    if (!find)
                    {
                        comboBox_1.Items.Add(Title);
                        if (!(usedTitleList == null))
                        {
                            int arrayLength = usedTitleList.Length;
                            usedTitleList = new string[arrayLength + 1];
                            Array.Clear(usedTitleList, 0, usedTitleList.Length);
                            usedTitleList[arrayLength] = Title;
                        }
                        else
                        {
                            usedTitleList = new string[1];
                            usedTitleList[0] = Title.ToString();
                        }
                    }
            }
            comboBox_1.SelectedIndex = 0;
            //comboBox_1.SelectedIndexChanged +=
            TextBox textbox_2 = new TextBox();
            textbox_2.MinimumSize = new Size(150, 20);
            textbox_2.Location = new System.Drawing.Point(comboBox_1.Location.X + comboBox_1.Width + 10, 0);

            panel.Controls.Add(button);
            panel.Controls.Add(comboBox_1);
            panel.Controls.Add(textbox_2);
            flowLayoutPanel1.Controls.Add(panel);
            flowLayoutPanel1.Controls.SetChildIndex(panel, SeqNo);
            SeqNo = SeqNo + 1;

            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.Controls.SetChildIndex(button2, SeqNo);
        }

    }
}
