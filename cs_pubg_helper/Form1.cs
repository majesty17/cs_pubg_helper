using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cs_pubg_helper
{
    public partial class Form1 : Form
    {
        //各阶段时间
        int[,] timeIntervals = new int[,] { { 300, 300 }, { 200, 140 }, { 150, 90 }, 
        { 120, 60 }, { 120, 40 }, { 90, 30 }, { 90, 25 }, { 60, 20 } };
        Label[,] timerLabel = null;
        DateTime startTime;



        public Form1()
        {
            InitializeComponent();
        }

        //初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化计时器
            initTimer();
        }






        //初始化计时器
        private void initTimer() {
            timerLabel=new Label[timeIntervals.Length,2];
            //((Label)tableLayoutPanel1.GetControlFromPosition(2, 7)).Text = "123";

            for (int i = 0; i < timeIntervals.Length/2; i++) {
                timerLabel[i, 0] = (Label)(tableLayoutPanel1.GetControlFromPosition(0, i + 1));
                timerLabel[i, 1] = (Label)(tableLayoutPanel1.GetControlFromPosition(2, i + 1));

                ((Label)(tableLayoutPanel1.GetControlFromPosition(1, i + 1))).Text = "第 " + (i + 1) + " 轮";

                timerLabel[i, 0].Text = timeIntervals[i, 0] + "";
                timerLabel[i, 1].Text = timeIntervals[i, 1] + "";
            }
            timer1.Stop();
        }

        //开始
        private void button_start_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            timer1.Start();
        }
        //停止
        private void button_reset_Click(object sender, EventArgs e)
        {
            initTimer();
        }


        //时钟事件
        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            //游戏进行时间
            int span = (int)((now - startTime).TotalSeconds);

            //找到属于第几个时段
            int i;
            int sumtime=0;
            int currentLeft = 0;
            for (i = 0; i < timeIntervals.Length; i++) {
                sumtime += timeIntervals[i/2,i%2];
                if (span <= sumtime)
                {
                    currentLeft = sumtime - span;
                    break;
                }
            }

            if (i == timeIntervals.Length) {
                timer1.Stop();
                MessageBox.Show("Game is over!");
                return;
            }

            label28.Text = "游戏总运行时间：" + span + " 秒";

            //更新当前计数器
            timerLabel[i/2, i%2].Text = "" + currentLeft;
            //更新当前轮次颜色
            ((Label)(tableLayoutPanel1.GetControlFromPosition(1, i / 2 + 1))).BackColor = Color.Gray;



        }

        //游戏开始时间后推1s
        private void button_1s_plus_Click(object sender, EventArgs e)
        {
            startTime = startTime.AddSeconds(5.0);
        }
        //游戏开始时间提前1s
        private void button_1s_minus_Click(object sender, EventArgs e)
        {
            startTime = startTime.AddSeconds(-5.0);
        }




    }
}
