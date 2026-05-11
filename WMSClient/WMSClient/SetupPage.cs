using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using static WMSClient.Class.SocketConnect;

namespace WMSClient
{
    public partial class SetupPage : BaseBusinessForm
    {
        //necessary + 
        BindingSource bindingSource = new BindingSource();
        //necessary - 

        //necessary but different +
        List<Setup> dataList;
        Setup dataSetup;
        String tableName = "Setup";
        //necessary but different -

        //other + 
        String A = "";
        //other -

        public SetupPage(SocketConnect socketConnect)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Setup";

            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            Setup setup = new Setup();
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,setup);
                dataList = JsonConvert.DeserializeObject<List<Setup>>(a);
                bindingSource.DataSource = dataList;
                foreach (Setup a1 in dataList)
                {
                    dataSetup = a1;
                    Console.WriteLine(a1.ExcelPath);
                }
                textBox1.Text = dataSetup.ExcelPath;

            }
            catch
            {
            }
            //String[] str = list.ToArray();

            
        }
        private void button3_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Excel Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "xlsx",
                Filter = "Microsoft Excel 檔案(*.xl*,*.xls,*.xlsx,*.xlsm,*.xlsb,*.xla,*.xlt,*.xlm,*.xlc,*.xlw)|*.xl*;*.xls;*.xlsx;*.xlsm;*.xlsb;*.xla;*.xlt;*.xlm;*.xlc;*.xlw",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            Setup updateSetup = new Setup();
            updateSetup.ExcelPath = textBox1.Text;
            List<Setup> setupList = new List<Setup>();
            setupList.Add(dataSetup);
            setupList.Add(updateSetup);

            String a = _socketConnect.SendMessage(SQLOption.Update,setupList);
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
