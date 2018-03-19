using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;

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

            //默认选中第四个tab
            tabControl1.SelectedIndex = 3;


            //热键
            Hotkey hotkey;
            hotkey = new Hotkey(this.Handle);
            Hotkey1 = hotkey.RegisterHotkey(System.Windows.Forms.Keys.V, 0);   //定义快键(Ctrl + F2)
            hotkey.OnHotkey += new HotkeyEventHandler(OnHotkey);
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        private Color cross_color=Color.Red;
        private int cross_size;
        private int cross_type;
        private int cross_opacity;

        /// <summary>
        /// 开启准星模拟器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cross_open_Click(object sender, EventArgs e)
        {
            cross_size = (int)numericUpDown_size.Value;
            cross_opacity = (int)numericUpDown_opacity.Value;
        }
        /// <summary>
        /// 颜色选择器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                cross_color = ColorForm.Color;
                //GetColor就是用户选择的颜色，接下来就可以使用该颜色了
                panel_color.BackColor = cross_color;
            }
        }



        /// <summary>
        /// 模拟倍镜
        /// </summary>
        /// <param name="HotKeyID"></param>


        public delegate void HotkeyEventHandler(int HotKeyID);
        private int Hotkey1;
        public class Hotkey : System.Windows.Forms.IMessageFilter
        {
            Hashtable keyIDs = new Hashtable();
            IntPtr hWnd;

            public event HotkeyEventHandler OnHotkey;

            public enum KeyFlags
            {
                MOD_ALT = 0x1,
                MOD_CONTROL = 0x2,
                MOD_SHIFT = 0x4,
                MOD_WIN = 0x8
            }
            [DllImport("user32.dll")]
            public static extern UInt32 RegisterHotKey(IntPtr hWnd, UInt32 id, UInt32 fsModifiers, UInt32 vk);

            [DllImport("user32.dll")]
            public static extern UInt32 UnregisterHotKey(IntPtr hWnd, UInt32 id);

            [DllImport("kernel32.dll")]
            public static extern UInt32 GlobalAddAtom(String lpString);

            [DllImport("kernel32.dll")]
            public static extern UInt32 GlobalDeleteAtom(UInt32 nAtom);

            public Hotkey(IntPtr hWnd)
            {
                this.hWnd = hWnd;
                Application.AddMessageFilter(this);
            }

            public int RegisterHotkey(Keys Key, KeyFlags keyflags)
            {
                UInt32 hotkeyid = GlobalAddAtom(System.Guid.NewGuid().ToString());
                RegisterHotKey((IntPtr)hWnd, hotkeyid, (UInt32)keyflags, (UInt32)Key);
                keyIDs.Add(hotkeyid, hotkeyid);
                return (int)hotkeyid;
            }

            public void UnregisterHotkeys()
            {
                Application.RemoveMessageFilter(this);
                foreach (UInt32 key in keyIDs.Values)
                {
                    UnregisterHotKey(hWnd, key);
                    GlobalDeleteAtom(key);
                }
            }

            public bool PreFilterMessage(ref System.Windows.Forms.Message m)
            {
                if (m.Msg == 0x312)
                {
                    if (OnHotkey != null)
                    {
                        foreach (UInt32 key in keyIDs.Values)
                        {
                            if ((UInt32)m.WParam == key)
                            {
                                OnHotkey((int)m.WParam);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }
        //热键处理
        public void OnHotkey(int HotkeyID)
        {
            if (HotkeyID == Hotkey1 && checkBox_scope_switch.Checked)
            {
                textBox2.AppendText("hahah");

            }
        }

        private Form_cross form_cross;
        //开启
        private void checkBox_scope_switch_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_scope_switch.Checked)
            {
                int scope_size = Int32.Parse( textBox_scope_size.Text);
                double scope_time = Double.Parse(textBox_scope_times.Text);



                form_cross = new Form_cross();
                form_cross.Height = 0;
                form_cross.Show();
            }
            else {
                form_cross = null;
            }
        }

    }
}
