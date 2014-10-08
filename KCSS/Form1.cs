using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace KCSS
{
    public partial class Form1 : Form
    {
        RamGecTools.MouseHook mouseHook = new RamGecTools.MouseHook();

        //メモリ撮影用
        Bitmap[] tmpbmp = new Bitmap[10];
        int timer1Counter = 0;
        int timer1Counter2 = 0;
        bool timer1flag = false;

        public Form1()
        {
            InitializeComponent();
            
            string xmlPath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
            if (!System.IO.File.Exists(xmlPath))
            {
                System.IO.File.Create(xmlPath);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckWindow Cwin = new CheckWindow();
            Point Wp = Cwin.KanCollePosition();
            textBox1.Text = Wp.X.ToString();
            textBox2.Text = Wp.Y.ToString();
            MessageBox.Show("位置を設定しました。","確認",
                            MessageBoxButtons.OK,MessageBoxIcon.None);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = screenShot();
            Rectangle rect = new Rectangle(int.Parse(textBox1.Text), int.Parse(textBox2.Text), 800, 480);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            Form2 form2 = new Form2();
            form2.Show();
            form2.pictureBox1.Image = bmp;
        }

        private void savePicture(Bitmap bmp,System.Drawing.Imaging.ImageFormat format)
        {
            bmp.Save(textBox3.Text + @"\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + format.ToString(), format);
            bmp.Dispose();
        }

        private void savePicture2(System.Drawing.Imaging.ImageFormat format, DateTime date)
        {
            if (!System.IO.Directory.Exists(textBox3.Text + @"\" + date.ToString("yyyy-MM-dd-HH-mm-ss")))
            {
                System.IO.Directory.CreateDirectory(textBox3.Text + @"\" + date.ToString("yyyy-MM-dd-HH-mm-ss"));
                for (int i = 0; i < 10; i++)
                {

                    tmpbmp[i].Save(textBox3.Text + @"\" + date.ToString("yyyy-MM-dd-HH-mm-ss") + @"\" + i.ToString() + "." + format.ToString(), format);
                    tmpbmp[i].Dispose();

                }
                if (radioButton5.Checked) { timer1.Start(); }
            }
            
        }

        private Bitmap screenShot()
        {
            System.Windows.Forms.Cursor.Hide();

            Bitmap bmp = new Bitmap(800, 480);
            Rectangle rect = new Rectangle(int.Parse(textBox1.Text), int.Parse(textBox2.Text), 800, 480);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
                g.Dispose();
            }

            System.Windows.Forms.Cursor.Show();
            return bmp;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            runMouseHook();//mouseHook
            if (radioButton5.Checked){timer1.Start();}
            if (radioButton4.Checked) 
            {
                timer2.Interval = int.Parse(textBox4.Text);
                timer2.Start(); 
            }
            notifyIcon1.BalloonTipText = "艦これ監視中です…";
            notifyIcon1.ShowBalloonTip(2000);
            e.Cancel = true;
            this.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "フォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                textBox3.Text = fbd.SelectedPath;
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton1.Checked = false;
            }
            else
            {
                radioButton1.Checked = true;
            }
            if (!radioButton3.Checked && !radioButton2.Checked)
            {
                radioButton1.Checked = true;
            }
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton2.Checked = false;
            }
            else
            {
                radioButton2.Checked = true;
            }
            if (!radioButton1.Checked && !radioButton3.Checked)
            {
                radioButton2.Checked = true;
            }
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                radioButton3.Checked = false;
            }
            else
            {
                radioButton3.Checked = true;
            }
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                radioButton3.Checked = true;
            }
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                radioButton6.Checked = false;
            }
            else
            {
                radioButton6.Checked = true;
            }

            if (!radioButton4.Checked && !radioButton5.Checked)
            {
                radioButton6.Checked = true;
            }
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                radioButton5.Checked = false;
            }
            else
            {
                radioButton5.Checked = true;
            }
            if (!radioButton4.Checked && !radioButton6.Checked)
            {
                radioButton5.Checked = true;
            }

        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                radioButton4.Checked = false;
            }
            else
            {
                radioButton4.Checked = true;
            }
            if (!radioButton6.Checked && !radioButton5.Checked)
            {
                radioButton4.Checked = true;
            }
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            mouseHook.Uninstall();
            if (radioButton5.Checked) { timer1.Stop(); }
            if (radioButton4.Checked) { timer2.Stop(); }
            this.Visible = true;
            this.Activate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mouseHook.Uninstall();
            this.Dispose();
        }

        private void runMouseHook()
        {
            mouseHook.RightButtonDown += new RamGecTools.MouseHook.MouseHookCallback(mouseHook_RightButtonDown);

            mouseHook.Install();
        }

        void mouseHook_RightButtonDown(RamGecTools.MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            int X = int.Parse(textBox1.Text);
            int Y = int.Parse(textBox2.Text);

            if (X < mouseStruct.pt.x && mouseStruct.pt.x < X + 800 && Y < mouseStruct.pt.y && mouseStruct.pt.y < Y + 480)
            {
                if (checkBox1.Checked)
                {
                    notifyIcon1.BalloonTipText = "画像を保存しました。";
                    notifyIcon1.ShowBalloonTip(2000);
                }

                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                if (radioButton1.Checked) { format = System.Drawing.Imaging.ImageFormat.Png; }
                if (radioButton2.Checked) { format = System.Drawing.Imaging.ImageFormat.Jpeg; }
                if (radioButton3.Checked) { format = System.Drawing.Imaging.ImageFormat.Bmp; }

                if (radioButton6.Checked) { savePicture(screenShot(), format); }
                if (radioButton5.Checked) { timer1flag = true; }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1Counter > 9) timer1Counter = 0;
            //tmpbmp[timer1Counter].Dispose();
            tmpbmp[timer1Counter] = screenShot();
            timer1Counter++;
            if (timer1flag)
            {
                timer1Counter2++;
                if (timer1Counter2 > 4)
                {
                    timer1.Stop();
                    timer1Counter2 = 0;
                    timer1flag = false;
                    System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                    if (radioButton1.Checked) { format = System.Drawing.Imaging.ImageFormat.Png; }
                    if (radioButton2.Checked) { format = System.Drawing.Imaging.ImageFormat.Jpeg; }
                    if (radioButton3.Checked) { format = System.Drawing.Imaging.ImageFormat.Bmp; }
                    savePicture2(format, DateTime.Now);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
            if (radioButton1.Checked) { format = System.Drawing.Imaging.ImageFormat.Png; }
            if (radioButton2.Checked) { format = System.Drawing.Imaging.ImageFormat.Jpeg; }
            if (radioButton3.Checked) { format = System.Drawing.Imaging.ImageFormat.Bmp; }

            savePicture(screenShot(), format); 
        }
    }
}
