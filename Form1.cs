using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Configuration;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace bark
{
    public partial class Form1 : Form

    {
        int proxy_flag = 1;
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮 
            if (WindowState == FormWindowState.Minimized)
            {
                //托盘显示图标等于托盘图标对象 
                //注意notifyIcon1是控件的名字而不是对象的名字 
                //notifyIcon1.Icon = ico;
                //隐藏任务栏区图标 
                this.ShowInTaskbar = false;
                //图标显示在托盘区 
                notifyIcon1.Visible = true;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            // 注意判断关闭事件reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //取消"关闭窗口"事件
                e.Cancel = true; // 取消关闭窗体 

                //使关闭时窗口向右下角缩小的效果
                WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                //this.m_cartoonForm.CartoonClose();
                this.Hide();
                return;
            }
        }
        //双击图标显示窗口
        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
            else
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }



        public Form1()
        {
            InitializeComponent();
            bool flag = false;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, "bark", out flag);
            if (!flag)
            {
                MessageBox.Show("程序已运行", "请确定", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);//退出程序  
                //Application.Exit();
            }

        }

        //修改url_key
        private void button2_Click(object sender, EventArgs e)
        {
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["url_key"].Value = textBox1.Text;
            config.Save();
            MessageBox.Show("修改成功");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url="";
            if (radioButton1.Checked)
            {
             
                url = textBox1.Text + textBox3.Text;
            }

            else if (radioButton2.Checked)
            {
                
                url = textBox1.Text + textBox2.Text + "/" + textBox3.Text;
                
            }

            else if (radioButton3.Checked)
            {
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                url =textBox1.Text + "网址?url=" + textBox4.Text;
              
            }
            else if (radioButton4.Checked)
            {
                textBox4.Enabled = false;
                url = textBox1.Text + textBox3.Text+ "?automaticallyCopy=1 ";
                //MessageBox.Show(url);
            }



            UriBuilder uri = new UriBuilder(url);
            if (proxy_flag == 1) {
                WebProxy myproxy = new WebProxy("http://" + textBox5.Text + ":" + textBox6.Text); //添加代理服务器
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate; //代理服务器ssl证书
            }
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.Uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var str = reader.ReadToEnd();
            //MessageBox.Show(str);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //读取配置
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            textBox1.Text=config.AppSettings.Settings["url_key"].Value;
            textBox5.Text= config.AppSettings.Settings["proxy_ip"].Value;
            textBox6.Text = config.AppSettings.Settings["proxy_port"].Value;
        }

        private void 推送剪切板ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要退出？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {

                this.notifyIcon1.Visible = false;
                this.Close();
                Dispose();
                System.Environment.Exit(System.Environment.ExitCode);

            }
        }
        //手动打开窗口推送
        private void 打开并手动推动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
            else
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = false;
            textBox3.Enabled = true;
            textBox4.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = true;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = false;
            textBox3.Enabled = true;
            textBox4.Enabled = false;
        }

        private void 推送剪切板内容ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            UriBuilder uri = new UriBuilder(textBox1.Text+Clipboard.GetText());
            if (proxy_flag == 1) {
                WebProxy myproxy = new WebProxy("http://" + textBox5.Text + ":" + textBox6.Text);//添加代理服务器
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;//代理服务器ssl证书
            }
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.Uri);
            //request.Proxy = myproxy;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var str = reader.ReadToEnd();
            MessageBox.Show("推送:" + "'"+Clipboard.GetText()+"'"+"成功");
        }
        //代理服务器ssl证书函数
        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            //为了通过证书验证，总是返回true
            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["proxy_ip"].Value = textBox5.Text;
            config.AppSettings.Settings["proxy_port"].Value = textBox6.Text;
            config.Save();
            MessageBox.Show("修改成功代理服务器成功");
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                proxy_flag = 1;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
            }

            else
            {
                proxy_flag = 0;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
            }
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Substring(textBox1.Text.Trim().Length - 1, 1) == "/")
            {
                textBox1.Text = textBox1.Text.Trim();
            }
            else
            {
                textBox1.Text = textBox1.Text + "/";
            }
        }
    }
}
