using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    class TimersTimer
    {
        //定义全局变量
        public int currentCount = 0;
        //定义Timer类
        System.Timers.Timer timer;
        //定义委托
        public delegate void SetControlValue(string value);
        public static NAV.NAV nav;

        /// <summary>
        /// 初始化Timer控件
        /// </summary>
        public void InitTimer()
        {
            //设置定时间隔(毫秒为单位)
            int interval = 5000;
            timer = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer.Enabled = true;
            //绑定Elapsed事件
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp);
        }

        /// <summary>
        /// Timer类执行定时到点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //OdataAction.OdataLink(nav);
                //OdataAction.PrintCustomersCalledCust(nav);
                OdataAction.OdataLink();
                OdataAction.SyncData();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("执行定时到点事件失败:" + ex.Message);
            }
        }

        public void timer_Start()
        {
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);//Log
            timer.Start();
        }

        public void timer_Stop()
        {
            timer.Stop();
        }
    }
}
