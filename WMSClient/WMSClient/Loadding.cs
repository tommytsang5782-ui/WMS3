using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMSClient
{
    public partial class Loadding : Form
    {
        private IAsyncResult _AsyncResult;
        private EventHandler<EventArgs> _Method;
        public Loadding()
        {
            InitializeComponent();
        }
        public Loadding(EventHandler<EventArgs> method)
        {
            InitializeComponent();
            _Method = method;

        }
        private static Loadding load = null;
        public static Loadding getInstance(EventHandler<EventArgs> method)
        {
            if (load == null || load.IsDisposed)
            {
                load = new Loadding(method);
            }
            return load;
        }

        private void OnShown(object sender, EventArgs e)
        {
            if (Win32.AnimateWindow(this.Handle, 100, Win32.AW_ACTIVATE | Win32.AW_VER_POSITIVE | Win32.AW_BLEND))
            {
                //上到下特效显示  
                Win32.AnimateWindow(this.Handle, 100, Win32.AW_ACTIVATE);
            }
            _AsyncResult = _Method.BeginInvoke(null, null, null, null);
        }
        private void FormClose(object sender, EventArgs e)
        {
            if (Win32.AnimateWindow(this.Handle, 100, Win32.AW_HIDE | Win32.AW_VER_POSITIVE | Win32.AW_BLEND))
            {
                //上到下特效显示  
                Win32.AnimateWindow(this.Handle, 100, Win32.AW_HIDE);
                
            }
        }
        private void _Timer_Tick(object sender, EventArgs e)
        {
            if (_AsyncResult.IsCompleted)
            {
                this.Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
    public class Win32
    {
        public const Int32 AW_HOR_POSITIVE = 0x00000001; // 从左到右打开窗口
        public const Int32 AW_HOR_NEGATIVE = 0x00000002; // 从右到左打开窗口
        public const Int32 AW_VER_POSITIVE = 0x00000004; // 从上到下打开窗口
        public const Int32 AW_VER_NEGATIVE = 0x00000008; // 从下到上打开窗口
        public const Int32 AW_CENTER = 0x00000010; //若使用了AW_HIDE标志，则使窗口向内重叠；若未使用AW_HIDE标志，则使窗口向外扩展。
        public const Int32 AW_HIDE = 0x00010000; //隐藏窗口，缺省则显示窗口。
        public const Int32 AW_ACTIVATE = 0x00020000; //激活窗口。在使用了AW_HIDE标志后不要使用这个标志。
        public const Int32 AW_SLIDE = 0x00040000; //使用滑动类型。缺省则为滚动动画类型。当使用AW_CENTER标志时，这个标志就被忽略。
        public const Int32 AW_BLEND = 0x00080000; //使用淡出效果。只有当hWnd为顶层窗口的时候才可以使用此标志。
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool AnimateWindow(
      IntPtr hwnd, // handle to window
          int dwTime, // duration of animation
          int dwFlags // animation type
          );
    }
}
