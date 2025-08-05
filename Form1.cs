using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WinLock.Form1;

namespace WinLock
{
    public partial class Form1 : Form
    {
        public bool isStart=false;
        public int opt=1;
        public int timeOpt=1;
        public long timestampSeconds = 0;
        public long leftTime = 0;

        Timer timer1 = null;
        public struct Date
        {
            public int year;
            public int month;
            public int day;
            public int hour;
            public int minute;
            public int second;
        }
        Date date;

        enum Option
        {
            关机=1, 重启, 注销, 睡眠, 锁定
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        
        public void SetEnable(bool enable)
        {
            radioButton1.Enabled = enable;
            radioButton2.Enabled = enable;
            radioButton3.Enabled = enable;
            radioButton4.Enabled = enable;
            radioButton5.Enabled = enable;
            radioButton6.Enabled = enable;
            radioButton7.Enabled = enable;
            dateTimePicker1.Enabled = enable;
            numericUpDown1.Enabled = enable;
            numericUpDown2.Enabled = enable;
            numericUpDown3.Enabled = enable;

        }

        private void button1_Click(object sender, EventArgs e)
        {
         
            if (isStart)
            {
                panel1.Visible = false;
                SetEnable(true);
                timer1.Stop();           // 先停止
                timer1.Dispose();        // 释放资源
                timer1 = null;           // 可选：置空引用
            }
            else
            {
                panel1.Visible = true;
               
                if (timeOpt == 1)
                {
                    date.hour = (int)numericUpDown1.Value;
                    date.minute = (int)numericUpDown2.Value;
                    date.second = (int)numericUpDown3.Value;

                    string hourStr = date.hour > 9 ? date.hour.ToString() : "0" + date.hour.ToString();
                    string minuteStr = date.minute > 9 ? date.minute.ToString() : "0" + date.minute.ToString();
                    string secondStr = date.second > 9 ? date.second.ToString() : "0" + date.second.ToString();

                    leftTime = date.hour * 3600 + date.minute * 60 + date.second;

                    label4.Text = "距离" + (Option)opt + "还有" + hourStr + ":" + minuteStr + ":" + secondStr;

                }
                else
                {
                    date.year = dateTimePicker1.Value.Year;
                    date.month = dateTimePicker1.Value.Month;
                    date.day = dateTimePicker1.Value.Day;
                    date.hour = (int)numericUpDown1.Value;
                    date.minute = (int)numericUpDown2.Value;
                    date.second = (int)numericUpDown3.Value;

                    string yearStr = date.year.ToString();
                    string monthStr = date.month.ToString();
                    string dayStr = date.day.ToString();
                    string hourStr = date.hour> 9? date.hour.ToString():"0"+ date.hour.ToString();
                    string minuteStr = date.minute > 9 ? date.minute.ToString() : "0" + date.minute.ToString();
                    string secondStr = date.second > 9 ? date.second.ToString() : "0" + date.second.ToString();

                    string timeStr= yearStr + "-" + monthStr + "-" + dayStr + " " + hourStr + ":" + minuteStr + ":" + secondStr;
                    string format = "yyyy-M-d HH:mm:ss";
                    DateTime dateTime = DateTime.ParseExact(
                        timeStr,
                        format,
                        CultureInfo.InvariantCulture
                    );
                    DateTime utcDateTime = dateTime.ToUniversalTime();

                    timestampSeconds = new DateTimeOffset(utcDateTime).ToUnixTimeSeconds();  // 设置时间的时间戳

                  
                    DateTime utcNow = DateTime.Now;
                    long nowTimestamp = new DateTimeOffset(utcNow).ToUnixTimeSeconds();

                    long lastTimestamp = timestampSeconds - nowTimestamp;
                    if( lastTimestamp < 0)
                    {
                        MessageBox.Show("过去已是过去");
                        return;
                    }

                    // 转换为 TimeSpan
                    TimeSpan remainingTime = TimeSpan.FromSeconds(lastTimestamp);

                    // 提取天、时、分、秒
                    string daysStr = remainingTime.Days.ToString();
                    string hoursStr = remainingTime.Hours.ToString();
                    string minutesStr = remainingTime.Minutes.ToString();
                    string secondsStr = remainingTime.Seconds.ToString();


                    string text = "距离"+ yearStr+"年"+ monthStr + "月"+ dayStr + "日" + 
                        " " + hourStr+ ":" + minuteStr+":"+ secondStr+ (Option)opt+"\n" +"还有"+ daysStr + "天"+ hoursStr + "时"+ minutesStr + "分"+ secondsStr + "秒";
                        
                    label4.Text = text;
                }
                timer1 = new Timer();
                timer1.Interval = 1000;
                timer1.Enabled = true;
                timer1.Tick += new EventHandler(timer1EventProcessor);//添加事件
                SetEnable(false);

            }
            button1.Text = isStart ? "开始任务" : "停止任务";
            isStart = !isStart;
        }
        public void timer1EventProcessor(object source, EventArgs e)
        {
            timestampSeconds -= 1;

           
            if (timeOpt == 1)
            {
                leftTime -= 1;
                if (leftTime < 0)
                {
                    timer1.Stop();           // 先停止
                    timer1.Dispose();        // 释放资源
                    timer1 = null;           // 可选：置空引用
                    return;
                }
                date.hour = (int)numericUpDown1.Value;
                date.minute = (int)numericUpDown2.Value;
                date.second = (int)numericUpDown3.Value;

                // 转换为 TimeSpan
                TimeSpan remainingTime = TimeSpan.FromSeconds(leftTime);

                // 提取天、时、分、秒
                string hoursStr = remainingTime.Hours>9 ? remainingTime.Hours.ToString():"0" + remainingTime.Hours.ToString();
                string minutesStr = remainingTime.Minutes > 9 ? remainingTime.Minutes.ToString() : "0" + remainingTime.Minutes.ToString();
                string secondsStr = remainingTime.Seconds > 9 ? remainingTime.Seconds.ToString() : "0" + remainingTime.Seconds.ToString();



                label4.Text = "距离" + (Option)opt + "还有" + hoursStr + ":" + minutesStr + ":" + secondsStr;

            }
            else
            {
                date.year = dateTimePicker1.Value.Year;
                date.month = dateTimePicker1.Value.Month;
                date.day = dateTimePicker1.Value.Day;
                date.hour = (int)numericUpDown1.Value;
                date.minute = (int)numericUpDown2.Value;
                date.second = (int)numericUpDown3.Value;

                string yearStr = date.year.ToString();
                string monthStr = date.month.ToString();
                string dayStr = date.day.ToString();
                string hourStr = date.hour > 9 ? date.hour.ToString() : "0" + date.hour.ToString();
                string minuteStr = date.minute > 9 ? date.minute.ToString() : "0" + date.minute.ToString();
                string secondStr = date.second > 9 ? date.second.ToString() : "0" + date.second.ToString();

                string timeStr = yearStr + "-" + monthStr + "-" + dayStr + " " + hourStr + ":" + minuteStr + ":" + secondStr;
                string format = "yyyy-M-d HH:mm:ss";
                DateTime dateTime = DateTime.ParseExact(
                    timeStr,
                    format,
                    CultureInfo.InvariantCulture
                );
                DateTime utcDateTime = dateTime.ToUniversalTime();

                timestampSeconds = new DateTimeOffset(utcDateTime).ToUnixTimeSeconds();  // 设置时间的时间戳


                DateTime utcNow = DateTime.Now;
                long nowTimestamp = new DateTimeOffset(utcNow).ToUnixTimeSeconds();

                long lastTimestamp = timestampSeconds - nowTimestamp;
                if (lastTimestamp < 0)
                {
                    timer1.Stop();           // 先停止
                    timer1.Dispose();        // 释放资源
                    timer1 = null;           // 可选：置空引用
                    return;
                }

                // 转换为 TimeSpan
                TimeSpan remainingTime = TimeSpan.FromSeconds(lastTimestamp);

                // 提取天、时、分、秒
                string daysStr = remainingTime.Days.ToString();
                string hoursStr = remainingTime.Hours.ToString();
                string minutesStr = remainingTime.Minutes.ToString();
                string secondsStr = remainingTime.Seconds.ToString();


                string text = "距离" + yearStr + "年" + monthStr + "月" + dayStr + "日" +
                    " " + hourStr + ":" + minuteStr + ":" + secondStr + (Option)opt+"\n" + "还有" + daysStr + "天" + hoursStr + "时" + minutesStr + "分" + secondsStr + "秒";

                label4.Text = text;

            }

          

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                opt = 1;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                opt = 2;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                opt = 3;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                opt = 4;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                opt = 5;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                timeOpt = 1;
                dateTimePicker1.Hide();
            }
        }
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                timeOpt = 2;
                dateTimePicker1.Show();
                numericUpDown1.Maximum = 24;
            }
        }
    }
}
